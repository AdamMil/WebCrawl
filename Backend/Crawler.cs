using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using BinaryReader = AdamMil.IO.BinaryReader;
using BinaryWriter = AdamMil.IO.BinaryWriter;

namespace WebCrawl.Backend
{

#region Enums
public enum DirectoryNavigation
{
  Same, Down, Up, UpAndDown
}

public enum DomainNavigation
{
  SameHostName, SameDomain, SameTLD, Everywhere
}

[Flags]
public enum Download
{
  None=0, Unknown=0, Html=1, NonHtml=2, TypeMask=Everything, Everything=Html|NonHtml, PrioritizeHtml=4, NearFiles=8
}

[Flags]
public enum ProgressType
{
  None=0, UrlQueued=1, DownloadStarted=2, DownloadFinished=4, NonFatalErrorOccurred=8, FatalErrorOccurred=16,
  AnyErrorOccurred = NonFatalErrorOccurred | FatalErrorOccurred,
  All = UrlQueued|DownloadStarted|DownloadFinished|AnyErrorOccurred
}
#endregion

public delegate void ContentFilter(string url, ref string content, ref string mimeType);
public delegate string LocalPathCreator(string relativeUrl);
public delegate void ProgressHandler(Resource resource);
public delegate void SimpleEventHandler();
public delegate Uri UriFilter(Uri uri);

#region MimeOverride
public struct MimeOverride
{
  public MimeOverride(string extension, string mimeType)
  {
    this.Extension = extension;
    this.MimeType  = mimeType;
  }

  public string Extension, MimeType;
}
#endregion

#region Resource
public struct Resource
{
  internal Resource(Uri uri, string referrer, int depth, Crawler.LinkType type, bool external)
  {
    this.uri      = uri;
    this.referrer = referrer;
    this.depth    = depth;
    this.type     = type;
    this.external = external;
    
    responseCode = HttpStatusCode.Unused;
    responseText = localPath = contentType = null;
    status       = ProgressType.None;
    retries      = 0;
  }

  internal Resource(BinaryReader reader)
  {
    referrer = reader.ReadStringWithLength();
    localPath = reader.ReadStringWithLength();
    responseText = reader.ReadStringWithLength();
    contentType = reader.ReadStringWithLength();
    uri = new Uri(reader.ReadStringWithLength());
    responseCode = (HttpStatusCode)reader.ReadInt32();
    depth = reader.ReadInt32();
    retries = reader.ReadInt32();
    status = (ProgressType)reader.ReadInt32();
    type = (Crawler.LinkType)reader.ReadInt32();
    external = reader.ReadBool();
  }

  public string Referrer
  {
    get { return referrer; }
  }

  public string LocalPath
  {
    get { return localPath; }
  }

  public string ResponseText
  {
    get { return responseText; }
  }

  public string ContentType
  {
    get { return contentType; }
  }

  public Uri Uri
  {
    get { return uri; }
  }

  public HttpStatusCode ResponseCode
  {
    get { return responseCode; }
  }

  public ProgressType Status
  {
    get { return status; }
  }

  public int Depth
  {
    get { return depth; }
  }
  
  public int Retries
  {
    get { return retries; }
  }

  public bool External
  {
    get { return external; }
  }
  
  internal bool IsValid
  {
    get { return Uri != null && Uri.IsAbsoluteUri; }
  }

  internal void Write(BinaryWriter writer)
  {
    writer.WriteStringWithLength(referrer);
    writer.WriteStringWithLength(localPath);
    writer.WriteStringWithLength(responseText);
    writer.WriteStringWithLength(contentType);
    writer.WriteStringWithLength(uri.ToString());
    writer.Write((int)responseCode);
    writer.Write(depth);
    writer.Write(retries);
    writer.Write((int)status);
    writer.Write((int)type);
    writer.Write(external);
  }
  
  internal string referrer, localPath, responseText, contentType;
  Uri uri;
  internal HttpStatusCode responseCode;
  internal int depth, retries;
  internal ProgressType status;
  internal Crawler.LinkType type;
  bool external;
}
#endregion

#region Crawler
public sealed class Crawler : IDisposable
{
  ~Crawler()
  {
    Dispose(true);
  }

  public event ContentFilter FilterContent;
  public event LocalPathCreator CreatePath;
  public event ProgressHandler Progress;
  public event UriFilter FilterUris;
  
  public string BaseDirectory
  {
    get { return baseDir; }
  }

  public bool CaseSensitivePaths
  {
    get { return pathComparison == StringComparison.Ordinal; }
    set { pathComparison = value ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase; }
  }

  public int ConnectionIdleTimeout
  {
    get { return idleTimeout; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException();
      idleTimeout = value;
    }
  }

  public int CurrentBytesPerSecond
  {
    get
    {
      if(!IsInitialized) return 0;

      int bytesPerSecond = 0;
      lock(threads)
      {
        foreach(ConnectionThread thread in threads)
        {
          if(thread.IsRunning) bytesPerSecond += thread.CurrentBytesPerSecond;
        }
      }
      return bytesPerSecond;
    }
  }

  public int CurrentDownloadCount
  {
    get
    {
      return currentActiveThreads;
    }
  }
  
  public int CurrentLinksQueued
  {
    get
    {
      if(!IsInitialized) return 0;

      int totalLinks = 0;
      lock(services)
      {
        foreach(Service service in services.Values)
        {
          totalLinks += service.ResourceCount;
        }
      }
      return totalLinks;
    }
  }

  public string DefaultReferrer
  {
    get { return defaultReferrer; }
    set { defaultReferrer = value; }
  }

  public DirectoryNavigation DirectoryNavigation
  {
    get { return dirNav; }
    set { dirNav = value; }
  }

  public DomainNavigation DomainNavigation
  {
    get { return domainNav; }
    set { domainNav = value; }
  }

  public Download Download
  {
    get { return download; }
    set { download = value; }
  }

  public bool EnableUrlHacks
  {
    get { return urlHacks; }
    set { urlHacks = value; }
  }

  public bool GenerateFilesOnError
  {
    get { return errorFiles; }
    set { errorFiles = value; }
  }

  public bool IsInitialized
  {
    get { return baseDir != null; }
  }

  public int MaxConnectionsPerServer
  {
    get { return connsPerServer; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException();
      connsPerServer = value;
    }
  }
  
  public int MaxConnections
  {
    get { return maxConnections; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException();
      maxConnections = value;
    }
  }

  public int MaxDepth
  {
    get { return maxDepth; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException();
      maxDepth = value;
    }
  }

  public int MaxQueuedLinks
  {
    get { return maxQueuedLinks; }
    set
    {
      if(value < 0) throw new ArgumentOutOfRangeException();
      maxQueuedLinks = value;
    }
  }

  public int MaxQueryStringsPerFile
  {
    get { return maxQueryStrings; }
    set
    {
      if(value < 0) throw new ArgumentOutOfRangeException();
      maxQueryStrings = value;
    }
  }

  public int MaxRedirects
  {
    get { return maxRedirects; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException();
      maxRedirects = value;
    }
  }

  public int MaxRetries
  {
    get { return retries; }
    set
    {
      if(value < 0) throw new ArgumentOutOfRangeException();
      retries = value;
    }
  }

  public bool PassiveFtp
  {
    get { return passiveFtp; }
    set { passiveFtp = value; }
  }

  public string PreferredLanguage
  {
    get { return language; }
    set { language = value; }
  }

  public ProgressType ProgressFilter
  {
    get { return progress; }
    set { progress = value; }
  }

  public int ReadTimeout
  {
    get { return ioTimeout; }
    set
    {
      if(value < 0) throw new ArgumentOutOfRangeException();
      ioTimeout = value;
    }
  }

  public bool RewriteLinks
  {
    get { return rewriteLinks; }
    set { rewriteLinks = value; }
  }

  public int TransferTimeout
  {
    get { return transferTimeout; }
    set
    {
      if(value < 0) throw new ArgumentOutOfRangeException();
      transferTimeout = value;
    }
  }

  public bool UseCookies
  {
    get { return useCookies; }
    set { useCookies = value; }
  }

  public string UserAgent
  {
    get { return userAgent; }
    set { userAgent = value; }
  }

  public void Initialize(string baseDirectory)
  {
    if(disposed) throw new ObjectDisposedException("Crawler");
    Deinitialize();
    Directory.CreateDirectory(baseDirectory);
    baseDir = baseDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
  }

  public void Deinitialize()
  {
    if(IsInitialized)
    {
      Terminate(0);
      ClearUris();
      services.Clear();
      baseDir = null;
    }
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    Dispose(false);
  }

  public void AddBaseUri(Uri uri, bool enqueue)
  {
    if(uri == null) throw new ArgumentNullException();
    if(!uri.IsAbsoluteUri) throw new ArgumentNullException("The uri must be absolute.");
    AssertInitialized();

    lock(baseUris) baseUris.Add(uri);
    if(enqueue) EnqueueUri(uri, null, 0, LinkType.Link, true);
  }

  public void ClearUris()
  {
    lock(services)
    {
      foreach(Service service in services.Values)
      {
        service.Clear();
      }
    }
    baseUris.Clear();
  }

  public void RemoveUris(Regex uriFilter, bool allowRequeue)
  {
    lock(services)
    {
      foreach(Service service in services.Values)
      {
        service.Remove(uriFilter, allowRequeue);
      }
    }
  }

  public void SaveState(BinaryWriter writer)
  {
    AssertInitialized();
    if(running || CurrentDownloadCount != 0)
    {
      throw new InvalidOperationException("The crawler must be fully stopped to save its state.");
    }

    writer.WriteStringWithLength(userAgent);
    writer.WriteStringWithLength(language);
    writer.WriteStringWithLength(defaultReferrer);
    writer.WriteStringWithLength(baseDir);
    
    writer.Write((int)dirNav);
    writer.Write((int)domainNav);
    writer.Write((int)download);
    writer.Write((int)pathComparison);
    writer.Write((int)progress);
    writer.Write(idleTimeout);
    writer.Write(connsPerServer);
    writer.Write(maxConnections);
    writer.Write(maxDepth);
    writer.Write(retries);
    writer.Write(maxQueuedLinks);
    writer.Write(ioTimeout);
    writer.Write(transferTimeout);
    writer.Write(maxRedirects);
    writer.Write(maxQueryStrings);
    writer.Write(rewriteLinks);
    writer.Write(useCookies);
    writer.Write(errorFiles);
    writer.Write(urlHacks);
    writer.Write(passiveFtp);

    writer.Write(mimeOverrides.Count);
    foreach(KeyValuePair<string,string> pair in mimeOverrides)
    {
      writer.WriteStringWithLength(pair.Key);
      writer.WriteStringWithLength(pair.Value);
    }
    
    writer.Write(services.Count);
    foreach(Service service in services.Values) service.Write(writer);
    
    writer.Write(baseUris.Count);
    foreach(Uri uri in baseUris) writer.WriteStringWithLength(uri.ToString());
  }

  public void Start()
  {
    AssertInitialized();
    
    if(!running)
    {
      running = true;

      lock(services)
      {
        foreach(Service service in services.Values)
        {
          if(!service.IsQueueEmpty) CrawlService(service);
        }
      }
    }
  }

  public void Stop()
  {
    AssertInitialized();
    running = false;

    lock(threads)
    {
      foreach(ConnectionThread thread in threads)
      {
        IdleThread(thread);
      }
    }
  }

  public void Terminate(int timeToWait)
  {
    if(timeToWait < 0 && timeToWait != Timeout.Infinite) throw new ArgumentOutOfRangeException();
    if(!IsInitialized) return;

    lock(threads)
    {
      Stop();

      DateTime start = DateTime.Now;

      foreach(ConnectionThread thread in threads)
      {
        thread.Terminate(timeToWait);

        if(timeToWait > 0)
        {
          timeToWait -= (int)(DateTime.Now - start).TotalMilliseconds;
          if(timeToWait < 0) timeToWait = 0;
        }
      }
    }
  }

  public void EnqueueUri(Uri uri)
  {
    EnqueueUri(uri, null, 0, LinkType.Link, true);
  }

  #region Mime overrides
  public void AddMimeOverride(string extension, string mimeType)
  {
    AddMimeOverride(new MimeOverride(extension, mimeType));
  }

  public void AddMimeOverride(MimeOverride mimeOverride)
  {
    if(string.IsNullOrEmpty(mimeOverride.Extension) || string.IsNullOrEmpty(mimeOverride.MimeType))
    {
      throw new ArgumentException("The override had an empty extension or mime type.");
    }
    
    lock(mimeOverrides)
    {
      mimeOverrides["."+mimeOverride.Extension.ToLowerInvariant()] = mimeOverride.MimeType.ToLowerInvariant();
    }
  }

  public void AddStandardMimeOverrides()
  {
    AddMimeOverride("htm",   "text/html");
    AddMimeOverride("html",  "text/html");
    AddMimeOverride("sht",   "text/html");
    AddMimeOverride("shtm",  "text/html");
    AddMimeOverride("shtml", "text/html");
    AddMimeOverride("php",   "text/html");
    AddMimeOverride("php2",  "text/html");
    AddMimeOverride("php3",  "text/html");
    AddMimeOverride("php4",  "text/html");
    AddMimeOverride("asp",   "text/html");
    AddMimeOverride("aspx",  "text/html");
    AddMimeOverride("jsp",   "text/html");
    AddMimeOverride("cgi",   "text/html");
    AddMimeOverride("cfm",   "text/html");
    AddMimeOverride("pl",    "text/html");
  }

  public MimeOverride[] GetMimeOverrides()
  {
    MimeOverride[] array = new MimeOverride[mimeOverrides.Count];

    lock(mimeOverrides)
    {
      int index = 0;
      foreach(KeyValuePair<string,string> pair in mimeOverrides)
      {
        array[index] = new MimeOverride(pair.Key, pair.Value);
      }
    }
    
    return array;
  }

  public void SetMimeOverrides(params MimeOverride[] overrides)
  {
    foreach(MimeOverride mo in overrides)
    {
      if(string.IsNullOrEmpty(mo.Extension) || string.IsNullOrEmpty(mo.MimeType))
      {
        throw new ArgumentException("A mime override had an empty extension or mime type.");
      }
    }

    lock(mimeOverrides)
    {
      mimeOverrides.Clear();
      foreach(MimeOverride mo in overrides)
      {
        mimeOverrides[mo.Extension.ToLowerInvariant()] = mo.MimeType.ToLowerInvariant();
      }
    }
  }
  #endregion

  #region ConnectionThread
  sealed class ConnectionThread
  {
    public ConnectionThread(Crawler crawler)
    {
      if(crawler == null) throw new ArgumentNullException();
      this.crawler = crawler;
    }

    public int CurrentBytesPerSecond
    {
      get { return lastBytesPerSecond; }
    }

    public bool IsRunning
    {
      get
      {
        Thread thread = this.thread;
        return !shouldQuit && thread != null && thread.IsAlive;
      }
    }
    
    public bool IsStopping
    {
      get { return shouldQuit; }
    }

    public Service Service
    {
      get { return service; }
    }

    public void Start(Service service)
    {
      if(shouldQuit) throw new InvalidOperationException("The thread is in the process of quitting.");
      if(service == null) throw new ArgumentNullException();
      if(service == this.service) return;

      if(IsStopping) Terminate(0); // if the thread is in the process of stopping, finish it immediately

      this.service = service;
      this.service.connections++;
      crawler.currentActiveThreads++;

      if(thread == null)
      {
        thread = new Thread(ThreadFunc);
        thread.Start();
      }
    }

    public void Stop()
    {
      shouldQuit = true;
    }

    public void Terminate(int timeToWait)
    {
      if(timeToWait < 0 && timeToWait != Timeout.Infinite) throw new ArgumentOutOfRangeException();

      if(thread != null)
      {
        Stop();
        if(!thread.Join(timeToWait)) thread.Abort();
      }

      Disassociate();
    }

    void CleanupRequest()
    {
      if(responseStream != null)
      {
        responseStream.Close();
        responseStream = null;
      }

      if(response != null)
      {
        response.Close();
        response = null;
      }

      if(request != null)
      {
        request.Abort();
        request = null;
      }
    }

    void Disassociate()
    {
      CleanupRequest();

      if(service != null)
      {
        crawler.currentActiveThreads--;
        service.connections--;
        service = null;
      }

      lastBytesPerSecond = 0;
      thread = null;
      shouldQuit = false;
    }

    void ThreadFunc()
    {
      while(!shouldQuit)
      {
        CleanupRequest();
        if(service.CurrentConnections > crawler.MaxConnectionsPerServer || // if the service has too many connections,
           !service.TryDequeue(out resource)) // or the service has no more resources for us to process...
        {
          crawler.OnThreadIdle(this); // the crawler will re-associate, stop, or terminate this thread
          continue;
        }

        resourceUri = resource.Uri;
        string localFileName = null;
        try
        {
          bool crawlerWantsEverything = (crawler.Download & Download.TypeMask) == Download.Everything;
          Download resourceType = Download.Unknown;

          if(!crawlerWantsEverything)
          {
            // we need to determine whether we actually want to download this item. try to guess from the Url
            resourceType = crawler.GuessResourceType(resourceUri);
            // we can't be sure about resources from FTP, so assume they're non-html
            if(resourceType == Download.Unknown && resourceUri.Scheme == "ftp")
            {
              resourceType = Download.NonHtml;
            }

            // skip non-Html resources if we don't want them. we'll still need to download Html resources to scan them.
            if(resourceType == Download.NonHtml && !crawler.WantResource(resourceType)) continue;
          }

          request = WebRequest.Create(resourceUri);

          HttpWebRequest httpRequest = request as HttpWebRequest;
          if(httpRequest != null)
          {
            SetupHttpRequest(httpRequest);

            if(!crawlerWantsEverything && resourceType == Download.Unknown)
            {
              httpRequest.Method = "HEAD"; // use a HEAD request to determine the content type before downloading it
              response = httpRequest.GetResponse();

              if(crawler.UseCookies) Service.SaveCookies((HttpWebResponse)response);
              resourceType = Crawler.GetResourceType(GetMimeType(((HttpWebResponse)response).ContentType));
              response.Close();

              if(!crawler.WantResource(resourceType)) continue;

              request = WebRequest.Create(resourceUri); // create a new request to retrieve the actual content
              httpRequest = (HttpWebRequest)request;
              SetupHttpRequest(httpRequest);
            }
          }

          FtpWebRequest ftpRequest = request as FtpWebRequest;
          if(ftpRequest != null)
          {
            crawler.SetServicePointProperties(ftpRequest.ServicePoint);
            ftpRequest.ReadWriteTimeout =
              crawler.ReadTimeout == 0 ? Timeout.Infinite : crawler.ReadTimeout*1000;
            ftpRequest.UsePassive = crawler.PassiveFtp;
          }

          // at this point, we know we probably want the data, so create the file and download it
          localFileName = service.GetLocalFileName(resourceUri, resource.type);
          // if the filename is null, it means this resource is not wanted. this can happen for a file with too many
          if(localFileName == null) continue; // query strings (to prevent the crawler from crawling it infinitely)

          resource.localPath = localFileName;
          crawler.OnProgress(ref resource, ProgressType.DownloadStarted);

          request.Timeout = crawler.TransferTimeout == 0 ? Timeout.Infinite : crawler.TransferTimeout*1000;
          response = request.GetResponse();

          HttpWebResponse httpResponse = response as HttpWebResponse;
          if(httpResponse != null)
          {
            if(crawler.UseCookies) Service.SaveCookies(httpResponse);
            if(resourceType == Download.Unknown)
            {
              resourceType = Crawler.GetResourceType(GetMimeType(httpResponse.ContentType));
            }
            resource.responseCode = httpResponse.StatusCode;
            resource.responseText = httpResponse.StatusDescription;

            // if the response Uri is different from the resource Uri, it was probably a redirection of some sort.
            // get a new local file name.
            if(!httpResponse.ResponseUri.Equals(resourceUri))
            {
              resourceUri = httpResponse.ResponseUri;

              // ensure that the place we redirected to is an allowed Url
              bool isExternal;
              if(!crawler.IsUriAllowed(httpResponse.ResponseUri, resource.External, out isExternal))
              {
                localFileName = null;
              }
              else
              {
                localFileName = service.GetLocalFileName(httpResponse.ResponseUri, resource.type);
              }
            }
          }
          else if(response is FtpWebResponse)
          {
            resource.responseText = ((FtpWebResponse)response).StatusDescription;
          }

          if(localFileName == null) // if we later discovered that this is not what we want, just consume the stream.
          {                         // (for instance, if it redirected to an invalid url)
            response.Close();
          }
          else if(resourceType == Download.Html && crawler.RewriteLinks)
          {
            Encoding encoding = GetEncoding(httpResponse);
            string html;

            DateTime startTime = DateTime.Now;
            using(StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
            {
              html = sr.ReadToEnd();
            }
            double totalSeconds = (DateTime.Now-startTime).TotalSeconds;
            if(totalSeconds == 0) totalSeconds = 0.1;

            html = ScanForAndRewriteLinks(html, Path.GetDirectoryName(localFileName));

            byte[] bytes = encoding.GetBytes(html);
            using(FileStream file = new FileStream(localFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
              file.Write(bytes, 0, bytes.Length);
            }

            lastBytesPerSecond = (int)Math.Round(bytes.Length / totalSeconds);
          }
          else
          {
            DateTime startTime = DateTime.Now;
            int size = CopyStream(response.GetResponseStream(),
                                  new FileStream(localFileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            double totalSeconds = (DateTime.Now-startTime).TotalSeconds;
            if(totalSeconds == 0) totalSeconds = 0.1;
            lastBytesPerSecond = (int)Math.Round(size / totalSeconds);

            if(resourceType == Download.Html)
            {
              string html;
              using(StreamReader sr = new StreamReader(localFileName, GetEncoding(httpResponse)))
              {
                html = sr.ReadToEnd();
              }
              ScanForLinks(html);
            }
          }

          crawler.OnProgress(ref resource, ProgressType.DownloadFinished);
        }
        catch(WebException ex)
        {
          bool isFatal = (++resource.retries > crawler.MaxRetries && crawler.MaxRetries != 0) || IsFatalError(ex);
          crawler.OnErrorOccurred(ref resource, ex, isFatal);
          if(!isFatal)
          {
            service.Enqueue(ref resource, true);
          }
          else if(localFileName != null)
          {
            try { File.Delete(localFileName); } catch { }
          }
        }
        catch(IOException ex) // an IO exception occurs if the other side closes the connection, for instance
        {
          bool isFatal = ++resource.retries > crawler.MaxRetries && crawler.MaxRetries != 0;
          crawler.OnErrorOccurred(ref resource, ex, isFatal);
          if(!isFatal)
          {
            service.Enqueue(ref resource, true);
          }
          else if(localFileName != null)
          {
            try { File.Delete(localFileName); }
            catch { }
          }
        }
        catch(Exception ex)
        {
          crawler.OnErrorOccurred(ref resource, ex, true);
          if(localFileName != null)
          {
            try { File.Delete(localFileName); } catch { }
          }
        }
      }
      
      Disassociate();
    }

    Uri GetAbsoluteLinkUrl(Uri baseUri, Group linkGroup, bool decodeEntities)
    {
      string linkText = linkGroup.Value;
      if(decodeEntities) linkText = HttpUtility.HtmlDecode(linkText);
      if(linkText.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
         linkText.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
      {
        return null;
      }

      Uri newUri = new Uri(linkText, UriKind.RelativeOrAbsolute);
      if(!newUri.IsAbsoluteUri) // if the link is not absolute, make it absolute
      {
        newUri = new Uri(baseUri, newUri);
      }
      return newUri;
    }

    Group GetLinkMatchGroup(Match m, out LinkType type)
    {
      type = LinkType.Link;
      Group g = m.Groups["link"];
      if(!g.Success)
      {
        g = m.Groups["resLink"];
        type = service.External ? LinkType.ExternalResource : LinkType.InternalResource;
      }
      return g;
    }

    static string GetMimeType(string contentType)
    {
      if(contentType == null) return null;
      int endPos = contentType.IndexOf(';');
      return endPos == -1 ? contentType : contentType.Substring(0, endPos);
    }

    void HandleLinkMatch(Uri baseUri, string referrer, Match m, bool decodeEntities)
    {
      LinkType type;
      Uri uri = crawler.FilterUri(GetAbsoluteLinkUrl(baseUri, GetLinkMatchGroup(m, out type), decodeEntities));
      if(uri != null)
      {
        try { crawler.EnqueueUri(uri, referrer, resource.Depth+1, type, false); }
        catch(UriFormatException) { }
      }
    }

    // TODO: scan inside CSS files (anything coming from a CSS link or having a text/css mime type).
    // scan inside javascript too.
    void ScanForLinks(string html)
    {
      Uri baseUri = resourceUri; // use the resource uri as the base uri by default
      string referrer = resourceUri.GetLeftPart(UriPartial.Query);

      Match m = baseRe.Match(html); // but if the document specifies a different base Uri, use it.
      if(m.Success)
      {
        try { baseUri = new Uri(m.Value); }
        catch(UriFormatException) { }
      }
      
      m = linkRe.Match(html); // now look for links in the HTML
      while(m.Success)
      {
        HandleLinkMatch(baseUri, referrer, m, true);
        m = m.NextMatch();
      }
      
      m = styleRe.Match(html); // now look for style blocks
      while(m.Success)
      {
        Match linkMatch = styleLinkRe.Match(m.Groups["css"].Value); // and links within the style blocks
        while(linkMatch.Success)
        {
          HandleLinkMatch(baseUri, referrer, linkMatch, false);
          linkMatch = linkMatch.NextMatch();
        }
        
        m = m.NextMatch();
      }
    }
    
    string ScanForAndRewriteLinks(string html, string localDir)
    {
      Uri baseUri = resourceUri; // use the resource uri as the base uri by default
      string referrer = resourceUri.GetLeftPart(UriPartial.Query);
      
      Match m = baseRe.Match(html);
      if(m.Success) // but if the document specifies a different base Uri, use it.
      {
        try { baseUri = new Uri(m.Value); }
        catch(UriFormatException) { }
        html = baseRe.Replace(html, "."); // replace the base with '.'
      }

      MatchEvaluator htmlReplacer =
        delegate(Match match) { return RewriteHtmlLink(match, baseUri, referrer, localDir, true); };
      MatchEvaluator cssReplacer =
        delegate(Match match) { return RewriteHtmlLink(match, baseUri, referrer, localDir, false); };
      html = linkRe.Replace(html, htmlReplacer);
      html = styleRe.Replace(html, delegate(Match match) { return styleLinkRe.Replace(match.Value, cssReplacer); });
      return html;
    }

    void SetupHttpRequest(HttpWebRequest httpRequest)
    {
      crawler.SetServicePointProperties(httpRequest.ServicePoint);

      if(crawler.UseCookies) service.LoadCookies(httpRequest);

      string referrer = resource.Referrer;
      if(string.IsNullOrEmpty(referrer)) referrer = crawler.DefaultReferrer;
      if(!string.IsNullOrEmpty(referrer))
      {
        httpRequest.Referer = referrer;
      }

      if(!string.IsNullOrEmpty(crawler.UserAgent))
      {
        httpRequest.UserAgent = crawler.UserAgent;
      }

      if(!string.IsNullOrEmpty(crawler.PreferredLanguage))
      {
        httpRequest.Headers[HttpRequestHeader.AcceptLanguage] = crawler.PreferredLanguage;
      }
      
      httpRequest.ReadWriteTimeout = crawler.ReadTimeout == 0 ? Timeout.Infinite : crawler.ReadTimeout*1000;
      httpRequest.MaximumAutomaticRedirections = crawler.MaxRedirects;
    }

    string RewriteHtmlLink(Match m, Uri baseUri, string referrer, string localDir, bool decodeEntities)
    {
      try
      {
        LinkType type;
        Group g = GetLinkMatchGroup(m, out type);
        Uri absUri = crawler.FilterUri(GetAbsoluteLinkUrl(baseUri, g, decodeEntities));
        if(absUri != null)
        {
          string prefix = m.Value.Substring(0, g.Index-m.Index);
          string suffix = m.Value.Substring(g.Index-m.Index+g.Length);
          string relUri = null;
          if(crawler.EnqueueUri(absUri, referrer, resource.Depth+1, type, false))
          {
            relUri = crawler.GetRelativeUri(localDir, absUri, type);
          }
          if(relUri == null) relUri = absUri.ToString();
          return prefix + relUri + suffix;
        }
      }
      catch(UriFormatException) { }

      return m.Value;
    }

    readonly Crawler crawler;
    Service service;

    WebRequest request;
    WebResponse response;
    Stream responseStream;
    Resource resource;
    Uri resourceUri;

    Thread thread;
    int lastBytesPerSecond;
    bool shouldQuit;

    static int CopyStream(Stream source, Stream dest)
    {
      try
      {
        byte[] buffer = new byte[4096];
        int totalSize = 0;
        while(true)
        {
          int read = source.Read(buffer, 0, 4096);
          if(read == 0) break;
          dest.Write(buffer, 0, read);
          totalSize += read;
        }
        return totalSize;
      }
      finally
      {
        dest.Close();
        source.Close();
      }
    }

    static Encoding GetEncoding(HttpWebResponse response)
    {
      if(response != null)
      {
        string encoding = response.ContentEncoding;
        if(!string.IsNullOrEmpty(encoding))
        {
          try { return Encoding.GetEncoding(encoding); }
          catch(ArgumentException) { }
        }
      }

      return Encoding.UTF8;
    }

    static bool IsFatalError(WebException e)
    {
      if(e.Status == WebExceptionStatus.ProtocolError)
      {
        HttpWebResponse httpResponse = e.Response as HttpWebResponse;
        if(httpResponse != null)
        {
          switch(httpResponse.StatusCode)
          {
            case HttpStatusCode.Ambiguous: case HttpStatusCode.Gone: case HttpStatusCode.Moved:
            case HttpStatusCode.NotAcceptable: case HttpStatusCode.NotFound: case HttpStatusCode.PaymentRequired:
            case HttpStatusCode.Redirect: case HttpStatusCode.ProxyAuthenticationRequired:
            case HttpStatusCode.TemporaryRedirect: case HttpStatusCode.RedirectMethod:
            case HttpStatusCode.Unauthorized:
              return true;
          }
        }
        
        FtpWebResponse ftpResponse = e.Response as FtpWebResponse;
        if(ftpResponse != null)
        {
          switch(ftpResponse.StatusCode)
          {
            case FtpStatusCode.AccountNeeded: case FtpStatusCode.NeedLoginAccount: case FtpStatusCode.NotLoggedIn:
            case FtpStatusCode.ServerWantsSecureSession:
              return true;
          }
        }
      }
      else if(e.Status == WebExceptionStatus.MessageLengthLimitExceeded)
      {
        return true;
      }
      
      return false;
    }

    static Regex baseRe = new Regex(@"(?<=<base\s[^>]*href\s*=\s*""?)[^"">]+", RegexOptions.Compiled |
                                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
    static Regex linkRe = new Regex(@"<(?:a\b[^>]*?\bhref\s*=\s*(?:""(?<link>[^"">]+)|'(?<link>[^'>]+))|
                                          (?:img|script|embed)\b[^>]*?\bsrc\s*=\s*(?:""(?<resLink>[^"">]+)|'(?<resLink>[^'>]+))|
                                          i?frame\b[^>]*?\bsrc\s*=\s*(?:""(?<link>[^"">]+)|'(?<link>[^'>]+))|
                                          link\b[^>]*?\bhref\s*=\s*(?:""(?<resLink>[^"">]+)|'(?<resLink>[^'>]+))|
                                          applet\b[^>]*?\b(?:code|object)\s*=\s*(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+))|
                                          object\b[^>]*?\bdata\s*=\s*(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+))|
                                          param\s+name=[""'](?:src|href|file|filename|data)[""']\s+value=(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+))|
                                          \w+\b[^>]+?\b(?:background|bgimage)\s*=\s*(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+)))",
                                    RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase |
                                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
    static Regex styleRe = new Regex(@"<style>(?<css>.*?)</style>|<[^>]+\sstyle\s*=\s*(?:""(?<css>[^"">]+)|'(?<css>[^'>]+))",
                                     RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                     RegexOptions.CultureInvariant | RegexOptions.Singleline);
    static Regex scriptRe = new Regex(@"<script>(.*?)</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                      RegexOptions.CultureInvariant | RegexOptions.Singleline);
    static Regex styleLinkRe = new Regex(@"@import ""(?<resLink>[^""]+)|url\(['""]?(?<resLink>[^)]+?)['""]?\)",
                                         RegexOptions.Compiled | RegexOptions.CultureInvariant |
                                         RegexOptions.IgnoreCase | RegexOptions.Singleline);
  }
  #endregion

  #region Service
  sealed class Service
  {
    public Service(Crawler crawler, Uri uri, bool external)
    {
      if(uri == null) throw new ArgumentNullException();
      if(!uri.IsAbsoluteUri) throw new ArgumentException("The uri must be absolute.");

      this.crawler  = crawler;
      this.baseUri  = new Uri(uri.GetLeftPart(UriPartial.Authority));
      this.external = external;
      
      InitializeBaseDirectory();
    }

    public Service(BinaryReader reader, Crawler crawler, bool external)
    {
      this.crawler  = crawler;
      this.external = external;

      baseUri = new Uri(reader.ReadStringWithLength());

      int count = reader.ReadInt32();
      while(count-- > 0) resources.Enqueue(new Resource(reader));

      count = reader.ReadInt32();
      while(count-- > 0) files.Add(reader.ReadStringWithLength(), new LocalFileInfo(reader));

      InitializeBaseDirectory();
    }
    
    static Service()
    {
      badChars = Path.GetInvalidFileNameChars();
      Array.Sort(badChars);
    }

    public string BaseDirectory
    {
      get { return baseDir; }
    }

    public Uri BaseUri
    {
      get { return baseUri; }
    }

    public int CurrentConnections
    {
      get { return connections; }
    }

    public bool External
    {
      get { return external; }
    }

    public bool IsQueueEmpty
    {
      get { lock(resources) return resources.Count == 0; }
    }

    public int ResourceCount
    {
      get { return resources.Count; }
    }

    public void Clear()
    {
      lock(resources) resources.Clear();
    }

    public void Enqueue(ref Resource resource)
    {
      Enqueue(ref resource, false);
    }

    public void Enqueue(ref Resource resource, bool forceRequeue)
    {
      if(!resource.IsValid) throw new ArgumentException();

      Uri uri = resource.Uri;
      Debug.Assert(uri.Scheme == baseUri.Scheme && uri.Port == baseUri.Port &&
                   string.Equals(uri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase));

      bool alreadyQueued = false;
      if(!forceRequeue)
      {
        string key = MakeKey(uri);
        lock(queued)
        {
          int position = queued.BinarySearch(key);
          alreadyQueued = position >= 0;
          if(!alreadyQueued) queued.Insert(~position, key);
        }
      }

      if(!alreadyQueued)
      {
        lock(resources) resources.Enqueue(resource);
        crawler.OnProgress(ref resource, ProgressType.UrlQueued);
      }
    }

    public void Remove(Regex uriRegex, bool allowRequeue)
    {
      lock(resources)
      lock(queued)
      {
        Resource[] array = resources.ToArray();
        resources.Clear();
        for(int i=0; i<array.Length; i++)
        {
          if(!uriRegex.IsMatch(array[i].Uri.ToString()))
          {
            resources.Enqueue(array[i]);
          }
          else if(allowRequeue)
          {
            int position = queued.BinarySearch(MakeKey(array[i].Uri));
            if(position >= 0) queued.RemoveAt(position);
          }
        }

      }
    }

    public bool TryDequeue(out Resource resource)
    {
      lock(resources)
      {
        if(resources.Count == 0)
        {
          resource = new Resource();
          return false;
        }
        else
        {
          resource = resources.Dequeue();
          return true;
        }
      }
    }

    public string GetLocalFileName(Uri uri, LinkType type)
    {
      string baseUriPath = uri.AbsolutePath;
      if(!crawler.CaseSensitivePaths) baseUriPath = baseUriPath.ToLowerInvariant();

      string query = uri.Query;
      if(crawler.EnableUrlHacks) query = NormalizeQuery(query);
      
      string localFileName;
      lock(files)
      {
        LocalFileInfo info;
        bool reAdd = false, touch = false;

        if(!files.TryGetValue(baseUriPath, out info))
        {
          info  = new LocalFileInfo(FindUnusedFile(baseUriPath, type));
          reAdd = touch = true;
        }

        localFileName = info.BasePath;
        
        if(!string.IsNullOrEmpty(query))
        {
          if(info.Queries == null)
          {
            info.Queries = new Dictionary<string,string>();
            reAdd = true;
          }

          string queryPart;
          if(!info.Queries.TryGetValue(query, out queryPart))
          {
            if(crawler.MaxQueryStringsPerFile != 0 && info.Queries.Count >= crawler.MaxQueryStringsPerFile)
            {
              return null;
            }

            info.Queries[query] = queryPart = FindQueryPart(localFileName, query, type);
            touch = true;
          }

          localFileName = AddQueryPart(localFileName, queryPart, type);
        }

        if(reAdd) files[baseUriPath] = info;

        // create an empty file to prevent another resource from getting the same one
        if(touch) new FileStream(localFileName, FileMode.Create, FileAccess.Write, FileShare.None).Close();
      }

      return localFileName;
    }

    public void LoadCookies(HttpWebRequest request)
    {
      if(request.CookieContainer == null) request.CookieContainer = new CookieContainer();

      lock(this)
      {
        if(cookies != null)
        {
          request.CookieContainer.Add(request.RequestUri, cookies.GetCookies(request.RequestUri));
        }
      }
    }

    public void SaveCookies(HttpWebResponse response)
    {
      if(response.Cookies == null || response.Cookies.Count == 0) return;

      lock(this)
      {
        if(cookies == null) cookies = new CookieContainer();
        try { cookies.Add(response.ResponseUri, response.Cookies); }
        catch(CookieException) { } // ignore invalid cookies
      }
    }

    public void Write(BinaryWriter writer)
    {
      writer.WriteStringWithLength(baseUri.ToString());

      writer.Write(resources.Count);
      foreach(Resource resource in resources) resource.Write(writer);

      writer.Write(files.Count);
      foreach(KeyValuePair<string,LocalFileInfo> pair in files)
      {
        writer.WriteStringWithLength(pair.Key);
        pair.Value.Write(writer);
      }
    }

    struct LocalFileInfo
    {
      public LocalFileInfo(string fileName) { BasePath = fileName; Queries = null; }

      public LocalFileInfo(BinaryReader reader)
      {
        BasePath = reader.ReadStringWithLength();

        int count = reader.ReadInt32();
        if(count == 0)
        {
          Queries = null;
        }
        else
        {
          Queries = new Dictionary<string,string>(count);
          while(count-- > 0) Queries.Add(reader.ReadStringWithLength(), reader.ReadStringWithLength());
        }
      }

      public string BasePath;
      public Dictionary<string,string> Queries;
      
      public void Write(BinaryWriter writer)
      {
        writer.WriteStringWithLength(BasePath);
        writer.Write(Queries == null ? 0 : Queries.Count);
        foreach(KeyValuePair<string,string> pair in Queries)
        {
          writer.WriteStringWithLength(pair.Key);
          writer.WriteStringWithLength(pair.Value);
        }
      }
    }

    sealed class QueryKeyComparer : IComparer<KeyValuePair<string,string>>
    {
      QueryKeyComparer() { }

      public int Compare(KeyValuePair<string,string> a, KeyValuePair<string,string> b)
      {
        return string.CompareOrdinal(a.Key, b.Key);
      }

      public readonly static QueryKeyComparer Instance = new QueryKeyComparer();
    }

    string AddQueryPart(string path, string query, LinkType type)
    {
      return GetPathWithoutExtension(path) + "_" + query + GetExtension(path, type);
    }

    string FindQueryPart(string path, string query, LinkType type)
    {
      string queryPart = ToHex((uint)query.GetHashCode()), extension = GetExtension(path, type);
      path = GetPathWithoutExtension(path) + "_";

      string queryToTest = queryPart;
      int suffix = 2;
      while(File.Exists(path+queryToTest+extension))
      {
        queryToTest = queryPart + suffix.ToString(System.Globalization.CultureInfo.InvariantCulture);
        suffix++;
      }
      
      return queryToTest;
    }

    string FindUnusedFile(string baseUriPath, LinkType type)
    {
      if(baseUriPath.StartsWith("/")) baseUriPath = baseUriPath.Substring(1);

      string[] bits = baseUriPath.Split('/');
      for(int i=0; i<bits.Length; i++) bits[i] = UrlSafeDecode(bits[i]);
      baseUriPath = string.Join("/", bits);

      string directory = this.baseDir;
      string fileName  = null, extension = GetExtension(baseUriPath, type);

      int lastSlash = baseUriPath.LastIndexOf('/');
      if(lastSlash == -1)
      {
        fileName = baseUriPath;
      }
      else
      {
        if(lastSlash != baseUriPath.Length-1)
        {
          fileName    = baseUriPath.Substring(lastSlash+1);
          baseUriPath = baseUriPath.Substring(0, lastSlash);
        }

        directory = Path.Combine(directory, baseUriPath.Replace('/', Path.DirectorySeparatorChar));
      }

      Directory.CreateDirectory(directory);

      if(string.IsNullOrEmpty(fileName)) // for things that look like directories, name the file index.html
      {
        fileName  = "index";
        extension = ".html";
      }
      else
      {
        fileName = Path.GetFileNameWithoutExtension(fileName);
      }

      fileName = Path.Combine(directory, fileName);

      string fileToTest = fileName;
      int suffix = 2;
      while(File.Exists(fileToTest+extension))
      {
        fileToTest = fileName + suffix.ToString(System.Globalization.CultureInfo.InvariantCulture);
        suffix++;
      }

      return fileToTest + extension;
    }

    string GetExtension(string path, LinkType type)
    {
      string extension = Path.GetExtension(path);
      return type != LinkType.Link ||
             !string.IsNullOrEmpty(extension) && crawler.GuessResourceType(extension) != Download.Html ?
               extension : ".html";
    }

    static string GetPathWithoutExtension(string path)
    {
      int periodPos = path.LastIndexOf('.');
      if(periodPos != -1)
      {
        int slashPos = path.LastIndexOf(Path.DirectorySeparatorChar);
        if(slashPos == -1 || periodPos > slashPos)
        {
          path = path.Substring(0, periodPos);
        }
      }
      return path;
    }

    void InitializeBaseDirectory()
    {
      string dirName = baseUri.Host.ToLowerInvariant();
      if(baseUri.Scheme != "http") dirName += "_"+baseUri.Scheme;
      if(!baseUri.IsDefaultPort) dirName += "_"+baseUri.Port.ToString(System.Globalization.CultureInfo.InvariantCulture);
      baseDir = Path.Combine(crawler.BaseDirectory, dirName);
    }

    string MakeKey(Uri uri)
    {
      string key = uri.AbsolutePath;
      if(!crawler.CaseSensitivePaths) key = key.ToLowerInvariant();

      string query = crawler.EnableUrlHacks ? uri.Query : NormalizeQuery(uri.Query);
      if(!string.IsNullOrEmpty(query)) key += query;

      return key;
    }

    // if it looks like a query string with key/value pairs (ie. ?a=A&b=B), then sort the keys so that
    // ?id=5&page=1 and ?page=1&id=5 are considered identical.
    static string NormalizeQuery(string query)
    {
      if(string.IsNullOrEmpty(query)) return null;

      Match m = queryRe.Match(query);
      if(!m.Success) return query;
      
      List<KeyValuePair<string,string>> pairs = new List<KeyValuePair<string,string>>();

      CaptureCollection keys = m.Groups["key"].Captures, values = m.Groups["value"].Captures;
      int stringLength = 0;
      for(int i=0; i<keys.Count; i++)
      {
        KeyValuePair<string,string> pair = new KeyValuePair<string,string>(keys[i].Value, values[i].Value);
        pairs.Add(pair);
        stringLength += pair.Key.Length + pair.Value.Length + 1; // +1 for '=' sign
        if(i != 0) stringLength++; // +1  for '&' sign
      }

      pairs.Sort(QueryKeyComparer.Instance);
      
      StringBuilder sb = new StringBuilder(stringLength+1);
      sb.Append('?');

      bool sep = false;
      foreach(KeyValuePair<string,string> pair in pairs)
      {
        if(sep) sb.Append('&');
        else sep = true;
        sb.Append(pair.Key).Append('=').Append(pair.Value);
      }
      return sb.ToString();
    }

    static string ToHex(uint value)
    {
      const string hexChars = "0123456789abcdef";
      char[] result = new char[8];

      for(int i=0; i<8; i++)
      {
        int shift = (8-i) * 2;
        result[i] = hexChars[(int)((value>>shift) & 0xF)];
      }
      
      return new string(result);
    }

    static string UrlSafeDecode(string encodedFile)
    {
      string unencodedFile = Uri.UnescapeDataString(encodedFile);

      for(int i=0; i<unencodedFile.Length; i++)
      {
        if(Array.BinarySearch(badChars, unencodedFile[i]) >= 0)
        {
          char[] safeChars = new char[unencodedFile.Length];
          for(int j=0; j<i; j++) safeChars[j] = unencodedFile[j];
          for(; i<safeChars.Length; i++)
          {
            safeChars[i] = Array.BinarySearch(badChars, unencodedFile[i]) >= 0 ? '_' : unencodedFile[i];
          }
          unencodedFile = new string(safeChars);
          break;
        }
      }
      
      return unencodedFile;
    }

    readonly Crawler crawler;
    readonly Uri baseUri;
    readonly Queue<Resource> resources = new Queue<Resource>();
    readonly Dictionary<string,LocalFileInfo> files = new Dictionary<string,LocalFileInfo>();
    readonly List<string> queued = new List<string>();
    CookieContainer cookies;
    string baseDir;
    internal int connections;
    readonly bool external;

    static readonly Regex queryRe = new Regex(@"^\?(?<key>[\w\-/.!~*'()]+)=(?<value>[\w\-/.!~*'()]*)(?:&(?<key>[\w\-/.!~*'()]+)=(?<value>[\w\-/.!~*'()]*))*&?$",
                                              RegexOptions.Compiled | RegexOptions.ECMAScript);
    static readonly char[] badChars;
  }
  #endregion

  internal enum LinkType
  {
    Link, InternalResource, ExternalResource
  }

  internal Uri FilterUri(Uri uri)
  {
    if(FilterUris != null && uri != null)
    {
      try { uri = FilterUris(uri); }
      catch { }
    }
    return uri;
  }

  bool DownloadNearFiles
  {
    get { return (download & Download.NearFiles) != 0; }
  }

  static bool AreSameDomain(Uri a, Uri b)
  {
    if(a.HostNameType != b.HostNameType) return false;

    if(a.HostNameType != UriHostNameType.Dns)
    {
      return string.Equals(a.Host, b.Host, StringComparison.Ordinal);
    }
    else
    {
      Match aMatch = domainRe.Match(a.Host), bMatch = domainRe.Match(b.Host);
      return aMatch.Success && bMatch.Success &&
             string.Equals(aMatch.Value, bMatch.Value, StringComparison.OrdinalIgnoreCase);
    }
  }

  bool AreSameHostName(Uri a, Uri b)
  {
    if(a.HostNameType != b.HostNameType) return false;
    if(string.Equals(a.Host, b.Host, StringComparison.OrdinalIgnoreCase)) return true;

    if(EnableUrlHacks && a.HostNameType == UriHostNameType.Dns)
    {
      // assume that foo.com == www.foo.com
      bool aWWW = a.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase);
      bool bWWW = b.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase);
      if(aWWW && !bWWW && string.Equals(a.Host, "www."+b.Host, StringComparison.OrdinalIgnoreCase) ||
         !aWWW && bWWW && string.Equals("www."+a.Host, b.Host, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
    }
    
    return false;
  }

  static bool AreSameTLD(Uri a, Uri b)
  {
    if(a.HostNameType != b.HostNameType) return false;

    if(a.HostNameType != UriHostNameType.Dns)
    {
      return string.Equals(a.Host, b.Host, StringComparison.Ordinal);
    }
    else
    {
      Match aMatch = tldRe.Match(a.Host), bMatch = tldRe.Match(b.Host);
      return aMatch.Success && bMatch.Success &&
             string.Equals(aMatch.Value, bMatch.Value, StringComparison.OrdinalIgnoreCase);
    }
  }

  void AssertInitialized()
  {
    if(!IsInitialized) throw new InvalidOperationException("The crawler has not been initialized.");
  }

  void CrawlService(Service service)
  {
    if(service == null) throw new ArgumentNullException();
    if(currentActiveThreads >= MaxConnections) return;

    lock(threads)
    {
      if(service.CurrentConnections < MaxConnectionsPerServer)
      {
        int idleThread;
        for(idleThread=0; idleThread<threads.Count; idleThread++) // find the first idle thread
        {
          if(!threads[idleThread].IsRunning) break;
        }

        do
        {
          ConnectionThread thread;
          if(idleThread == threads.Count)
          {
            thread = new ConnectionThread(this);
            threads.Add(thread);
            idleThread++; // make sure idleThread still equals threads.Count
          }
          else
          {
            thread = threads[idleThread];
            for(; idleThread<threads.Count; idleThread++) // advance to the next idle thread
            {
              if(!threads[idleThread].IsRunning) break;
            }
          }

          StartThread(thread, service);
        } while(currentActiveThreads < MaxConnections && service.CurrentConnections < MaxConnectionsPerServer &&
                service.ResourceCount > 0 && idleThread < MaxConnections);
      }
    }
  }

  bool DirectoryIsAbove(Uri uri, Uri baseUri)
  {
    // the directory is above if uri's path components are less than or equal to baseUri's path components, and all
    // of them are equal.
    string[] segments = uri.Segments, baseSegments = baseUri.Segments;

    // ignore trailing items that look like filenames, because we're only interested in directories
    int segmentsLength = segments.Length;
    if(segmentsLength != 0 && !segments[segmentsLength-1].EndsWith("/", StringComparison.Ordinal)) segmentsLength--;
    int baseLength = baseSegments.Length;
    if(baseLength != 0 && !baseSegments[baseLength-1].EndsWith("/", StringComparison.Ordinal)) baseLength--;

    if(segmentsLength <= baseLength)
    {
      for(int i=0; i<segmentsLength; i++)
      {
        if(!string.Equals(segments[i], baseSegments[i], pathComparison)) return false;
      }
      return true;
    }

    return false;
  }

  bool DirectoryIsBelow(Uri uri, Uri baseUri)
  {
    // the directory is above if uri's path components are greater than or equal to baseUri's path components, and all
    // of them up to baseUri's length are equal.
    string[] segments = uri.Segments, baseSegments = baseUri.Segments;

    // ignore trailing items that look like filenames, because we're only interested in directories
    int segmentsLength = segments.Length;
    if(segmentsLength != 0 && !segments[segmentsLength-1].EndsWith("/", StringComparison.Ordinal)) segmentsLength--;
    int baseLength = baseSegments.Length;
    if(baseLength != 0 && !baseSegments[baseLength-1].EndsWith("/", StringComparison.Ordinal)) baseLength--;

    if(segmentsLength >= baseLength)
    {
      for(int i=0; i<baseLength; i++)
      {
        if(!string.Equals(segments[i], baseSegments[i], pathComparison)) return false;
      }
      return true;
    }

    return false;
  }

  void Dispose(bool finalizing)
  {
    Deinitialize();
    disposed = true;
  }

  bool EnqueueUri(Uri uri, string referrer, int depth, LinkType type, bool force)
  {
    bool external;
    if(IsUriAllowed(uri, type != LinkType.Link, out external) &&
       (MaxDepth == 0 || depth <= MaxDepth) && (MaxQueuedLinks == 0 || CurrentLinksQueued < MaxQueuedLinks) ||
       force)
    {
      Resource resource = new Resource(uri, referrer, depth, type, external);
      Service service = GetService(uri, external);
      service.Enqueue(ref resource);

      if(running && currentActiveThreads < MaxConnections && service.connections < MaxConnectionsPerServer)
      {
        CrawlService(service);
      }

      return true;
    }
    else
    {
      return false;
    }
  }

  Service GetService(Uri uri, bool external)
  {
    Service service;
    string key = GetServiceKey(uri);
    lock(services)
    {
      if(!services.TryGetValue(key, out service))
      {
        services[key] = service = new Service(this, uri, external);
      }
    }
    return service;
  }

  static string GetServiceKey(Uri uri)
  {
    return uri.Scheme + "_" + uri.Authority.ToLowerInvariant();
  }

  string GetRelativeUri(string localBase, Uri absUri, LinkType type)
  {
    Service service;

    string serviceKey = GetServiceKey(absUri);
    lock(services) service = services[serviceKey];
    string localFileName = service.GetLocalFileName(absUri, type);
    if(localFileName == null) return null;

    char[] split = new char[1] { Path.DirectorySeparatorChar };
    string[] baseBits = localBase.Split(split, StringSplitOptions.RemoveEmptyEntries);
    string[] newBits  = localFileName.Split(split, StringSplitOptions.RemoveEmptyEntries);

    int index = 0;
    for(int len=Math.Min(baseBits.Length, newBits.Length); index<len; index++)
    {
      if(!string.Equals(baseBits[index], newBits[index], StringComparison.Ordinal)) break;
    }
    
    StringBuilder sb = new StringBuilder();
    for(int i=index; i<baseBits.Length; i++)
    {
      sb.Append("../");
    }
    for(int i=index; i<newBits.Length; i++)
    {
      if(i != index) sb.Append('/');
      sb.Append(newBits[i]);
    }

    if(!string.IsNullOrEmpty(absUri.Query)) sb.Append(absUri.Query);
    if(!string.IsNullOrEmpty(absUri.Fragment)) sb.Append(absUri.Fragment);
    return sb.ToString();
  }

  static Download GetResourceType(string mimeType)
  {
    bool isHtml = string.Equals(mimeType, "text/html",  StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "application/xhtml+xml", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "text/xhtml", StringComparison.OrdinalIgnoreCase) || // incorrect but in use
                  string.Equals(mimeType, "text/xml", StringComparison.OrdinalIgnoreCase); // incorrect but in use
    return isHtml ? Download.Html : Download.NonHtml;
  }

  Download GuessResourceType(Uri uri)
  {
    string[] segments = uri.Segments;
    if(segments.Length == 0) return Download.Unknown;
    return GuessResourceType(Path.GetExtension(segments[segments.Length-1]));
  }
  
  Download GuessResourceType(string extension)
  {
    string mimeType;

    if(string.IsNullOrEmpty(extension))
    {
      mimeType = null;
    }
    else
    {
      lock(mimeOverrides) mimeOverrides.TryGetValue(extension.ToLowerInvariant(), out mimeType);
    }

    return mimeType == null ? Download.Unknown : GetResourceType(mimeType);
  }

  void IdleThread(ConnectionThread thread)
  {
    thread.Stop();
  }

  bool IsUriAllowed(Uri uri, bool ignoreUriRestrictions, out bool isExternal)
  {
    isExternal = false;

    if(!string.Equals(uri.Scheme, "http", StringComparison.Ordinal) && // only accept these three protocols
       !string.Equals(uri.Scheme, "https", StringComparison.Ordinal) &&
       !string.Equals(uri.Scheme, "ftp", StringComparison.Ordinal))
    {
      return false;
    }

    Download resourceType = GuessResourceType(uri); // skip non-html files if we aren't downloading them
    if(resourceType == Download.NonHtml && (Download & Download.NonHtml) == 0)
    {
      return false;
    }

    if(domainNav == DomainNavigation.Everywhere && dirNav == DirectoryNavigation.UpAndDown)
    {
      return true;
    }

    lock(baseUris)
    {
      foreach(Uri baseUri in baseUris)
      {
        // first check if the directory navigation is allowed (if we care about that)
        if(DirectoryNavigation != DirectoryNavigation.UpAndDown)
        {
          if(AreSameHostName(uri, baseUri))
          {
            if(dirNav == DirectoryNavigation.Down && !DirectoryIsBelow(uri, baseUri) ||
               dirNav == DirectoryNavigation.Up   && !DirectoryIsAbove(uri, baseUri))
            {
              continue;
            }
          }
          else if(domainNav == DomainNavigation.SameHostName)
          {
            continue; // while we're here, we can use our knowledge of the host name to speed up the SameHostName check
          }
        }

        bool domainMatch =
          domainNav == DomainNavigation.Everywhere ||
          domainNav == DomainNavigation.SameHostName && AreSameHostName(uri, baseUri) ||
          domainNav == DomainNavigation.SameDomain   && AreSameDomain(uri, baseUri) ||
          domainNav == DomainNavigation.SameTLD      && AreSameTLD(uri, baseUri);

        if(domainMatch) return true;
      }
    }
    
    isExternal = true; // if we got here, it's an external link (outside all base uris)

    // if it's external but it's a resource of an internal file, then it's OK
    return ignoreUriRestrictions && DownloadNearFiles;
  }

  void OnErrorOccurred(ref Resource resource, Exception ex, bool isFatal)
  {
    OnProgress(ref resource, isFatal ? ProgressType.FatalErrorOccurred : ProgressType.NonFatalErrorOccurred);
  }

  void OnProgress(ref Resource resource, ProgressType newStatus)
  {
    resource.status = newStatus;
    if(Progress != null && (ProgressFilter & newStatus) != 0)
    {
      try { Progress(resource); }
      catch { }
    }
  }

  void OnThreadIdle(ConnectionThread thread)
  {
    if(!running) // if we're stopped, just idle the thread
    {
      IdleThread(thread);
    }
    else // otherwise find another service that we can associate with this thread
    {
      lock(services)
      {
        foreach(Service service in services.Values)
        {
          if(service != thread.Service && !service.IsQueueEmpty &&
             service.CurrentConnections < MaxConnectionsPerServer)
          {
            StartThread(thread, service);
            return;
          }
        }

        IdleThread(thread); // if we couldn't find a service to run, idle the thread
      }
    }
  }

  void SetServicePointProperties(ServicePoint point)
  {
    point.MaxIdleTime     = ConnectionIdleTimeout * 1000;
    point.ConnectionLimit = MaxConnectionsPerServer;
  }

  void StartThread(ConnectionThread thread, Service service)
  {
    thread.Start(service);
  }

  bool WantResource(Download resourceType)
  {
    Debug.Assert(resourceType == Download.Html || resourceType == Download.NonHtml);
    return (download & resourceType) != 0;
  }

  Dictionary<string,string> mimeOverrides = new Dictionary<string,string>();
  Dictionary<string,Service> services = new Dictionary<string,Service>();
  List<Uri> baseUris = new List<Uri>();
  List<ConnectionThread> threads = new List<ConnectionThread>();
  string userAgent = "Mozilla/4.5 (compatible; AdamMil 1.0; Windows XP)", language, defaultReferrer, baseDir;
  DirectoryNavigation dirNav = DirectoryNavigation.Down;
  DomainNavigation domainNav = DomainNavigation.SameHostName;
  Download download = Download.Everything | Download.PrioritizeHtml;
  StringComparison pathComparison = StringComparison.OrdinalIgnoreCase;
  ProgressType progress = ProgressType.All;
  int idleTimeout = 30, connsPerServer = 2, maxConnections = 10, maxDepth = 50, retries = 1, maxQueuedLinks,
      ioTimeout = 60, transferTimeout = 1800, maxRedirects = 20, currentActiveThreads, maxQueryStrings = 500;
  bool rewriteLinks = true, useCookies = true, errorFiles, urlHacks, passiveFtp = true, disposed, running;

  static Regex domainRe = new Regex(@"[\w-]+\.[\w]+$", RegexOptions.Compiled | RegexOptions.Singleline);
  static Regex tldRe = new Regex(@"(?<=\.)[\w]+$", RegexOptions.Compiled | RegexOptions.Singleline);
}
#endregion

} // namespace WebCrawl.Backend