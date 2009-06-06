using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using AdamMil.Collections;
using BinaryReader=AdamMil.IO.BinaryReader;
using BinaryWriter=AdamMil.IO.BinaryWriter;

namespace WebCrawl.Backend
{

#region Enums
/// <summary>An enumeration used to control where in a site's directory structure the crawler is allowed to visit.</summary>
public enum DirectoryNavigation
{
  /// <summary>The crawler can only visit within the same directory as the base URLs.</summary>
  Same,
  /// <summary>The crawler can visit directories in the base URLs and descendants of those directories.</summary>
  Down,
  /// <summary>The crawler can visit directories in the base URLs and ancestors of those directories.</summary>
  Up,
  /// <summary>The crawler can visit any directory on the site.</summary>
  UpAndDown
}

/// <summary>An enumeration used to control which domains the crawler is allowed to visit.</summary>
public enum DomainNavigation
{
  /// <summary>The crawler is allowed to visit only URLs with the exact same hostname as the base URLs. For instance,
  /// if a base URL http://www.foo.com was added, the crawler could not visit http://sub.foo.com or even
  /// http://foo.com. Note that URL filters can be used to normalize "www.foo.com" and "foo.com", allowing the crawler
  /// to travel between them as a special case. <see cref="UrlFilters.StripWWWPrefix"/> can be used for this purpose.
  /// </summary>
  SameHostName,
  /// <summary>The crawler is allowed to visit any subdomain of the domains added in the base URLs. For instance, if
  /// a base URL http://www.foo.com was added, the crawler could visit http://sub.foo.com but not http://www.bar.com.
  /// </summary>
  SameDomain,
  /// <summary>The crawler is allowed to visit any site with the same top-level domain (eg, .com, .net, or .edu). For
  /// instance, if a base URL http://www.foo.com was added, the crawler could visit http://www.bar.com, but not
  /// http://www.baz.net.
  /// </summary>
  SameTLD,
  /// <summary>The crawler is allowed to visit any domain.</summary>
  Everywhere
}

/// <summary>An enumeration that describes which types of files will be downloaded. These values can be ORed together
/// to allow multiple types of resources to be downloaded.
/// </summary>
[Flags]
public enum DownloadFlags
{
  /// <summary>A value used internally to indicate that the type of a file is unknown.</summary>
  Unknown=0,
  /// <summary>Web pages and necessary supporting files like CSS and javascript will be downloaded.</summary>
  Html=1,
  /// <summary>Non-web resources, such as images, movies, etc. will be downloaded.</summary>
  NonHtml=2,
  /// <summary>A mask that can be applied to extract the types of files to be downloaded.</summary>
  TypeMask=Everything, // this goes before Everything so that ResourceType.ToString() prints "Everything" rather than "TypeMask"
  /// <summary>All resources will be downloaded.</summary>
  Everything=Html|NonHtml,
  /// <summary>Web pages will be downloaded before other resources.</summary>
  PrioritizeHtml=4,
  /// <summary>Non-web resources will be downloaded before web pages.</summary>
  PrioritizeNonHtml=8,
  /// <summary>A mask that can be applied to extract the types of files that will be prioritized.</summary>
  PriorityMask=12,
  /// <summary>Supporting non-web resources will be downloaded, even if the crawler is not normally allowed to visit
  /// them (eg, if they're on another domain and the crawler is limited to the current domain).
  /// </summary>
  ExternalResources=16,
}

/// <summary>An enumeration used to indicate the current status of a <see cref="Resource"/>.</summary>
public enum Status
{
  /// <summary>No progress has yet been made. This value is only used to indicate the status of a
  /// <see cref="Resource"/> and is not reported to a progress handler.
  /// </summary>
  None,
  /// <summary>The given resource was added to the download queue.</summary>
  UrlQueued,
  /// <summary>The given resource has begun downloading.</summary>
  DownloadStarted,
  /// <summary>The given resource has finished downloading.</summary>
  DownloadFinished,
  /// <summary>A non-fatal error has occurred, meaning that a resource failed to download successfully, but the crawler
  /// will try again.
  /// </summary>
  NonFatalErrorOccurred,
  /// <summary>A fatal error occurred, meaning that a resource failed to download successfully, and the crawler will
  /// not try again.
  /// </summary>
  FatalErrorOccurred
}

/// <summary>An enumeration used to indicate the type of content contained within a file.</summary>
public enum ResourceType
{
  /// <summary>The contents of the file are unknown.</summary>
  Unknown,
  /// <summary>The file contains HTML.</summary>
  Html,
  /// <summary>The file contains CSS.</summary>
  Css,
  /// <summary>The file contains Javascript.</summary>
  Javascript,
  /// <summary>The file contains VBScript.</summary>
  VBScript,
  /// <summary>The file is treated as an opaque binary. It is assumed, for now, that all non-html types are lumped into
  /// Binary.
  /// </summary>
  Binary
}
#endregion

#region Delegates
/// <summary>Allows the content of a resource to be filtered before it is processed by the crawler.</summary>
/// <param name="url">The URL of the resource.</param>
/// <param name="mimeType">The MIME type of the resource. Note that this is not completely reliable, since it may be
/// misreported by the web server.
/// </param>
/// <param name="resourceType">The resource type of the file, one of the <see cref="ResourceType"/> values.
/// Note that this is not completely reliable, since it's based in part on the MIME type, which may be
/// misreported by the server, and/or the file extension, which may be misleading.
/// </param>
/// <param name="contentFileName">The path to a file on disk containing the content. If the content is to be changed,
/// the new content should be written to this file.
/// </param>
/// <param name="encoding">The encoding reported by the server for an HTML content type, or null for other content
/// types or if the encoding could not be determined.
/// </param>
/// <remarks>This method is called to filter a resource that has been downloaded, but not yet processed. The content of
/// the resource is stored in the file given by <paramref name="contentFileName"/>, and if the content is changed, the
/// new content should be written to that same file.
/// </remarks>
public delegate void ContentFilter(Uri url, string mimeType, ResourceType resourceType, Encoding encoding,
                                   string contentFileName);

/// <summary>A handler that is called at various stages in the downloading and processing of a resource.</summary>
/// <param name="resource">The <see cref="Resource"/> that to which the progress update pertains. The current status
/// of the resource, corresponding to the reason for the progress update, is stored in <see cref="Resource.Status"/>.
/// </param>
/// <param name="extraMessage">Extra text related to the progress update. For instance, if an exception occurs while
/// downloading the resource, this may be the exception text.
/// </param>
public delegate void ProgressHandler(Resource resource, string extraMessage);

/// <summary>A delegate allowing URLs to be filtered after they have been retrieved from a web page but before they are
/// enqueued or otherwise processed by the crawler.
/// </summary>
/// <param name="uri">A URI found within a web page, in absolute form.</param>
/// <returns>The absolute URI to use for the resource, or null if the URI is to be ignored.</returns>
public delegate Uri UriFilter(Uri uri);
#endregion

#region Download
/// <summary>A structure representing a snapshot of a thread's current download.</summary>
public struct Download
{
  public Download(Resource resource, int speed)
  {
    Resource     = resource;
    CurrentSpeed = speed;
  }

  /// <summary>The resource being downloaded.</summary>
  public readonly Resource Resource;
  /// <summary>The speed at which the resource is being downloaded.</summary>
  public readonly int CurrentSpeed;
}
#endregion

#region MimeOverride
/// <summary>A structure that allows the MIME type of a resource to be assumed based on its file extension.</summary>
public struct MimeOverride
{
  /// <summary>Initializes a new MIME type override for a given file extension.</summary>
  /// <param name="extension">The file extension whose MIME type will be overriden. This can be empty, but cannot be
  /// null, and should not contain the leading period.
  /// </param>
  /// <param name="mimeType">The MIME type to use for resources with the given file extension.</param>
  /// <param name="isPreferred">Whether this is the preferred extension for the given MIME type.</param>
  public MimeOverride(string extension, string mimeType, bool isPreferred)
  {
    if(extension == null || string.IsNullOrEmpty(mimeType))
    {
      throw new ArgumentException("Extension cannot be null, and mime type cannot be empty.");
    }
    if(extension.StartsWith(".")) extension = extension.Substring(1);
    Extension   = extension;
    MimeType    = mimeType;
    IsPreferred = isPreferred;
  }

  /// <summary>The file extension.</summary>
  public string Extension;
  /// <summary>The MIME type for resources matching <see cref="Extension"/>.</summary>
  public string MimeType;
  /// <summary>Whether this is the preferred extension for <see cref="MimeType"/>.</summary>
  public bool IsPreferred;
}
#endregion

#region Resource
/// <summary>A structure representing an internet resource.</summary>
public sealed class Resource
{
  internal Resource(Uri uri, string postData, Uri referrer, int depth, Crawler.LinkType type)
  {
    this.uri      = uri;
    this.postData = postData;
    this.referrer = referrer;
    this.depth    = depth;
    this.type     = type;
    
    responseCode = HttpStatusCode.Unused;
  }

  /// <summary>Gets the URI of the resource that linked to this resource, or null if this is a root URI.</summary>
  public Uri Referrer
  {
    get { return referrer; }
  }

  /// <summary>Gets the local file name containing the downloaded resource, or null if no local file name has been
  /// allocated yet.
  /// </summary>
  public string LocalPath
  {
    get { return localPath; }
  }

  /// <summary>Gets the response text from the server when the resource was downloaded, or null if the resource has not
  /// been downloaded yet.
  /// </summary>
  public string ResponseText
  {
    get { return responseText; }
  }

  /// <summary>Gets the MIME type of the resource, as reported by the web server, or null if the resource has not been
  /// downloaded yet.
  /// </summary>
  public string ContentType
  {
    get { return contentType; }
  }

  /// <summary>Gets the data to post to the URI, in application/x-www-form-urlencoded format, or null if no data is to
  /// be posted.
  /// </summary>
  public string PostData
  {
    get { return postData; }
  }

  /// <summary>Gets the absolute URI of the resource.</summary>
  public Uri Uri
  {
    get { return uri; }
  }

  /// <summary>Gets the HTTP status code returned by the web server.</summary>
  public HttpStatusCode ResponseCode
  {
    get { return responseCode; }
  }

  /// <summary>Gets the current status of the resource.</summary>
  public Status Status
  {
    get { return status; }
  }

  /// <summary>Gets the link depth of the resource, which is the number of links that were traversed before the link to
  /// the resource was found.
  /// </summary>
  public int Depth
  {
    get { return depth; }
  }
  
  /// <summary>Gets the number of times that the download of the resource has failed.</summary>
  public int Failures
  {
    get { return failures; }
  }

  /// <summary>Gets the number of bytes downloaded so far.</summary>
  public long BytesDownloaded
  {
    get { return bytesDownloaded; }
  }

  /// <summary>Gets the total size of the resource, or -1 if the size is not known.</summary>
  public long TotalSize
  {
    get { return size; }
  }

  /// <summary>Gets whether the resource contains a valid URI.</summary>
  internal bool IsValid
  {
    get { return Uri != null && Uri.IsAbsoluteUri; }
  }

  internal string PlaceholderUrl
  {
    get { return "amwcRewrite://" + rewriteTag.ToString(CultureInfo.InvariantCulture) + "/"; }
  }

  internal void AddReference(Resource reference)
  {
    if(references == null) references = new List<Resource>();
    int index = references.BinarySearch(reference, ResourceComparer.Instance);
    if(index < 0) references.Insert(~index, reference);
  }

  internal long size=-1, bytesDownloaded;
  internal string localPath, responseText, contentType;
  internal List<Resource> references;
  internal Encoding encoding;
  internal HttpStatusCode responseCode;
  internal uint rewriteTag;
  internal int depth, failures;
  internal Status status;
  internal Crawler.RewriteStatus rewriteStatus;
  internal Crawler.LinkType type;

  Uri uri, referrer;
  string postData;
}
#endregion

#region ResourceComparer
/// <summary>Compares resources by URL, ignoring any URL fragment.</summary>
sealed class ResourceComparer : IComparer<Resource>
{
  ResourceComparer() { }

  public int Compare(Resource a, Resource b)
  {
    return string.CompareOrdinal(a.Uri.GetLeftPart(UriPartial.Query), b.Uri.GetLeftPart(UriPartial.Query));
  }

  public static readonly ResourceComparer Instance = new ResourceComparer();
}
#endregion

#region Crawler
/// <summary>A class that implements an internet web crawler.</summary>
public sealed class Crawler : IDisposable
{
  /// <summary>A value that can be used for <see cref="DepthLimit"/> and other properties to indicate that there is no
  /// limit.
  /// </summary>
  public const int Infinite = -1;

  /// <summary>An integer used to identify the version of the settings file.</summary>
  const int SettingsVersion = 2;

  ~Crawler()
  {
    Dispose(true);
  }

  /// <summary>An event that allows a <see cref="ContentFilter"/> to be attached, to filter the downloaded content.</summary>
  public event ContentFilter FilterContent;
  /// <summary>An event that allows a <see cref="UriFilter"/> to be attached, to filter links found by the crawler.</summary>
  public event UriFilter FilterUris;
  /// <summary>An event that allows a <see cref="ProgressHandler"/> to be attached, to recieve progress updates.</summary>
  public event ProgressHandler Progress;
  
  /// <summary>Gets or sets the base directory, into which the downloaded data will be saved.</summary>
  /// <remarks>This property is only valid after the crawler has been initialized.</remarks>
  public string BaseDirectory
  {
    get { return baseDir; }
    set
    {
      AssertNotRunningForSetter();
      string newValue = value == null ?
        null : value.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      if(newValue != baseDir)
      {
        baseDir = newValue;
        baseDirInitialized = false;
      }
    }
  }

  /// <summary>Gets or sets whether URIs will be treated case sensitively.</summary>
  /// <remarks>The default is true.</remarks>
  public bool CaseSensitivePaths
  {
    get { return caseSensitive; }
    set { caseSensitive = value; }
  }

  /// <summary>Gets or sets the number of seconds without activity before a persistent connection with the server is
  /// closed.
  /// </summary>
  /// <remarks>The default is 30 seconds.</remarks>
  public int ConnectionIdleTimeout
  {
    get { return idleTimeout; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException("The idle timeout must be positive.");
      idleTimeout = value;
    }
  }

  /// <summary>Gets an approximation of the current download speed, in bytes per second.</summary>
  public int CurrentBytesPerSecond
  {
    get
    {
      int bytesPerSecond = 0;
      lock(threads)
      {
        foreach(ConnectionThread thread in threads) bytesPerSecond += thread.CurrentBytesPerSecond;
      }
      return bytesPerSecond;
    }
  }

  /// <summary>Gets the number of currently active downloads.</summary>
  public int CurrentDownloadCount
  {
    get { return currentActiveThreads; }
  }

  /// <summary>Gets the number of queued links.</summary>
  public int CurrentLinksQueued
  {
    get
    {
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

  /// <summary>Gets or sets the default referrer, which will be sent to the server for URLs that are enqueued directly,
  /// rather than being found via a link from a web page. If set to null, no referrer will be sent for root URLs.
  /// </summary>
  /// <remarks>The default value is null.</remarks>
  public Uri DefaultReferrer
  {
    get { return defaultReferrer; }
    set { defaultReferrer = value; }
  }

  /// <summary>Gets or sets which paths the crawler can visit, relative to the base URIs that have been added.</summary>
  /// <remarks>The default value is <see cref="Backend.DirectoryNavigation.Down"/>.</remarks>
  public DirectoryNavigation DirectoryNavigation
  {
    get { return dirNav; }
    set { dirNav = value; }
  }

  /// <summary>Gets or sets which hosts the crawler can visit, relative to the base URIs that have been added.</summary>
  /// <remarks>The default value is <see cref="Backend.DomainNavigation.SameHostName"/>.</remarks>
  public DomainNavigation DomainNavigation
  {
    get { return domainNav; }
    set { domainNav = value; }
  }

  /// <summary>Gets or sets a value that controls which types of resources should be downloaded, and how they should
  /// be prioritized.
  /// </summary>
  /// <remarks>The default is <see cref="Backend.DownloadFlags.Everything"/> |
  /// <see cref="Backend.DownloadFlags.ExternalResources"/>.
  /// </remarks>
  public DownloadFlags Download
  {
    get { return download; }
    set { download = value; }
  }

  /// <summary>Gets or sets whether web pages that failed to download will be replaced by web pages describing the
  /// error that occurred during download. The default is true.
  /// </summary>
  public bool GenerateErrorFiles
  {
    get { return errorFiles; }
    set { errorFiles = value; }
  }

  /// <summary>Gets whether all queued resources have finished downloading.</summary>
  public bool IsDone
  {
    get { return CurrentDownloadCount == 0 && CurrentLinksQueued == 0; }
  }

  /// <summary>Gets whether the crawler is running. This will return false if the crawler is stopping, but not yet
  /// stopped.
  /// </summary>
  public bool IsRunning
  {
    get { return running; }
  }

  /// <summary>Gets whether the crawler is completely stopped.</summary>
  public bool IsStopped
  {
    get { return !running && !HasWorkToDo; }
  }

  /// <summary>Gets whether the crawler is stopping, but is not yet stopped.</summary>
  public bool IsStopping
  {
    get { return !running && HasWorkToDo; }
  }

  /// <summary>Gets or sets the number of connections allowed to a single host.</summary>
  /// <remarks>Note that foo.com and bar.com are considered two different hosts, even if they reside on the same
  /// server. The default value is 2.
  /// </remarks>
  public int MaxConnectionsPerServer
  {
    get { return connsPerServer; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException("The connection limit must be positive.");
      bool increased = value > connsPerServer;
      connsPerServer = value;
      // if the connections were increased and the crawler is running, allocate the new connections
      if(running && increased && currentActiveThreads < MaxConnections) CrawlServices();
    }
  }
  
  /// <summary>Gets or sets the maximum number of concurrent connections.</summary>
  public int MaxConnections
  {
    get { return maxConnections; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException("The connection limit must be positive.");
      bool increased = value > maxConnections;
      maxConnections = value;
      // if the connections were increased and the crawler is running, allocate the new connections
      if(running && increased) CrawlServices();
    }
  }

  /// <summary>Gets or sets the depth limit for resources being downloaded. If set to <see cref="Infinite"/>, no
  /// limit will be used.
  /// </summary>
  /// <remarks>The depth of a resource is the number of links followed from a root URI to reach that resource.
  /// Resources with a depth greater than or equal to the depth limit will not be downloaded. The default value is 50.
  /// </remarks>
  public int DepthLimit
  {
    get { return depthLimit; }
    set
    {
      if(value != Infinite && value <= 0)
      {
        throw new ArgumentOutOfRangeException("The maximum depth must be either positive or Infinite.");
      }
      depthLimit = value;
    }
  }

  /// <summary>Gets or sets the maximum size of a resource that can be downloaded, or <see cref="Infinite"/> to specify
  /// that there is no limit.
  /// </summary>
  /// <remarks>Resources greater than the maximum size will be truncated. The default value is 50 megabytes.</remarks>
  public long MaxFileSize
  {
    get { return maxSize; }
    set
    {
      if(value != Infinite && value <= 0)
      {
        throw new ArgumentOutOfRangeException("The maximum file size must be either positive or Infinite.");
      }
      maxSize = value;
    }
  }

  /// <summary>Gets or sets the maximum number of links that can be queued at any given time, or <see cref="Infinite"/>
  /// to specify that there is no limit.
  /// </summary>
  /// <remarks>Links found when the queue is full will be ignored. The default value is <see cref="Infinite"/>.</remarks>
  public int MaxQueuedLinks
  {
    get { return maxQueuedLinks; }
    set
    {
      if(value != Infinite && value <= 0)
      {
        throw new ArgumentOutOfRangeException("The maximum queue size must be either positive or Infinite.");
      }
      maxQueuedLinks = value;
    }
  }

  /// <summary>Gets or sets the maximum number of distinct query strings that can be encountered for a given URI path
  /// before future references to that path will be ignored. This can be set to <see cref="Infinite"/> to specify that
  /// an unlimited number of query strings are allowed.
  /// </summary>
  /// <remarks>Links found that exceed the per-file query string limit will be ignored. The default value is 500.</remarks>
  public int MaxQueryStringsPerFile
  {
    get { return maxQueryStrings; }
    set
    {
      if(value != Infinite && value <= 0)
      {
        throw new ArgumentOutOfRangeException("The maximum query strings must be either positive or Infinite.");
      }
      maxQueryStrings = value;
    }
  }

  /// <summary>Gets or sets the maximum number of redirects that are allowed before the crawler gives up on downloading
  /// a resource.
  /// </summary>
  /// <remarks>The default value is 20.</remarks>
  public int MaxRedirects
  {
    get { return maxRedirects; }
    set
    {
      if(value <= 0) throw new ArgumentOutOfRangeException("The maximum number of redirects must be positive.");
      maxRedirects = value;
    }
  }

  /// <summary>Gets or sets the maximum number of retries for resources that failed to download, or
  /// <see cref="Infinite"/> to specify an unlimited number of retries (not recommended).
  /// </summary>
  /// <remarks>Resources that fail to download due to a non-fatal error (such as a 500 error or an aborted connection)
  /// will be placed back onto the queue to be tried again later. If the download fails too many times, or fails due to
  /// a fatal error (such as a 404 error code) the link will be discarded. This can be set to zero to disable retries.
  /// The default value is 1.
  /// </remarks>
  public int MaxRetries
  {
    get { return retries; }
    set
    {
      if(value != Infinite && value < 0)
      {
        throw new ArgumentOutOfRangeException("The maximum number of retries must be either "+
                                              "non-negative or Infinite.");
      }
      retries = value;
    }
  }

  /// <summary>Gets or sets whether FTP connects will be made using passive mode.</summary>
  /// <remarks>Passive FTP allows FTP connections to work through most firewalls. The default is true.</remarks>
  public bool PassiveFtp
  {
    get { return passiveFtp; }
    set { passiveFtp = value; }
  }

  /// <summary>Gets or sets the preferred language, in RFC 1766 format, or null to not specify any preferred language.</summary>
  /// <remarks>This property sets the Accept-Language HTTP header, and may affect the language of the content returned
  /// by the web server.
  /// </remarks>
  public string PreferredLanguage
  {
    get { return language; }
    set { language = value; }
  }

  /// <summary>Gets or sets the amount of time, in seconds, that the crawler will wait without receiving any data
  /// during a download attempt before closing the connection. Setting this to <see cref="Infinite"/> will specify no
  /// timeout (not recommended).
  /// </summary>
  /// <remarks>This timeout affects periods where the download stalls. The default value is 60 seconds.</remarks>
  public int ReadTimeout
  {
    get { return ioTimeout; }
    set
    {
      if(value != Infinite && value < 0)
      {
        throw new ArgumentOutOfRangeException("The read timeout must be either non-negative or Infinite.");
      }
      ioTimeout = value;
    }
  }

  /// <summary>Gets or sets whether links will be rewritten to reference the files on the local machine.</summary>
  /// <remarks>If link rewriting is disabled, all links will be left unchanged. This may cause some links to break,
  /// or to be pointing to the original web server. If enabled, links will be rewritten to reference the locally
  /// downloaded resource files. The default value is true.
  /// </remarks>
  public bool RewriteLinks
  {
    get { return rewriteLinks; }
    set
    {
      if(value && running) InitializeRewriteThread();
      rewriteLinks = value;
    }
  }

  /// <summary>Gets the total number of resources downloaded since the crawler was last initialized.</summary>
  public int TotalResourcesDownloaded
  {
    get { return resourcesDownloaded; }
  }

  /// <summary>Gets the total number of bytes downloaded since the crawler was last initialized.</summary>
  public long TotalBytesDownloaded
  {
    get
    {
      long bytes = bytesDownloaded;
      lock(threads)
      {
        foreach(ConnectionThread thread in threads)
        {
          Resource resource = thread.CurrentResource;
          if(resource != null) bytes += resource.BytesDownloaded;
        }
      }
      return bytes;
    }
  }

  /// <summary>Gets or sets the amount of time, in seconds, that the crawler will wait for a download to complete
  /// before closing the connection. Setting this to <see cref="Infinite"/> will specify no timeout.
  /// </summary>
  /// <remarks>This timeout will cause the abortion of downloads that take a long time, even if they are not stalled.
  /// The default value is 5 minutes.
  /// </remarks>
  public int TransferTimeout
  {
    get { return transferTimeout; }
    set
    {
      if(value != Infinite && value < 0)
      {
        throw new ArgumentOutOfRangeException("The transfer timeout must be either non-negative or Infinite.");
      }
      transferTimeout = value;
    }
  }

  /// <summary>Gets or sets whether cookies will be accepted from and passed back to web hosts during downloads.</summary>
  /// <remarks>Enabling cookies takes some additional memory to store the cookies, but is necessary to successfully
  /// crawl many websites. The default value is true.
  /// </remarks>
  public bool UseCookies
  {
    get { return useCookies; }
    set { useCookies = value; }
  }

  /// <summary>Gets or sets the user agent string that is reported to the web server, or null to not send a user agent
  /// string.
  /// </summary>
  /// <remarks>The default value is "Mozilla/4.5 (compatible; AdamMil 1.0; Windows XP)".</remarks>
  public string UserAgent
  {
    get { return userAgent; }
    set { userAgent = value; }
  }

  /// <summary>Terminates all downloads and deinitializes the crawler.</summary>
  /// <param name="timeToWait">The amount of time to wait for current downloads to complete, in milliseconds, or
  /// <see cref="Infinite"/> to wait for as long as it takes.
  /// </param>
  public void Deinitialize(int timeToWait)
  {
    Terminate(timeToWait); // stop all downloads and threads
    services.Clear();      // remove all services
    resourcesToRewrite.Clear(); // remove all resources from the rewrite queue
    rewriteTag = 0;        // reset the rewrite tag
    bytesDownloaded = resourcesDownloaded = 0; // reset accounting variables
    baseDirInitialized = false;
  }

  /// <summary>Deinitializes and disposes the crawler.</summary>
  public void Dispose()
  {
    GC.SuppressFinalize(this);
    Dispose(false);
  }

  /// <summary>Adds a base URI to the crawler.</summary>
  /// <param name="uri">The URI to add.</param>
  public void AddBaseUri(Uri uri)
  {
    AddBaseUri(uri, true, false);
  }

  /// <summary>Adds a base URI to the crawler.</summary>
  /// <param name="uri">The URI to add.</param>
  /// <param name="enqueue">Whether the URI should also be added to the download queue. The URI will not be enqueued
  /// if it has been enqueued previously, however.
  /// </param>
  public void AddBaseUri(Uri uri, bool enqueue)
  {
    AddBaseUri(uri, enqueue, false);
  }

  /// <summary>Adds a base URI to the crawler.</summary>
  /// <param name="uri">The URI to add.</param>
  /// <param name="enqueue">Whether the URI should also be added to the download queue.</param>
  /// <param name="force">Whether to enqueue the URI even if it's already been enqueued.</param>
  /// <remarks>The base URIs added to the crawler, together with the <see cref="DirectoryNavigation"/>, 
  /// <see cref="DomainNavigation"/>, and <see cref="Download"/> properties, determine which links and resources the
  /// crawler is allowed to visit and download.
  /// </remarks>
  public void AddBaseUri(Uri uri, bool enqueue, bool force)
  {
    ValidateNewUri(uri, null);
    lock(baseUris) baseUris.Add(uri);
    if(enqueue) EnqueueUri(uri, null, null, 0, LinkType.Link, force);
  }

  /// <summary>Removes all base URIs from the crawler.</summary>
  public void ClearBaseUris()
  {
    baseUris.Clear();
  }

  /// <summary>Removes all enqueued URIs from the crawler.</summary>
  public void ClearUris(bool allowReEnqueue)
  {
    lock(services)
    {
      foreach(Service service in services.Values) service.Clear(allowReEnqueue);
    }
  }

  /// <summary>Returns a list of the base URIs added to the crawler.</summary>
  public Uri[] GetBaseUris()
  {
    return baseUris.ToArray();
  }

  /// <summary>Returns a list of the resources that are currently being downloaded.</summary>
  public Download[] GetCurrentDownloads()
  {
    List<Download> resources = new List<Download>(currentActiveThreads);
    lock(threads)
    {
      foreach(ConnectionThread thread in threads)
      {
        Resource resource = thread.CurrentResource;
        if(resource != null) resources.Add(new Download(resource, thread.CurrentBytesPerSecond));
      }
    }
    return resources.ToArray();
  }

  /// <summary>Removes all enqueued URIs matching the given regular expression.</summary>
  /// <param name="uriFilter">The regular expression to match against all enqueued URIs.</param>
  /// <param name="allowRequeue">Whether the URIs that are removed will be allowed to be readded later.</param>
  public void RemoveUris(Regex uriFilter, bool allowRequeue)
  {
    lock(services)
    {
      foreach(Service service in services.Values) service.Remove(uriFilter, allowRequeue);
    }
  }

  /// <summary>Loads the crawler settings from the given <see cref="BinaryReader"/>.</summary>
  public void LoadSettings(BinaryReader reader)
  {
    if(!IsStopped) throw new InvalidOperationException("Settings cannot be loaded while the crawler is running.");

    int version = reader.ReadInt32();
    if(version < 1 || version > SettingsVersion)
    {
      throw new ArgumentException("The version file is not supported by this version of the software.");
    }

    BaseDirectory     = reader.ReadStringWithLength();
    string str = reader.ReadStringWithLength();
    DefaultReferrer   = str == null ? null : new Uri(str);
    PreferredLanguage = reader.ReadStringWithLength();
    UserAgent         = reader.ReadStringWithLength();

    CaseSensitivePaths      = reader.ReadBool();
    ConnectionIdleTimeout   = reader.ReadInt32();
    DepthLimit              = reader.ReadInt32();
    DirectoryNavigation     = (DirectoryNavigation)reader.ReadInt32();
    DomainNavigation        = (DomainNavigation)reader.ReadInt32();
    Download                = (DownloadFlags)reader.ReadInt32();
    GenerateErrorFiles      = reader.ReadBool();
    MaxConnections          = reader.ReadInt32();
    MaxConnectionsPerServer = reader.ReadInt32();
    MaxFileSize             = reader.ReadInt64();
    MaxQueuedLinks          = reader.ReadInt32();
    MaxQueryStringsPerFile  = reader.ReadInt32();
    MaxRedirects            = reader.ReadInt32();
    MaxRetries              = reader.ReadInt32();
    PassiveFtp              = reader.ReadBool();
    ReadTimeout             = reader.ReadInt32();
    RewriteLinks            = reader.ReadBool();
    TransferTimeout         = reader.ReadInt32();
    UseCookies              = reader.ReadBool();

    lock(extensionToMime)
    {
      ClearMimeOverrides();
      int count = reader.ReadInt32();
      while(count-- > 0)
      {
        AddMimeOverride(new MimeOverride(reader.ReadStringWithLength(), reader.ReadStringWithLength(),
                                         version < 2 || reader.ReadBool()));
      }
    }

    lock(baseUris)
    {
      baseUris.Clear();
      int count = reader.ReadInt32();
      while(count-- > 0) baseUris.Add(new Uri(reader.ReadStringWithLength()));
    }
  }

  /// <summary>Saves the current crawler settings to the given <see cref="BinaryWriter"/>.</summary>
  public void SaveSettings(BinaryWriter writer)
  {
    writer.Write(SettingsVersion);

    writer.WriteStringWithLength(BaseDirectory);
    writer.WriteStringWithLength(DefaultReferrer == null ? null : DefaultReferrer.AbsoluteUri);
    writer.WriteStringWithLength(PreferredLanguage);
    writer.WriteStringWithLength(UserAgent);

    writer.Write(CaseSensitivePaths);
    writer.Write(ConnectionIdleTimeout);
    writer.Write(DepthLimit);
    writer.Write((int)DirectoryNavigation);
    writer.Write((int)DomainNavigation);
    writer.Write((int)Download);
    writer.Write(GenerateErrorFiles);
    writer.Write(MaxConnections);
    writer.Write(MaxConnectionsPerServer);
    writer.Write(MaxFileSize);
    writer.Write(MaxQueuedLinks);
    writer.Write(MaxQueryStringsPerFile);
    writer.Write(MaxRedirects);
    writer.Write(MaxRetries);
    writer.Write(PassiveFtp);
    writer.Write(ReadTimeout);
    writer.Write(RewriteLinks);
    writer.Write(TransferTimeout);
    writer.Write(UseCookies);

    writer.Write(extensionToMime.Count);
    foreach(MimeOverride mimeOverride in GetMimeOverrides())
    {
      writer.WriteStringWithLength(mimeOverride.Extension);
      writer.WriteStringWithLength(mimeOverride.MimeType);
      writer.Write(mimeOverride.IsPreferred);
    }

    writer.Write(baseUris.Count);
    foreach(Uri uri in baseUris) writer.WriteStringWithLength(uri.AbsoluteUri);
  }

  /// <summary>Starts the crawler, so it will begin downloading resources.</summary>
  public void Start()
  {
    if(disposed) throw new ObjectDisposedException("Crawler");

    if(!baseDirInitialized)
    {
      if(string.IsNullOrEmpty(baseDir))
      {
        throw new InvalidOperationException("The BaseDirectory property has not been set yet.");
      }

      Directory.CreateDirectory(baseDir);
      baseDirInitialized = true;
    }

    terminating = false;

    if(RewriteLinks) InitializeRewriteThread();

    if(!running)
    {
      running = true;
      CrawlServices();
    }
  }

  /// <summary>Stops the crawler from beginning any new downloads, but does not terminate the current downloads.</summary>
  /// <remarks>The crawler can be resumed by calling <see cref="Start"/>.</remarks>
  public void Stop()
  {
    running = false;

    lock(threads)
    {
      foreach(ConnectionThread thread in threads) IdleThread(thread);
    }
  }

  /// <summary>Terminates all current downloads and prevents the crawler from beginning any new downloads.</summary>
  /// <param name="timeToWait">The amount of time to wait for current downloads to complete, in milliseconds, or
  /// <see cref="Infinite"/> to wait for as long as it takes.
  /// </param>
  public void Terminate(int timeToWait)
  {
    if(timeToWait != Infinite && timeToWait < 0)
    {
      throw new ArgumentOutOfRangeException("The timeout must be non-negative or Infinite.");
    }

    terminating = true;

    lock(threads)
    {
      Stop(); // first tell all threads to stop downloading

      Stopwatch timer = new Stopwatch();
      timer.Start();

      foreach(ConnectionThread thread in threads)
      {
        thread.Terminate(timeToWait);

        // if we have a timeout remaining, see how long it actually took to terminate that thread, and subtract it from
        // the time to wait.
        if(timeToWait != Infinite && timeToWait > 0)
        {
          timeToWait -= (int)timer.ElapsedMilliseconds;
          if(timeToWait < 0) timeToWait = 0;
          timer.Reset(); // reset the stopwatch so we can see how long the next thread takes to terminate
          timer.Start(); // and start it again
        }
      }

      Debug.Assert(currentActiveThreads == 0);
    }

    if(rewriteThread != null)
    {
      rewriteEvent.Set();
      if(!rewriteThread.Join(timeToWait == Infinite ? Timeout.Infinite : timeToWait)) rewriteThread.Abort();
      rewriteThread = null;
      rewriteEvent  = null;
    }
  }

  /// <summary>Adds a link to a resource to be downloaded. The link will be enqueued even if it's already been
  /// enqueued.
  /// </summary>
  public void EnqueueUri(Uri uri)
  {
    EnqueueUri(uri, null, true);
  }

  /// <summary>Adds a link to a resource to be downloaded. The link will be enqueued even if it's already been
  /// enqueued.
  /// </summary>
  /// <param name="uri">The URI to add.</param>
  /// <param name="postData">The data to post to the given URL, in application/x-www-form-urlencoded format.</param>
  public void EnqueueUri(Uri uri, string postData)
  {
    EnqueueUri(uri, postData, true);
  }

  /// <summary>Adds a link to a resource to be downloaded.</summary>
  /// <param name="force">If true, the uri will be enqueued even if it's previously been enqueued.</param>
  public void EnqueueUri(Uri uri, bool force)
  {
    EnqueueUri(uri, null, force);
  }

  /// <summary>Adds a link to a resource to be downloaded.</summary>
  /// <param name="uri">The URI to add.</param>
  /// <param name="postData">The data to post to the given URL, in application/x-www-form-urlencoded format.</param>
  /// <param name="force">If true, the uri will be enqueued even if it's previously been enqueued.</param>
  public void EnqueueUri(Uri uri, string postData, bool force)
  {
    ValidateNewUri(uri, postData);
    EnqueueUri(CleanupInputUri(uri), postData, null, 0, LinkType.Link, force);
  }

  #region Mime overrides
  /// <summary>Adds a new MIME override, which is used to assume that resources with the given file extension are of
  /// the given MIME type.
  /// </summary>
  /// <param name="extension">The extension of the resource, which must not be null and should not include the leading
  /// period.
  /// </param>
  /// <param name="mimeType">The MIME type of the resource, which cannot be empty.</param>
  /// <param name="isPreferred">Whether this is the preferred extension for the given MIME type.</param>
  public void AddMimeOverride(string extension, string mimeType, bool isPreferred)
  {
    AddMimeOverride(new MimeOverride(extension, mimeType, isPreferred));
  }

  /// <summary>Adds a new MIME override, which is used to assume that resources with the given file extension are of
  /// the given MIME type.
  /// </summary>
  public void AddMimeOverride(MimeOverride mimeOverride)
  {
    if(string.IsNullOrEmpty(mimeOverride.MimeType)) throw new ArgumentException("The override had an empty mime type.");
    string extension = NormalizeMimeOverrideExtension(mimeOverride.Extension);
    string mimeType  = mimeOverride.MimeType.ToLowerInvariant();
    lock(extensionToMime)
    {
      extensionToMime[extension] = mimeType;
      if(mimeOverride.IsPreferred || !mimeToExtension.ContainsKey(mimeType)) mimeToExtension[mimeType] = extension;
    }
  }

  /// <summary>Adds MIME overrides for standard extensions, such as htm, html, avi, png, etc.</summary>
  /// <remarks>Calling this method includes dynamic HTML extensions. See <see cref="AddStandardMimeOverrides(bool)"/>
  /// for more details.
  /// </remarks>
  public void AddStandardMimeOverrides()
  {
    AddStandardMimeOverrides(true);
  }

  /// <summary>Adds MIME overrides for standard extensions, such as htm, html, avi, png, etc.</summary>
  /// <param name="includeDynamicHtml">Whether to include common dynamic HTML extensions such as .asp, .php, .pl, etc.</param>
  /// <remarks>This method adds extensions that are relatively unambiguous. If <paramref name="includeDynamicHtml"/>
  /// is true, slightly more ambiguous extensions are added, such as .asp, .php, etc, which are usually used to
  /// generate dynamic HTML, but are occasionally used to generate dynamic PDFs, etc. The upside of including these
  /// extensions is that HEAD requests for them may be avoided. The downside is that in the rare case that HTML content
  /// is not wanted, non-HTML content generated dynamically by these pages may also be excluded.
  /// </remarks>
  public void AddStandardMimeOverrides(bool includeDynamicHtml)
  {
    // add relatively unambiguous HTML types
    AddMimeOverride("htm",   "text/html", false);
    AddMimeOverride("html",  "text/html", true);
    AddMimeOverride("sht",   "text/html", false);
    AddMimeOverride("shtm",  "text/html", false);
    AddMimeOverride("shtml", "text/html", false);

    if(includeDynamicHtml)
    { // add potentially ambiguous HTML types
      AddMimeOverride("php",  "text/html", false);
      AddMimeOverride("php2", "text/html", false);
      AddMimeOverride("php3", "text/html", false);
      AddMimeOverride("php4", "text/html", false);
      AddMimeOverride("php5", "text/html", false);
      AddMimeOverride("asp",  "text/html", false);
      AddMimeOverride("aspx", "text/html", false);
      AddMimeOverride("jsp",  "text/html", false);
      AddMimeOverride("cgi",  "text/html", false);
      AddMimeOverride("cfm",  "text/html", false);
      AddMimeOverride("pl",   "text/html", false);
    }

    // add non-HTML types
    AddMimeOverride("avi",   "video/avi", true);
    AddMimeOverride("bmp",   "image/bmp", true);
    AddMimeOverride("bz2",   "application/x-bzip2", true);
    AddMimeOverride("class", "application/java", true);
    AddMimeOverride("css",   "text/css", true);
    AddMimeOverride("doc",   "application/msword", true);
    AddMimeOverride("exe",   "application/octet-stream", true);
    AddMimeOverride("gif",   "image/gif", true);
    AddMimeOverride("gz",    "application/x-gzip", true);
    AddMimeOverride("jar",   "application/java-archive", true);
    AddMimeOverride("jpeg",  "image/jpeg", true);
    AddMimeOverride("jpg",   "image/jpeg", false);
    AddMimeOverride("js",    "application/x-javascript", true);
    AddMimeOverride("mov",   "video/quicktime", true);
    AddMimeOverride("mp2",   "audio/mpeg", false);
    AddMimeOverride("mp3",   "audio/mpeg3", true);
    AddMimeOverride("mp3",   "audio/mpeg", true);
    AddMimeOverride("mpg",   "video/mpeg", false);
    AddMimeOverride("mpeg",  "video/mpeg", true);
    AddMimeOverride("pdf",   "application/pdf", true);
    AddMimeOverride("png",   "image/png", true);
    AddMimeOverride("ppt",   "application/mspowerpoint", true);
    AddMimeOverride("qt",    "video/quicktime", false);
    AddMimeOverride("ra",    "audio/x-realaudio", true);
    AddMimeOverride("ram",   "audio/x-pn-realaudio", true);
    AddMimeOverride("rm",    "application/vnd.rn-realmedia", true);
    AddMimeOverride("rtf",   "text/rtf", true);
    AddMimeOverride("swf",   "application/x-shockwave-flash", true);
    AddMimeOverride("tgz",   "application/gnutar", true);
    AddMimeOverride("tif",   "image/tiff", false);
    AddMimeOverride("tiff",  "image/tiff", true);
    AddMimeOverride("txt",   "text/plain", true);
    AddMimeOverride("wav",   "audio/wav", true);
    AddMimeOverride("zip",   "application/zip", true);
  }

  /// <summary>Removes all MIME overrides from the crawler.</summary>
  public void ClearMimeOverrides()
  {
    lock(extensionToMime)
    {
      extensionToMime.Clear();
      mimeToExtension.Clear();
    }
  }

  /// <summary>Returns the MIME overrides that have been added to the crawler.</summary>
  public MimeOverride[] GetMimeOverrides()
  {
    MimeOverride[] array = new MimeOverride[extensionToMime.Count];

    lock(extensionToMime)
    {
      int index = 0;
      foreach(KeyValuePair<string,string> pair in extensionToMime)
      {
        string preferredExtension;
        bool isPreferred = mimeToExtension.TryGetValue(pair.Value, out preferredExtension) &&
                           string.Equals(pair.Key, preferredExtension, StringComparison.Ordinal);
        array[index++] = new MimeOverride(pair.Key, pair.Value, isPreferred);
      }
    }
    
    return array;
  }

  /// <summary>Removes the MIME override for the given extension.</summary>
  /// <param name="extension">The extension of the resource, which must not be null and should not include the leading
  /// period.
  /// </param>
  public void RemoveMimeOverride(string extension)
  {
    extension = NormalizeMimeOverrideExtension(extension);
    lock(extensionToMime)
    {
      string mimeType;
      if(extensionToMime.TryGetValue(extension, out mimeType))
      {
        extensionToMime.Remove(extension);

        if(mimeToExtension.Remove(mimeType)) // if we removed the preferred extension, assign another as the preferred
        {                                    // extension, if there is one
          foreach(KeyValuePair<string, string> pair in extensionToMime)
          {
            if(string.Equals(pair.Value, mimeType, StringComparison.Ordinal))
            {
              mimeToExtension[mimeType] = pair.Key;
              break;
            }
          }
        }
      }
    }
  }

  /// <summary>Validates, normalizes, and returns the extension given.</summary>
  static string NormalizeMimeOverrideExtension(string extension)
  {
    if(extension == null) throw new ArgumentNullException("extension");

    extension = extension.ToLowerInvariant();
    
    if(extension.Length == 0) return string.Empty;
    else if(extension[0] == '.') return extension;
    else return "."+extension;
  }
  #endregion

  #region ConnectionThread
  /// <summary>A class that represents a thread which is responsible for downloading resources from a
  /// <see cref="Service"/>.
  /// </summary>
  sealed class ConnectionThread
  {
    public ConnectionThread(Crawler crawler)
    {
      if(crawler == null) throw new ArgumentNullException();
      this.crawler = crawler;
    }

    /// <summary>Gets an approximation of the current throughput of this connection thread.</summary>
    public int CurrentBytesPerSecond
    {
      get { return IsIdle ? 0 : currentSpeed; } // TODO: this method of calculation should be improved.
    }

    /// <summary>Gets the <see cref="Resource"/> currently being downloaded, or null if no resource is being
    /// downloaded.
    /// </summary>
    public Resource CurrentResource
    {
      get { return resource; }
    }

    /// <summary>Gets whether the connection thread is not currently processing resources.</summary>
    public bool IsIdle
    {
      get { return !IsStopping && !IsRunning; }
    }

    /// <summary>Gets whether the connection thread is currently processing resources from a service.</summary>
    public bool IsRunning
    {
      get
      {
        Thread thread = this.thread;
        return !shouldQuit && thread != null && thread.IsAlive && service != null;
      }
    }
    
    /// <summary>Gets whether the connection thread is currently attempting to idle itself.</summary>
    public bool IsStopping
    {
      get { return shouldQuit; }
    }

    /// <summary>Gets the service from which this thread is downloading resources, or null if the thread is not
    /// associated with any service.
    /// </summary>
    public Service Service
    {
      get { return service; }
    }

    /// <summary>Associates this thread with the given service, and begins downloading resources from it.</summary>
    public void Start(Service service)
    {
      if(IsStopping) throw new InvalidOperationException("The thread is in the process of quitting.");
      if(IsRunning) throw new InvalidOperationException("This thread is already running!");
      if(service == null) throw new ArgumentNullException();

      Debug.Assert(this.service == null);

      this.service = service;
      Interlocked.Increment(ref this.service.connections);
      Interlocked.Increment(ref crawler.currentActiveThreads);

      if(thread == null)
      {
        thread = new Thread(ThreadFunc);
        thread.Start();
      }

      // wait until the thread is really active and has attempted to remove an item from the queue. this prevents the
      // main crawler thread from adding a bunch of threads in quick succession because it keeps seeing that there
      // are still resources queued in the service, when it's simply that the threads haven't had a chance to drain
      // the queue yet. we'll also break out of the loop if the thread was idled for some reason.
      while(!threadActive && !shouldQuit) Thread.Sleep(10);
    }

    /// <summary>Begins the process of stopping this connection thread. The current download will be completed, but no
    /// further downloads will be started.
    /// </summary>
    public void Stop()
    {
      if(!shouldQuit)
      {
        Thread thread = this.thread;
        if(thread != null && thread.IsAlive) shouldQuit = true;
      }
    }

    /// <summary>Terminates the current download and disassociates the connection thread from a service.</summary>
    /// <param name="timeToWait">The amount of time, in milliseconds, to wait for the current download to complete,
    /// or <see cref="Infinite"/> to wait as long as it takes.
    /// </param>
    public void Terminate(int timeToWait)
    {
      if(timeToWait != Infinite && timeToWait < 0) throw new ArgumentOutOfRangeException();

      if(thread != null)
      {
        Stop();
        if(!thread.Join(timeToWait == Infinite ? Timeout.Infinite : timeToWait))
        {
          thread.Abort();
          thread.Join(50); // we don't want to call Reset() until the thread has terminated because the thread has
        }                  // an event handler for ThreadAbortException that uses the current resource
      }

      Reset();
    }

    /// <summary>Cleans up the resources and members used by the current request.</summary>
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

      currentSpeed = 0;
      resourceUri  = null;
      resource     = null;
      resourcesToReference.Clear();
    }

    /// <summary>Disassociates the connection thread from a service.</summary>
    void Disassociate()
    {
      if(service != null)
      {
        Interlocked.Decrement(ref crawler.currentActiveThreads);
        Interlocked.Decrement(ref service.connections);
        service = null;
        threadActive = false;
      }
    }

    /// <summary>Cleans up the current request, disassociates the connection thread from a service, and resets various
    /// members to completely reset the state of the connection thread.
    /// </summary>
    void Reset()
    {
      CleanupRequest();
      Disassociate();

      thread     = null;
      shouldQuit = false;
    }

    /// <summary>Downloads the resources queued in the associated service until all have been downloaded, or the thread
    /// is instructed to stop.
    /// </summary>
    void ThreadFunc()
    {
      while(!shouldQuit)
      {
        CleanupRequest(); // cleanup the request from the previous iteration

        if(service.CurrentConnections > crawler.MaxConnectionsPerServer || // if the service has too many connections,
           !service.TryDequeue(out resource)) // or the service has no more resources for us to process...
        {
          Disassociate();
          // mark that we've at least tried doing something. this prevents a deadlock where Start() is waiting for
          // threadActive to become true (within a services lock from CrawlService), and OnThreadIdle() is waiting for
          // services to be unlocked
          threadActive = true;
          crawler.OnThreadIdle(this); // the crawler will re-associate or stop this thread
          continue;
        }

        threadActive = true; // mark that we've started doing something
        Stopwatch timer = new Stopwatch();
        timer.Start();

        // if no link to the resource lower than the depth limit has been found, don't download it. however, don't
        // subject external resources (if we want them) or form POSTs to this restriction. i realize it's possible that
        // a shallower link to it might be found later, but this may be the best we can do...
        if(crawler.DepthLimit != Infinite && resource.Depth >= crawler.DepthLimit &&
           (resource.PostData != null ||
            resource.type != LinkType.Resource || (crawler.Download & DownloadFlags.ExternalResources) == 0))
        {
          BeginRewritingToOriginalUrl(resource);
          continue;
        }

        string localPath = null;
        resourceUri = resource.Uri;
        try
        {
          bool crawlerWantsEverything = (crawler.Download & DownloadFlags.TypeMask) == DownloadFlags.Everything;
          ResourceType resourceType = ResourceType.Unknown;

          if(!crawlerWantsEverything && resource.PostData == null)
          {
            // we need to determine whether we actually want to download this item
            resourceType = crawler.GuessResourceType(resource);

            // we can't be sure about resources from FTP (there is no Content-Type header), so assume they're non-html
            if(resourceType == ResourceType.Unknown &&
               string.Equals(resourceUri.Scheme, Uri.UriSchemeFtp, StringComparison.Ordinal))
            {
              resourceType = ResourceType.Binary;
            }

            // skip non-Html resources if we don't want them. we'll still need to download Html resources to scan them
            // for links
            if(resource.PostData == null && // except that we always obey post requests
               resourceType == ResourceType.Binary && !crawler.WantResource(resourceType)) continue;
          }

          request = WebRequest.Create(resourceUri);

          HttpWebRequest httpRequest = request as HttpWebRequest;
          HttpWebResponse httpResponse;
          if(httpRequest != null)
          {
            SetupHttpRequest(httpRequest);

            if(!crawlerWantsEverything && resourceType == ResourceType.Unknown && resource.PostData == null)
            {
              httpRequest.Method = "HEAD"; // use a HEAD request to determine the content type before downloading it
              httpResponse = (HttpWebResponse)httpRequest.GetResponse();

              if(crawler.UseCookies) Service.SaveCookies(httpResponse);
              resource.contentType = GetMimeType(httpResponse.ContentType);
              resourceType = Crawler.GetResourceType(resource.ContentType);
              httpResponse.Close();

              // if it's not HTML and we don't want it, go to the next resource. we still need to download HTML to find
              // the links.
              if(resourceType == ResourceType.Binary && !crawler.WantResource(resourceType))
              {
                BeginRewritingToOriginalUrl(resource);
                continue;
              }

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
              crawler.ReadTimeout == Infinite ? Timeout.Infinite : crawler.ReadTimeout*1000;
            ftpRequest.UsePassive = crawler.PassiveFtp;
          }

          crawler.OnProgress(resource, Status.DownloadStarted);

          request.Timeout = crawler.TransferTimeout == Infinite ? Timeout.Infinite : crawler.TransferTimeout*1000;

          if(resource.PostData != null)
          {
            using(StreamWriter postWriter = new StreamWriter(request.GetRequestStream()))
            {
              postWriter.Write(resource.PostData);
            }
          }

          response = request.GetResponse();

          httpResponse = response as HttpWebResponse;
          if(httpResponse != null)
          {
            if(crawler.UseCookies) Service.SaveCookies(httpResponse);
            resource.contentType = GetMimeType(httpResponse.ContentType);

            // get the real resource type based on what the server actually reported
            resourceType = Crawler.GetResourceType(resource.ContentType);
            // if it's not HTML and we don't want it, go to the next resource. we still need to download HTML to find
            // the links.
            if(resourceType == ResourceType.Binary && !crawler.WantResource(resourceType))
            {
              BeginRewritingToOriginalUrl(resource);
              goto finished;
            }

            resource.responseCode = httpResponse.StatusCode;
            resource.responseText = httpResponse.StatusDescription;

            // if the response Uri is different from the resource Uri, it was probably a redirection of some sort.
            if(!httpResponse.ResponseUri.Equals(resourceUri))
            {
              resourceUri = httpResponse.ResponseUri;

              // ensure that the place we redirected to is an allowed Url
              bool isExternal;
              if(!crawler.IsUriAllowed(httpResponse.ResponseUri, resource.type, out isExternal))
              {
                BeginRewritingToOriginalUrl(resource);
                goto finished;
              }
            }
          }
          else if(response is FtpWebResponse)
          {
            resource.responseText = ((FtpWebResponse)response).StatusDescription;
            resource.contentType  = GetMimeType(resourceType);
          }

          // at this point, we know that we're going to download the data, so create the file to hold it
          localPath = service.GetLocalFileName(resource.Uri, resource.contentType, resource.type);
          if(localPath == null) // if the filename is null, it means this resource is not wanted.
          {                     // this can happen for a page with too many different query strings
            BeginRewritingToOriginalUrl(resource);
            goto finished;
          }

          FileStream outFile = new FileStream(localPath, FileMode.Create, FileAccess.Write);
          resource.size = response.ContentLength;
          if(response.ContentLength != -1) outFile.SetLength(response.ContentLength);
          CopyStream(response.GetResponseStream(), outFile, timer);
          crawler.OnResourceTransferred(resource);

          resource.encoding = resourceType != ResourceType.Binary ? GetEncoding(httpResponse) : null;

          // allow the user to filter the content
          crawler.DoFilterContent(resourceUri, resource.ContentType, resourceType, resource.encoding, localPath);

          if(resourceType != ResourceType.Binary)
          {
            string content;

            using(StreamReader sr = new StreamReader(localPath, resource.encoding))
            {
              content = sr.ReadToEnd();
            }

            // if an HTML document has a content type meta tag, and it's different from what the server reported,
            // reload the file if we can
            if(resourceType == ResourceType.Html)
            {
              Match contentTypeMatch = metaRe.Match(content);
              if(contentTypeMatch.Success)
              {
                Encoding rightEncoding = null;
                try
                {
                  rightEncoding = Encoding.GetEncoding(contentTypeMatch.Groups["charset"].Value);
                  if(!string.Equals(rightEncoding.WebName, resource.encoding.WebName, StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(rightEncoding.WebName, "us-ascii", StringComparison.OrdinalIgnoreCase))
                  {
                    resource.encoding = rightEncoding;
                    using(StreamReader sr = new StreamReader(localPath, resource.encoding))
                    {
                      content = sr.ReadToEnd();
                    }
                  }
                }
                catch(ArgumentException) { }
              }
            }

            if(!crawler.RewriteLinks || !crawler.WantResource(resourceType))
            {
              ScanForLinks(content, resourceType);
            }
            else
            {
              content = ScanForAndRewriteLinks(content, resourceType, Path.GetDirectoryName(localPath));
              using(FileStream file = new FileStream(localPath, FileMode.Create,
                                                     FileAccess.Write, FileShare.Read))
              {
                byte[] bytes = resource.encoding.GetBytes(content);
                file.Write(bytes, 0, bytes.Length);
              }
            }

            if(!crawler.WantResource(resourceType)) // if we didn't actually want it, delete it
            {
              service.FreeLocalFileName(resource.Uri, localPath);
              localPath = null;
              BeginRewritingToOriginalUrl(resource);
            }
          }

          // if we wanted this file, rewrite links that reference it
          if(crawler.WantResource(resourceType))
          {
            resource.localPath = localPath;
            BeginRewritingToLocalPath(resource);

            foreach(Resource res in resourcesToReference)
            {
              lock(res)
              {
                res.AddReference(resource);
                // it's possible that the resource was rewritten between the time we added it to resourcesToReference
                // and now. in that case, the reference would be ignored. so we need to manually rewrite it again to
                // pick up the new reference
                if(res.rewriteStatus == RewriteStatus.Rewritten)
                {
                  if(res.LocalPath != null) BeginRewritingToLocalPath(res);
                  else BeginRewritingToOriginalUrl(res);
                }
              }
            }
          }

          finished:
          crawler.OnProgress(resource, Status.DownloadFinished);
        }
        catch(ThreadAbortException ex) // if the thread is terminated, readd the resource without incrementing its
        {                              // failure count, since it's not its fault that it failed
          Resource currentResource = resource; // we have to grab a local copy of the member variables because during
          Service currentService = service;    // a thread abortion, they may be cleared by another thread
          if(currentResource != null && currentService != null)
          {
            if(localPath != null) currentService.FreeLocalFileName(currentResource.Uri, localPath);
            currentService.Enqueue(currentResource, true);
            crawler.OnErrorOccurred(currentResource, ex, false);
          }
        }
        catch(Exception ex)
        {
          HandleDownloadException(ex, localPath);
        }
      }
      
      Reset();
    }

    /// <summary>Adds the resource to the rewrite queue, to rewrite links to its local path, if link rewriting is
    /// enabled and it does not seem to be added already.
    /// </summary>
    void BeginRewritingToLocalPath(Resource resource)
    {
      crawler.AddToRewriteQueue(resource, RewriteStatus.RewriteToFile);
    }

    /// <summary>Adds the resource to the rewrite queue, to rewrite links to its original URL, if link rewriting is
    /// enabled and it does not seem to be added already.
    /// </summary>
    void BeginRewritingToOriginalUrl(Resource resource)
    {
      crawler.AddToRewriteQueue(resource, RewriteStatus.RewriteToUrl);
    }

    /// <summary>Copies data from the source stream to the destination stream. The amount of data copied will be
    /// limited by <see cref="Crawler.MaxFileSize"/>.
    /// </summary>
    void CopyStream(Stream source, Stream dest, Stopwatch timer)
    {
      try
      {
        byte[] dataBuffer = new byte[65536];
        resource.bytesDownloaded = 0;
        while(crawler.MaxFileSize == Infinite || resource.BytesDownloaded < crawler.MaxFileSize)
        {
          int read = source.Read(dataBuffer, 0,
                                 crawler.MaxFileSize == Infinite ? dataBuffer.Length :
                                   (int)Math.Min(crawler.MaxFileSize-resource.BytesDownloaded, dataBuffer.Length));
          if(read == 0) break;
          dest.Write(dataBuffer, 0, read);
          resource.bytesDownloaded += read;

          long elapsedMs = timer.ElapsedMilliseconds;
          if(elapsedMs == 0) elapsedMs = 1; // prevent division by zero
          currentSpeed = (int)(resource.BytesDownloaded*1000 / elapsedMs);
        }
      }
      finally
      {
        dest.Close();
        source.Close();
      }
    }

    /// <summary>Given a base URI and a regex group matching the link text, returns an absolute URI suitable to be
    /// enqueued into the crawler, or null if the link should be ignored.
    /// </summary>
    /// <param name="baseUri">The uri that relative links should be considered in relation to.</param>
    /// <param name="linkGroup">A regex group matching the link text.</param>
    /// <param name="decodeEntities">If true, HTML entities in the link will be decoded before the link is processed.</param>
    /// <returns>Returns an absolute URI suitable for enqueuing, or null if the link should be ignored.</returns>
    Uri GetAbsoluteLinkUrl(Uri baseUri, Group linkGroup, bool decodeEntities)
    {
      string linkText = linkGroup.Value;
      if(decodeEntities) linkText = HttpUtility.HtmlDecode(linkText);

      // ignore javascript and mailto links
      if(linkText.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
         linkText.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
      {
        return null;
      }

      Uri newUri = new Uri(linkText, UriKind.RelativeOrAbsolute);
      if(!newUri.IsAbsoluteUri) newUri = new Uri(baseUri, newUri); // if the link is not absolute, make it absolute

      return Crawler.CleanupInputUri(newUri);
    }

    /// <summary>Given a regex match, returns the regex group containing the link text, and the type of the link.</summary>
    /// <param name="m">The regex match containing the link text.</param>
    /// <param name="type">Receives the type of the link.</param>
    /// <returns>Returns the regex group matching the link text.</returns>
    Group GetLinkMatchGroup(Match m, out LinkType type)
    {
      type = LinkType.Link;
      Group g = m.Groups["link"];
      if(!g.Success)
      {
        g = m.Groups["resLink"];
        type = LinkType.Resource;
      }
      return g;
    }

    /// <summary>Given an exception that occurred while downloading the given resource, increments the failure count
    /// of the resource, issues a progress update, and either re-enqueues the resource or drops it, depending on
    /// whether the error was fatal.
    /// </summary>
    void HandleDownloadException(Exception ex, string localPath)
    {
      // grab local copies of these member variables because they may be set to null by another thread during a thread
      // abortion
      Resource resource = this.resource;
      Service service = this.service;
      if(resource == null) return;

      bool isFatal = (resource.failures++ >= crawler.MaxRetries && crawler.MaxRetries != Infinite) ||
                     IsFatalError(ex);
      if(!isFatal)
      {
        if(service != null)
        {
          if(localPath != null) service.FreeLocalFileName(resource.Uri, localPath);
          service.Enqueue(resource, true);
        }
      }
      else if(localPath != null)
      {
        if(crawler.GenerateErrorFiles)
        {
          using(StreamWriter writer = new StreamWriter(localPath))
          {
            writer.Write("<html>\n<head><title>Error occurred</title></head>\n<body>\n");
            writer.Write("<h3>An error occurred while downloading this resources</h3>\n");
            writer.Write("<b>Url:</b> ");
            writer.Write(HttpUtility.HtmlEncode(resource.Uri.AbsoluteUri));
            writer.Write("<br/>\n<b>Error:</b> ");
            writer.Write(HttpUtility.HtmlEncode(ex.Message));
            writer.Write("<br/>\n<br/>\nFull error text:<br/>\n");
            writer.Write(HttpUtility.HtmlEncode(ex.ToString()).Replace("\n", "<br/>\n"));
            writer.Write("\n</body>\n</html>\n");
          }
        }
        else if(service != null)
        {
          service.FreeLocalFileName(resource.Uri, localPath);
        }
      }

      crawler.OnErrorOccurred(resource, ex, isFatal);
    }

    /// <summary>Handles a link that has been found in a resource by extracting, filtering, and enqueueing it.</summary>
    /// <param name="baseUri">The base URI that relative links will be considered in relation to.</param>
    /// <param name="referrer">The URI of the resource that contained the link.</param>
    /// <param name="m">A regex match containing the link text.</param>
    /// <param name="decodeEntities">If true, HTML entities will be decoded in the link text before it is processed.</param>
    void HandleLinkMatch(Uri baseUri, Uri referrer, Match m, bool decodeEntities)
    {
      LinkType type;
      Uri uri = crawler.FilterUri(GetAbsoluteLinkUrl(baseUri, GetLinkMatchGroup(m, out type), decodeEntities));
      if(uri != null)
      {
        try { crawler.EnqueueUri(uri, null, referrer, resource.Depth+1, type, false); }
        catch(UriFormatException) { }
      }
    }

    /// <summary>Scans a document for links, and processes each link found.</summary>
    void ScanForLinks(string content, ResourceType resourceType)
    {
      Uri baseUri = resourceUri; // use the resource uri as the base uri by default

      if(resourceType == ResourceType.Html)
      {
        Match m = baseRe.Match(content); // but if the document specifies a different base Uri, use it.
        if(m.Success)
        {
          try { baseUri = new Uri(m.Value); }
          catch(UriFormatException) { }
        }

        m = htmlLinkRe.Match(content); // now look for links in the HTML
        while(m.Success)
        {
          HandleLinkMatch(baseUri, resourceUri, m, true);
          m = m.NextMatch();
        }

        m = styleRe.Match(content); // now look for style blocks
        while(m.Success)
        {
          Match linkMatch = cssLinkRe.Match(m.Groups["css"].Value); // and links within the style blocks
          while(linkMatch.Success)
          {
            HandleLinkMatch(baseUri, resourceUri, linkMatch, false);
            linkMatch = linkMatch.NextMatch();
          }

          m = m.NextMatch();
        }
      }
      else if(resourceType == ResourceType.Css)
      {
        Match m = cssLinkRe.Match(content); // search for links within CSS
        while(m.Success)
        {
          HandleLinkMatch(baseUri, resourceUri, m, false);
          m = m.NextMatch();
        }
      }
    }
    
    /// <summary>Scans a document for links, and processes each link found, replacing the link text with a
    /// reference to the locally downloaded resource.
    /// </summary>
    /// <param name="content">The content of the document.</param>
    /// <param name="resourceType">The type of content contained within the document.</param>
    /// <param name="localDir">The directory in which the document is stored. This is used to calculate the
    /// relative path used to replace links.
    /// </param>
    /// <returns>Returns the new document, with links replaced.</returns>
    string ScanForAndRewriteLinks(string content, ResourceType resourceType, string localDir)
    {
      Uri baseUri = resourceUri; // use the resource uri as the base uri by default

      MatchEvaluator cssReplacer =
          delegate(Match match) { return RewriteLink(match, baseUri, resourceUri, localDir, false); };

      if(resourceType == ResourceType.Html)
      {
        Match m = baseRe.Match(content);
        if(m.Success) // but if the document specifies a different base Uri, use it.
        {
          try { baseUri = new Uri(m.Value); }
          catch(UriFormatException) { }
          content = baseRe.Replace(content, "."); // replace the base with '.'
        }

        MatchEvaluator htmlReplacer =
          delegate(Match match) { return RewriteLink(match, baseUri, resourceUri, localDir, true); };

        content = htmlLinkRe.Replace(content, htmlReplacer);
        content = styleRe.Replace(content, delegate(Match match) { return cssLinkRe.Replace(match.Value, cssReplacer); });
      }
      else if(resourceType == ResourceType.Css)
      {
        content = cssLinkRe.Replace(content, cssReplacer);
      }

      return content;
    }

    /// <summary>Prepares an <see cref="HttpWebRequest"/> for issuance, by setting its various properties, such as
    /// timeouts, user agent, cookies, etc.
    /// </summary>
    void SetupHttpRequest(HttpWebRequest httpRequest)
    {
      crawler.SetServicePointProperties(httpRequest.ServicePoint);

      if(crawler.UseCookies) service.LoadCookies(httpRequest);

      Uri referrer = resource.Referrer != null ? resource.Referrer : crawler.DefaultReferrer;
      if(referrer != null) httpRequest.Referer = referrer.AbsoluteUri;

      if(resource.PostData != null)
      {
        httpRequest.Method      = "POST";
        httpRequest.ContentType = "application/x-www-form-urlencoded";
      }

      if(!string.IsNullOrEmpty(crawler.UserAgent))
      {
        httpRequest.UserAgent = crawler.UserAgent;
      }

      if(!string.IsNullOrEmpty(crawler.PreferredLanguage))
      {
        httpRequest.Headers[HttpRequestHeader.AcceptLanguage] = crawler.PreferredLanguage;
      }
      
      httpRequest.ReadWriteTimeout = crawler.ReadTimeout == Infinite ? Timeout.Infinite : crawler.ReadTimeout*1000;
      httpRequest.MaximumAutomaticRedirections = crawler.MaxRedirects;
    }

    /// <summary>Rewrites the link found within a document, and returns the new link text.</summary>
    /// <param name="m">A regex match containing the link text.</param>
    /// <param name="baseUri">The URI to which relative links should be consider to be related.</param>
    /// <param name="referrer">The URI of the resource in which the link was found.</param>
    /// <param name="localDir">The directory in which the resource containing the link is saved.</param>
    /// <param name="decodeEntities">If true, HTML entities will be decoded in the link text before it's processed.</param>
    /// <returns>Returns the new link text.</returns>
    string RewriteLink(Match m, Uri baseUri, Uri referrer, string localDir, bool decodeEntities)
    {
      try
      {
        LinkType type;
        Group g = GetLinkMatchGroup(m, out type);
        Uri origAbsUri = GetAbsoluteLinkUrl(baseUri, g, decodeEntities);
        if(origAbsUri != null)
        {
          Uri absUri = crawler.FilterUri(origAbsUri);
          string prefix = m.Value.Substring(0, g.Index-m.Index);
          string suffix = m.Value.Substring(g.Index-m.Index+g.Length);
          string newUri = null;
          if(absUri != null)
          {
            Resource resource = crawler.EnqueueUri(absUri, null, referrer, this.resource.Depth+1, type, false);
            if(resource != null)
            {
              lock(resource)
              {
                if(resource.LocalPath != null) // if the resource has already been saved,
                {                              // we can reference the file immediately
                  // we pass absUri rather than resource.Uri because the Uri fragment of the resource returned from
                  // EnqueueUri may be different than the fragment in absUri
                  newUri = crawler.GetRelativeUri(localDir, resource.LocalPath, absUri, type);
                }
                // otherwise, if the resource hasn't been rewritten yet, add this file as a reference and use the
                // rewrite tag to create a placeholder url
                else if(resource.rewriteStatus != RewriteStatus.Rewritten)
                {
                  if(resource.rewriteTag == 0) resource.rewriteTag = crawler.GetNextRewriteTag();
                  int index = resourcesToReference.BinarySearch(resource, ResourceComparer.Instance);
                  if(index < 0) resourcesToReference.Insert(~index, resource);
                  newUri = resource.PlaceholderUrl + absUri.Fragment;
                }
                // if the resource has been rewritten, but hasn't been saved, then we didn't want the file, so we
                // should just link back to the original source
              }
            }
          }

          return prefix + (newUri != null ? newUri : (absUri != null ? absUri : origAbsUri).AbsoluteUri) + suffix;
        }
      }
      catch(UriFormatException) { }

      return m.Value;
    }

    /// <summary>The crawler employing the connection thread.</summary>
    readonly Crawler crawler;
    /// <summary>The service currently associated with the connection thread, or null if none.</summary>
    Service service;

    /// <summary>The <see cref="WebRequest"/> for the resource currently being downloaded.</summary>
    WebRequest request;
    /// <summary>The <see cref="WebResponse"/> for the resource currently being downloaded.</summary>
    WebResponse response;
    /// <summary>The response stream for the resource currently being downloaded, if one has been retrieved so far.</summary>
    Stream responseStream;
    /// <summary>The resource currently being downloaded.</summary>
    Resource resource;
    /// <summary>The URI of the resource currently being downloaded. This may differ from the Uri of
    /// <see cref="resource"/> in the case of a redirection.
    /// </summary>
    Uri resourceUri;
    /// <summary>A list of resources that will be referenced as soon as the link rewriting process for the current
    /// download completes.
    /// </summary>
    List<Resource> resourcesToReference = new List<Resource>();

    /// <summary>The thread responsible for downloading resources from the associated service.</summary>
    Thread thread;
    /// <summary>A value containing the current download speed, in bytes per second.</summary>
    int currentSpeed;
    /// <summary>If true, the download thread should terminate as soon as the current download finishes.</summary>
    bool shouldQuit;
    /// <summary>If true, the thread has started and examined the service queue at least once.</summary>
    volatile bool threadActive;

    /// <summary>Given an <see cref="HttpWebResponse"/>, retrieves and parses the Content-Encoding into an
    /// <see cref="Encoding"/> object. If the content encoding cannot be parsed, UTF-8 is assumed.
    /// </summary>
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

    /// <summary>Parses an HTTP content type from the Content-Type header and returns the MIME type therein.</summary>
    static string GetMimeType(string contentType)
    {
      if(contentType == null) return null;
      int endPos = contentType.IndexOf(';');
      return endPos == -1 ? contentType : contentType.Substring(0, endPos);
    }

    /// <summary>Converts a <see cref="ResourceType"/> into a normalized MIME type.</summary>
    static string GetMimeType(ResourceType resourceType)
    {
      switch(resourceType)
      {
        case ResourceType.Css: return "text/css";
        case ResourceType.Html: return "text/html";
        case ResourceType.Javascript: return "text/javascript";
        case ResourceType.VBScript: return "text/vbscript";
        default: return "application/octet-stream";
      }
    }

    /// <summary>Returns true if the exception represents an error that is not likely to go away if the download is
    /// retried.
    /// </summary>
    static bool IsFatalError(Exception ex)
    {
      if(ex is IOException) return false; // IO exceptions occur, for instance, if the connection is terminated

      WebException we = ex as WebException;
      if(we == null) return true;

      if(we.Status == WebExceptionStatus.ProtocolError)
      {
        HttpWebResponse httpResponse = we.Response as HttpWebResponse;
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
        
        FtpWebResponse ftpResponse = we.Response as FtpWebResponse;
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
      else if(we.Status == WebExceptionStatus.MessageLengthLimitExceeded)
      {
        return true;
      }
      
      return false;
    }

    const RegexOptions BaseRegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant |
                                          RegexOptions.IgnoreCase | RegexOptions.Singleline;
    
    /// <summary>Matches BASE tags in HTML.</summary>
    static readonly Regex baseRe = new Regex(@"(?<=<base\s[^>]*href\s*=\s*""?)[^"">]+", BaseRegexOptions);

    /// <summary>Matches links in HTML.</summary>
    static readonly Regex htmlLinkRe =
      new Regex(@"<(?:a\b[^>]*?\bhref\s*=\s*(?:""(?<link>[^"">]+)|'(?<link>[^'>]+)|(?<link>[^>\s]+))|
                   (?:img|script|embed)\b[^>]*?\bsrc\s*=\s*(?:""(?<resLink>[^"">]+)|'(?<resLink>[^'>]+)|(?<resLink>[^>\s]+))|
                   i?frame\b[^>]*?\bsrc\s*=\s*(?:""(?<link>[^"">]+)|'(?<link>[^'>]+)|(?<link>[^>\s]+))|
                   link\b[^>]*?\bhref\s*=\s*(?:""(?<resLink>[^"">]+)|'(?<resLink>[^'>]+)|(?<resLink>[^>\s]+))|
                   applet\b[^>]*?\b(?:code|object)\s*=\s*(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+)|(?<resLink>[^>\s]+))|
                   object\b[^>]*?\b(?:data|codebase)\s*=\s*(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+)|(?<resLink>[^>\s]+))|
                   param\s+name=[""'](?:src|href|file|filename|data|movie)[""']\s+value=(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+)|(?<resLink>[^>\s]+))|
                   \w+\b[^>]+?\b(?:background|bgimage)\s*=\s*(?:""(?<resLink>[^""]+)|'(?<resLink>[^'>]+)|(?<resLink>[^>\s]+)))",
                BaseRegexOptions | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>Matches STYLE tags in HTML.</summary>
    static readonly Regex styleRe =
      new Regex(@"<style(?:\s[^>]*)?>(?<css>.*?)</style>|<[^>]+\bstyle\s*=\s*(?:""(?<css>[^"">]+)|'(?<css>[^'>]+))",
                BaseRegexOptions);
    
    /// <summary>Matches SCRIPT tags in HTML.</summary>
    static readonly Regex scriptRe = new Regex(@"<script(?:\s[^>]*)?>(.*?)</script>", BaseRegexOptions);
    
    /// <summary>Matches META tags that set the content type.</summary>
    static readonly Regex metaRe =
      new Regex(@"<meta\b[^>]*?\b(?:http-equiv=""content-type""[^>]*?\bcontent=""[^""]*?charset=(?<charset>[\w-]+)""|
                                    content=""[^""]*?charset=(?<charset>[\w-]+)""[^>]*?\bhttp-equiv=""content-type"")",
                BaseRegexOptions | RegexOptions.IgnorePatternWhitespace);

    /// <summary>Matches links in CSS.</summary>
    static readonly Regex cssLinkRe =
      new Regex(@"@import ""(?<resLink>[^""]+)|url\(['""]?(?<resLink>[^)]+?)['""]?\)", BaseRegexOptions);
  }
  #endregion

  #region Service
  /// <summary>Represents an internet service, such as an HTTP or FTP server, from which resources will be downloaded.</summary>
  sealed class Service
  {
    public Service(Crawler crawler, Uri uri)
    {
      if(uri == null) throw new ArgumentNullException();
      if(!uri.IsAbsoluteUri) throw new ArgumentException("The uri must be absolute.");

      this.crawler  = crawler;
      this.baseUri  = new Uri(uri.GetLeftPart(UriPartial.Authority));
      
      InitializeBaseDirectory();
    }

    static Service()
    {
      badChars = Path.GetInvalidFileNameChars();
      Array.Sort(badChars);
    }

    /// <summary>Gets the full path to the directory under which resources from this service will be saved.</summary>
    public string BaseDirectory
    {
      get { return baseDir; }
    }

    /// <summary>Gets the base URI for this resources, for instance http://www.foo.com.</summary>
    public Uri BaseUri
    {
      get { return baseUri; }
    }

    /// <summary>Gets the number of current connections to this internet service.</summary>
    public int CurrentConnections
    {
      get { return connections; }
    }

    /// <summary>Gets whether the download queue is empty.</summary>
    public bool IsQueueEmpty
    {
      get { lock(resources) return resources.Count == 0; }
    }

    /// <summary>Gets the number of resources currently existing in the download queue.</summary>
    public int ResourceCount
    {
      get { return resources.Count; }
    }

    /// <summary>Removes all resources from the download queue.</summary>
    public void Clear(bool allowReEnqueue)
    {
      lock(resources)
      {
        resources.Clear();

        if(allowReEnqueue)
        {
          queued.Clear();
        }
        else
        {
          foreach(string key in new List<string>(queued.Keys)) queued[key] = null;
        }
      }
    }

    /// <summary>Adds a new resource to the download queue, if it has not already been queued in the past.</summary>
    /// <returns>Returns true if the resource was enqueued and false if not.</returns>
    public void Enqueue(Resource resource)
    {
      Enqueue(resource, false);
    }

    /// <summary>Adds a new resource to the download queue.</summary>
    /// <param name="resource">The resource to add.</param>
    /// <param name="forceRequeue">If true, the resource will be added to the download queue even if it was already
    /// added previously.
    /// </param>
    /// <returns>Returns the resource existing in the queue, which may be different from the resource given, or null if
    /// no resource for the URI exists in the queue and none was added.
    /// </returns>
    public Resource Enqueue(Resource resource, bool forceRequeue)
    {
      if(!resource.IsValid) throw new ArgumentException();

      Uri uri = resource.Uri;
      Debug.Assert(uri.Scheme == baseUri.Scheme && uri.Port == baseUri.Port &&
                   string.Equals(uri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase));

      Resource existingResource;
      string key = MakeKey(uri);

      lock(resources)
      {
        bool alreadyQueued = queued.TryGetValue(key, out existingResource);

        if(existingResource != null)
        {
          // update the depth of the resource to the lowest depth at which a link has been found
          existingResource.depth = Math.Min(existingResource.Depth, resource.Depth);
        }
        else if(forceRequeue || !alreadyQueued)
        {
          // if the URL is not allowed due to the queue limit, don't enqueue it, and return null
          if(!forceRequeue &&
             crawler.MaxQueuedLinks != Infinite && crawler.CurrentLinksQueued >= crawler.MaxQueuedLinks)
          {
            return null;
          }

          queued[key] = resource;
          EnqueueCore(resource);
          crawler.OnProgress(resource, Status.UrlQueued);
        }
      }

      return existingResource != null ? existingResource : resource;
    }

    /// <summary>Removes all resources from the download queue with URIs that match the given regex.</summary>
    /// <param name="uriRegex">The regex against which resource URIs will be matched.</param>
    /// <param name="allowRequeue">If true, the resources removed can be reenqueued in the future.</param>
    public void Remove(Regex uriRegex, bool allowRequeue)
    {
      lock(resources)
      {
        Resource[] array = resources.ToArray();
        resources.Clear();
        for(int i=0; i<array.Length; i++)
        {
          if(!uriRegex.IsMatch(array[i].Uri.AbsoluteUri)) EnqueueCore(array[i]);
          else if(allowRequeue) queued.Remove(MakeKey(array[i].Uri));
        }
      }
    }

    /// <summary>Attempts to remove the next resource from the download queue.</summary>
    /// <returns>Returns true if a resource was removed and false if not.</returns>
    public bool TryDequeue(out Resource resource)
    {
      lock(resources)
      {
        if(resources.Count == 0)
        {
          resource = null;
          return false;
        }
        else
        {
          resource = (crawler.Download & DownloadFlags.PrioritizeNonHtml) != 0 ?
            resources.RemoveLast() : resources.RemoveFirst();
          // if we're not rewriting links, then we don't need to keep the resource class around
          if(!crawler.RewriteLinks) queued[MakeKey(resource.Uri)] = null;
          return true;
        }
      }
    }

    public void FreeLocalFileName(Uri uri, string localFileName)
    {
      string baseUriPath = uri.AbsolutePath;
      if(!crawler.CaseSensitivePaths) baseUriPath = baseUriPath.ToLowerInvariant();

      lock(files)
      {
        LocalFileInfo info;
        if(files.TryGetValue(baseUriPath, out info))
        {
          if(info.Queries != null && !string.IsNullOrEmpty(uri.Query)) info.Queries.Remove(uri.Query);
          if(info.Queries == null || info.Queries.Count == 0) files.Remove(baseUriPath);
        }
      }

      try { File.Delete(localFileName); }
      catch { }
    }

    /// <summary>Given a URI and its type, allocates and returns a local file to store the data downloaded for the
    /// resource, or null if the resource should be not be downloaded.
    /// </summary>
    public string GetLocalFileName(Uri uri, string mimeType, LinkType type)
    {
      string baseUriPath = uri.AbsolutePath;
      if(!crawler.CaseSensitivePaths) baseUriPath = baseUriPath.ToLowerInvariant();

      string localFileName;
      lock(files)
      {
        LocalFileInfo info;
        bool reAdd = false, touch = false;

        if(!files.TryGetValue(baseUriPath, out info))
        {
          info  = new LocalFileInfo(FindUnusedFile(baseUriPath, mimeType, type));
          reAdd = touch = true;
        }

        localFileName = info.BasePath;
        
        if(!string.IsNullOrEmpty(uri.Query))
        {
          if(info.Queries == null)
          {
            info.Queries = new Dictionary<string,string>();
            reAdd = true;
          }

          string queryPart;
          if(!info.Queries.TryGetValue(uri.Query, out queryPart))
          {
            if(crawler.MaxQueryStringsPerFile != 0 && info.Queries.Count >= crawler.MaxQueryStringsPerFile)
            {
              return null;
            }

            info.Queries[uri.Query] = queryPart = FindQuerySuffix(localFileName, uri.Query, mimeType, type);
            touch = true;
          }

          localFileName = AddQuerySuffix(localFileName, queryPart, mimeType, type);
        }

        if(reAdd) files[baseUriPath] = info;

        // create an empty file to prevent another resource from getting the same one
        if(touch) new FileStream(localFileName, FileMode.Create, FileAccess.Write, FileShare.None).Close();
      }

      return localFileName;
    }

    /// <summary>Given an <see cref="HttpWebRequest"/>, loads the cookies from the service back into the request
    /// object.
    /// </summary>
    public void LoadCookies(HttpWebRequest request)
    {
      if(request.CookieContainer == null) request.CookieContainer = new CookieContainer();

      if(cookies != null)
      {
        lock(this) request.CookieContainer.Add(request.RequestUri, cookies.GetCookies(request.RequestUri));
      }
    }

    /// <summary>Given an <see cref="HttpWebResponse"/>, saves the cookies returned by the web server into the service.</summary>
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

    public override string ToString()
    {
      return BaseUri.AbsoluteUri;
    }
    
    /// <summary>A structure representing a web page, not including the query string, and the local files associated
    /// with the page and its query strings.
    /// </summary>
    struct LocalFileInfo
    {
      public LocalFileInfo(string fileName) { BasePath = fileName; Queries = null; }

      /// <summary>The local file name associated with the web page.</summary>
      public string BasePath;
      /// <summary>A dictionary mapping query strings to the strings appended onto the filename.</summary>
      public Dictionary<string,string> Queries;
    }

    /// <summary>Given a filename and a query string suffix, adds the query string key to the filename.</summary>
    string AddQuerySuffix(string path, string querySuffix, string mimeType, LinkType type)
    {
      return GetPathWithoutExtension(path) + "_" + querySuffix + GetExtension(path, mimeType, type);
    }

    /// <summary>Given a filename and a query string, finds a unique suffix that can be appended to the filename
    /// to represent the query including that key.
    /// </summary>
    string FindQuerySuffix(string path, string query, string mimeType, LinkType type)
    {
      string querySuffix = ToHex((uint)query.GetHashCode()), extension = GetExtension(path, mimeType, type);
      path = GetPathWithoutExtension(path) + "_";

      string queryToTest = querySuffix;
      int suffix = 2;
      while(File.Exists(path+queryToTest+extension))
      {
        queryToTest = querySuffix + suffix.ToString(CultureInfo.InvariantCulture);
        suffix++;
      }
      
      return queryToTest;
    }

    /// <summary>Adds the given resource to the front or back of the queue depending on its apparent resource type.</summary>
    void EnqueueCore(Resource resource)
    {
      if(crawler.GuessResourceType(resource) == ResourceType.Binary) resources.Add(resource);
      else resources.Insert(0, resource);
    }

    /// <summary>Given a path below the service base URI, finds a filename to store the resources associated with that
    /// path.
    /// </summary>
    string FindUnusedFile(string baseUriPath, string mimeType, LinkType type)
    {
      if(baseUriPath.StartsWith("/")) baseUriPath = baseUriPath.Substring(1);

      string[] bits = baseUriPath.Split('/');
      for(int i=0; i<bits.Length; i++) bits[i] = UrlSafeDecode(bits[i]);
      baseUriPath = string.Join("/", bits);

      string directory = this.baseDir;
      string fileName  = null, extension = GetExtension(baseUriPath, mimeType, type);

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

      // get the filename without extension. for things that look like directories, name the file "index"
      fileName = string.IsNullOrEmpty(fileName) ? "index" : Path.GetFileNameWithoutExtension(fileName);
      // now get the full file path
      fileName = Path.Combine(directory, fileName);

      string fileToTest = fileName;
      int suffix = 2;
      while(File.Exists(fileToTest+extension))
      {
        fileToTest = fileName + suffix.ToString(CultureInfo.InvariantCulture);
        suffix++;
      }

      return fileToTest + extension;
    }

    /// <summary>Given a file path and the type of link from which the file was created, returns an extension to be
    /// used for resources associated with the path.
    /// </summary>
    string GetExtension(string path, string mimeType, LinkType type)
    {
      string extension = string.IsNullOrEmpty(mimeType) ? null : crawler.GetExtension(mimeType);

      if(string.IsNullOrEmpty(extension))
      {
        extension = Path.GetExtension(path);
        if(type == LinkType.Link)
        {
          if(string.IsNullOrEmpty(extension))
          {
            extension = ".html";
          }
          else
          {
            switch(crawler.GuessResourceType(extension)) // normalize the extensions of known resource types
            {
              case ResourceType.Css: extension = ".css"; break;
              case ResourceType.Html: extension = ".html"; break; // convert .asp, .jsp, .aspx, etc. to .html
              case ResourceType.Javascript: extension = ".js"; break;
              case ResourceType.VBScript: extension = ".vb"; break;
            }
          }
        }
      }

      return extension;
    }

    /// <summary>Given a path, returns the same path with the extension stripped from the filename.</summary>
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

    /// <summary>Creates a unique directory name for the service and stores it in <see cref="baseDir"/>.</summary>
    void InitializeBaseDirectory()
    {
      string dirName = baseUri.Host.ToLowerInvariant();
      if(!string.Equals(baseUri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal)) dirName += "_"+baseUri.Scheme;
      if(!baseUri.IsDefaultPort) dirName += "_"+baseUri.Port.ToString(CultureInfo.InvariantCulture);
      baseDir = Path.Combine(crawler.BaseDirectory, dirName);
    }

    /// <summary>Given a URI, returns a key that uniquely represents the URI.</summary>
    string MakeKey(Uri uri)
    {
      string key = uri.AbsolutePath;
      if(!crawler.CaseSensitivePaths) key = key.ToLowerInvariant();
      if(!string.IsNullOrEmpty(uri.Query)) key += uri.Query;
      return key;
    }

    /// <summary>Converts an integer into its hex representation.</summary>
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

    /// <summary>Given an encoded string from a URI, decodes the string and converts characters not allowed by the
    /// filesystem into other characters, and returns the decoded string.
    /// </summary>
    static string UrlSafeDecode(string encodedFile)
    {
      string unencodedFile = HttpUtility.UrlDecode(encodedFile);

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

    /// <summary>The crawler that is using this service.</summary>
    readonly Crawler crawler;
    /// <summary>The base URI of the service.</summary>
    readonly Uri baseUri;
    /// <summary>The queue of resources to be downloaded from this service.</summary>
    readonly CircularList<Resource> resources = new CircularList<Resource>();
    /// <summary>A dictionary mapping URL paths to the file info structure that holds the filenames associated with the
    /// URL.
    /// </summary>
    readonly Dictionary<string,LocalFileInfo> files = new Dictionary<string,LocalFileInfo>();
    /// <summary>A dictionary mapping URI keys to resources that exist in the queue, or to null if the resource has
    /// already been dequeued.
    /// </summary>
    readonly Dictionary<string, Resource> queued = new Dictionary<string, Resource>(StringComparer.Ordinal);
    /// <summary>The cookies for this service.</summary>
    CookieContainer cookies;
    /// <summary>The directory into which resources downloaded from this service will be saved.</summary>
    string baseDir;
    /// <summary>The current number of connections to this service.</summary>
    internal int connections;

    /// <summary>A sorted list of characters not allowed in file names.</summary>
    static readonly char[] badChars;
  }
  #endregion

  /// <summary>An enum representing the type of a link.</summary>
  internal enum LinkType : byte
  {
    /// <summary>A standard link to an unknown type of document.</summary>
    Link,
    /// <summary>A link to a supporting resource that contains no links of its own.</summary>
    Resource
  }

  /// <summary>An enum representing the status of a resource regarding link rewriting.</summary>
  internal enum RewriteStatus : byte
  {
    /// <summary>The rewriting process for the resource has not yet begun.</summary>
    None,
    /// <summary>The resource has been added to the rewrite list and is awaiting being rewritten to its original URL.</summary>
    RewriteToUrl,
    /// <summary>The resource has been added to the rewrite list and is awaiting being rewritten to its local path.</summary>
    RewriteToFile,
    /// <summary>The resource has finished being rewritten.</summary>
    Rewritten
  }

  /// <summary>Gets whether there is currently some crawl-related work to be done, such as downloading resources or
  /// rewriting links.
  /// </summary>
  bool HasWorkToDo
  {
    get { return CurrentDownloadCount != 0 || rewriteThread != null && resourcesToRewrite.Count != 0; }
  }

  /// <summary>Adds the given resource to the rewrite queue, if it does not seem to have been added already and
  /// link rewriting is enabled.
  /// </summary>
  void AddToRewriteQueue(Resource resource, RewriteStatus status)
  {
    if(RewriteLinks)
    {
      lock(resource)
      {
        bool shouldEnqueue =
          resource.rewriteStatus != RewriteStatus.RewriteToFile && resource.rewriteStatus != RewriteStatus.RewriteToUrl;
        resource.rewriteStatus = status;

        if(shouldEnqueue)
        {
          lock(resourcesToRewrite)
          {
            resourcesToRewrite.Enqueue(resource);
            rewriteEvent.Set();
          }
        }
      }
    }
  }

  /// <summary>Throws an exception if the crawler is running. This is meant to be called from property setters that
  /// cannot be changed while the crawler is running.
  /// </summary>
  void AssertNotRunningForSetter()
  {
    if(!IsStopped)
    {
      throw new InvalidOperationException("This property cannot be changed unless the crawler is stopped.");
    }
  }

  /// <summary>Determines whether the domain names in the two URIs are identical. This method will return true if
  /// comparing http://www.foo.com and http://sub.foo.com, because the domain names are the same, even though the
  /// subdomain names differ.
  /// </summary>
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

  /// <summary>Determines whether the host names in the two URIs are identical, taking into account URL hacks.</summary>
  bool AreSameHostName(Uri a, Uri b)
  {
    return a.HostNameType != b.HostNameType ? false :
      string.Equals(a.Host, b.Host, StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>Returns true if the top-level domains of the two URIs are identical.</summary>
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

  /// <summary>Given an input URI, attempts to clean up errors in its format, such as doubled slashes.</summary>
  static Uri CleanupInputUri(Uri uri)
  {
    // fix urls with runs of slashes in the path
    if(uri.AbsolutePath.IndexOf("//", StringComparison.Ordinal) != -1)
    {
      string path = uri.AbsolutePath;
      int lastLength;
      do // repeatedly replace double slashes with single slashes (so that //// becomes // and then /)
      {
        lastLength = path.Length;
        path = path.Replace("//", "/");
      } while(lastLength != path.Length);

      uri = new Uri(uri.GetLeftPart(UriPartial.Authority) + path + uri.Query + uri.Fragment);
    }

    return uri;
  }

  /// <summary>Assigns new connection threads to the service, if possible, and starts them.</summary>
  void CrawlService(Service service)
  {
    if(service == null) throw new ArgumentNullException();

    if(currentActiveThreads >= MaxConnections || service.CurrentConnections >= MaxConnectionsPerServer ||
       service.IsQueueEmpty)
    {
      return;
    }

    lock(threads)
    {
      int idleThread;
      for(idleThread=0; idleThread<threads.Count; idleThread++) // find the first idle thread
      {
        if(threads[idleThread].IsIdle) break;
      }

      while(currentActiveThreads < MaxConnections && service.CurrentConnections < MaxConnectionsPerServer &&
            !service.IsQueueEmpty && idleThread < MaxConnections)
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
          for(idleThread++; idleThread<threads.Count; idleThread++) // advance to the next idle thread
          {
            if(threads[idleThread].IsIdle) break;
          }
        }

        StartThread(thread, service);
      }
    }
  }

  /// <summary>Assigns connection threads to all known services.</summary>
  void CrawlServices()
  {
    Debug.Assert(running);
    lock(services)
    {
      foreach(Service service in services.Values)
      {
        CrawlService(service);
      }
    }
  }

  /// <summary>Compares the directories of two URIs. Returns <see cref="DirectoryNavigation.Same"/> if they are the
  /// same, <see cref="DirectoryNavigation.Up"/> if <paramref name="uri"/>'s directory is above
  /// <paramref name="baseUri"/>, <see cref="DirectoryNavigation.Down"/> if <paramref name="uri"/>'s directory is below
  /// <paramref name="baseUri"/>, and <see cref="DirectoryNavigation.UpAndDown"/> otherwise.
  /// </summary>
  DirectoryNavigation CompareDirectories(Uri uri, Uri baseUri)
  {
    string[] segments = uri.Segments, baseSegments = baseUri.Segments;

    // ignore trailing items that look like filenames, because we're only interested in directories
    int segmentsLength = segments.Length;
    if(segmentsLength != 0 && !segments[segmentsLength-1].EndsWith("/", StringComparison.Ordinal)) segmentsLength--;
    int baseLength = baseSegments.Length;
    if(baseLength != 0 && !baseSegments[baseLength-1].EndsWith("/", StringComparison.Ordinal)) baseLength--;

    DirectoryNavigation potentialType = segmentsLength < baseLength ? DirectoryNavigation.Up :
                                        segmentsLength > baseLength ? DirectoryNavigation.Down :
                                        DirectoryNavigation.Same;
    StringComparison pathComparison =
      CaseSensitivePaths ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
    for(int i=0,end=Math.Min(segmentsLength, baseLength); i<end; i++)
    {
      if(!string.Equals(segments[i], baseSegments[i], pathComparison)) return DirectoryNavigation.UpAndDown;
    }
    return potentialType;
  }

  /// <summary>Deinitializes the crawler and disposes it.</summary>
  void Dispose(bool finalizing)
  {
    Deinitialize(0);
    disposed = true;
  }

  /// <summary>Filters the content of a downloaded resource through the user-supplied filters.</summary>
  void DoFilterContent(Uri url, string mimeType, ResourceType resourceType, Encoding encoding, string fileName)
  {
    if(FilterContent != null)
    {
      FilterContent(url, mimeType, resourceType, encoding, fileName);
    }
  }

  /// <summary>Attempts to enqueue the given URI, given its referrer, depth, and link type.</summary>
  /// <returns>Returns the resource if the URI was enqueued, and null if not.</returns>
  Resource EnqueueUri(Uri uri, string postData, Uri referrer, int depth, LinkType type, bool force)
  {
    bool external;
    if(IsUriAllowed(uri, type, out external))
    {
      Resource resource = new Resource(uri, postData, referrer, depth, type);
      Service service = GetService(uri);
      resource = service.Enqueue(resource, force);
      if(resource != null && running) CrawlService(service);
      return resource;
    }
    else
    {
      return null;
    }
  }

  /// <summary>Filters a URI through the user-supplied filters.</summary>
  Uri FilterUri(Uri uri)
  {
    if(FilterUris != null && uri != null)
    {
      try { uri = FilterUris(uri); }
      catch { }
    }
    return uri;
  }

  /// <summary>Given a MIME type, returns the file extension commonly associated with it, including the leading period,
  /// or null if the extension is unknown.
  /// </summary>
  string GetExtension(string mimeType)
  {
    mimeType = mimeType.ToLowerInvariant();

    string extension;
    lock(extensionToMime) mimeToExtension.TryGetValue(mimeType, out extension);
    return extension;
  }

  /// <summary>Allocates and returns the next rewrite tag.</summary>
  uint GetNextRewriteTag()
  {
    uint tag = (uint)Interlocked.Increment(ref rewriteTag);
    if(tag == 0) tag = (uint)Interlocked.Increment(ref rewriteTag); // deal with overflow (but only partially...)
    return tag;
  }

  /// <summary>Gets the service that serves the given URI.</summary>
  Service GetService(Uri uri)
  {
    Service service;
    string key = GetServiceKey(uri);
    lock(services)
    {
      if(!services.TryGetValue(key, out service))
      {
        services[key] = service = new Service(this, uri);
      }
    }
    return service;
  }

  /// <summary>Gets a key that uniquely represents the service for the given URI.</summary>
  string GetServiceKey(Uri uri)
  {
    return uri.Scheme + "_" + uri.Authority.ToLowerInvariant();
  }

  /// <summary>Given the absolute URI of a downloaded resource, and the directory in which resource's referrer was
  /// saved, returns the path to the resource, relative to its referrer.
  /// </summary>
  string GetRelativeUri(string referrerDir, string resourcePath, Uri resourceUri, LinkType type)
  {
    char[] split = new char[1] { Path.DirectorySeparatorChar };
    string[] baseBits = referrerDir.Split(split, StringSplitOptions.RemoveEmptyEntries);
    string[] newBits  = resourcePath.Split(split, StringSplitOptions.RemoveEmptyEntries);

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

    if(!string.IsNullOrEmpty(resourceUri.Query)) sb.Append(resourceUri.Query);
    if(!string.IsNullOrEmpty(resourceUri.Fragment)) sb.Append(resourceUri.Fragment);
    return sb.ToString();
  }

  /// <summary>Given a MIME type of a document, returns its resource type.</summary>
  static ResourceType GetResourceType(string mimeType)
  {
    mimeType = mimeType.ToLowerInvariant();
    if(string.Equals(mimeType, "text/html", StringComparison.Ordinal) ||
       string.Equals(mimeType, "application/xhtml+xml", StringComparison.Ordinal) ||
       string.Equals(mimeType, "text/xhtml", StringComparison.Ordinal) || // incorrect but in use
       string.Equals(mimeType, "text/xml", StringComparison.Ordinal))     // incorrect but in use
    {
      return ResourceType.Html;
    }
    else if(string.Equals(mimeType, "text/css", StringComparison.Ordinal))
    {
      return ResourceType.Css;
    }
    else if(string.Equals(mimeType, "text/javascript", StringComparison.Ordinal) ||
            string.Equals(mimeType, "application/x-javascript", StringComparison.Ordinal) ||
            string.Equals(mimeType, "text/ecmascript", StringComparison.Ordinal))
    {
      return ResourceType.Javascript;
    }
    else if(string.Equals(mimeType, "text/vbscript", StringComparison.Ordinal))
    {
      return ResourceType.VBScript;
    }
    else
    {
      return ResourceType.Binary;
    }
  }

  /// <summary>Given a resource's URI, attempts to guess the resource type based on its file extension.</summary>
  ResourceType GuessResourceType(Uri uri)
  {
    string[] segments = uri.Segments;
    if(segments.Length == 0) return ResourceType.Unknown;
    return GuessResourceType(Path.GetExtension(segments[segments.Length-1]));
  }

  /// <summary>Given a <see cref="Resource"/>, attempts to guess the resource type.</summary>
  ResourceType GuessResourceType(Resource resource)
  {
    return string.IsNullOrEmpty(resource.ContentType) ?
      GuessResourceType(resource.Uri) : GetResourceType(resource.ContentType);
  }

  /// <summary>Given a file extension, including the leading period, attempts to guess the resource type.</summary>
  ResourceType GuessResourceType(string extension)
  {
    string mimeType;

    if(string.IsNullOrEmpty(extension))
    {
      mimeType = null;
    }
    else
    {
      extension = NormalizeMimeOverrideExtension(extension);
      lock(extensionToMime) extensionToMime.TryGetValue(extension, out mimeType);
    }

    return mimeType == null ? ResourceType.Unknown : GetResourceType(mimeType);
  }

  /// <summary>Idles a connection thread. An idle thread will disassociate itself after completing the current
  /// download.
  /// </summary>
  void IdleThread(ConnectionThread thread)
  {
    thread.Stop();
  }

  /// <summary>Starts the rewrite thread.</summary>
  void InitializeRewriteThread()
  {
    if(rewriteThread == null)
    {
      rewriteThread = new Thread(RewriteThreadFunc);
      rewriteEvent  = new AutoResetEvent(true);
      rewriteThread.Start();
    }
  }

  /// <summary>Determines if the given absolute URI is allowed, and whether it is an external resource.</summary>
  bool IsUriAllowed(Uri uri, LinkType type, out bool isExternal)
  {
    isExternal = false;

    if(!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal) && // only accept these three protocols
       !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal) &&
       !string.Equals(uri.Scheme, Uri.UriSchemeFtp, StringComparison.Ordinal))
    {
      return false;
    }

    // skip non-html files if we aren't downloading them
    ResourceType resourceType = type != LinkType.Link ? ResourceType.Binary : GuessResourceType(uri);
    if(resourceType == ResourceType.Binary && (Download & DownloadFlags.NonHtml) == 0)
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
            DirectoryNavigation comparison = CompareDirectories(uri, baseUri);
            if(dirNav != comparison && comparison != DirectoryNavigation.Same) continue;
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

    // it's external, but if it's a resource of an internal file, then it's OK
    return type != LinkType.Link && (download & DownloadFlags.ExternalResources) != 0;
  }

  /// <summary>Issues a progress report for the given error.</summary>
  void OnErrorOccurred(Resource resource, Exception ex, bool isFatal)
  {
    OnProgress(resource, isFatal ? Status.FatalErrorOccurred : Status.NonFatalErrorOccurred,
               ex.Message);
  }

  /// <summary>Issues a progress report for the given resource, given its new status.</summary>
  void OnProgress(Resource resource, Status newStatus)
  {
    OnProgress(resource, newStatus, null);
  }

  /// <summary>Sets the status of the resource and issues a new progress report about it.</summary>
  void OnProgress(Resource resource, Status newStatus, string message)
  {
    resource.status = newStatus;
    if(Progress != null)
    {
      try { Progress(resource, message); }
      catch { }
    }
  }

  void OnResourceTransferred(Resource resource)
  {
    Interlocked.Increment(ref resourcesDownloaded);
    Interlocked.Add(ref bytesDownloaded, resource.bytesDownloaded);
  }

  /// <summary>Called by a connection thread when it has run out of resources to download from its associated service.</summary>
  /// <remarks>This method will attempt to find a new service to associate the thread with.</remarks>
  void OnThreadIdle(ConnectionThread thread)
  {
    if(!running || currentActiveThreads >= MaxConnections)
    {
      IdleThread(thread); // if we're not creating any new connections, just stop the thread
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

        IdleThread(thread); // if we couldn't find a service to run, stop the thread
      }
    }
  }

  void RewriteThreadFunc()
  {
    while(!terminating)
    {
      rewriteEvent.WaitOne();

      while(resourcesToRewrite.Count != 0)
      {
        Resource resource = null;
        lock(resourcesToRewrite)
        {
          if(resourcesToRewrite.Count != 0) resource = resourcesToRewrite.Dequeue();
        }

        if(resource != null)
        {
          if(resource.rewriteStatus == RewriteStatus.RewriteToFile) RewriteToLocalPath(resource);
          else if(resource.rewriteStatus == RewriteStatus.RewriteToUrl) RewriteToOriginalUrl(resource);
        }
      }
    }
  }

  void RewriteToOriginalUrl(Resource resource)
  {
    lock(resource)
    {
      if(resource.references != null)
      {
        // omit the fragment from the URL, because it's already been added to the file
        string placeHolder = resource.PlaceholderUrl, origUrl = resource.Uri.GetLeftPart(UriPartial.Query);
        foreach(Resource reference in resource.references)
        {
          lock(reference) RewriteUrlInFile(reference.LocalPath, reference.encoding, placeHolder, origUrl);
        }
        resource.references = null;
      }

      resource.rewriteStatus = RewriteStatus.Rewritten;
    }
  }

  void RewriteToLocalPath(Resource resource)
  {
    lock(resource)
    {
      if(resource.references != null)
      {
        string placeHolder = resource.PlaceholderUrl;
        foreach(Resource reference in resource.references)
        {
          lock(reference)
          {
            RewriteUrlInFile(reference.LocalPath, reference.encoding, placeHolder,
                             GetRelativeUri(Path.GetDirectoryName(reference.LocalPath), resource.LocalPath,
                                            resource.Uri, resource.type));
          }
        }
        resource.references = null;
      }

      resource.rewriteStatus = RewriteStatus.Rewritten;
    }
  }

  static void RewriteUrlInFile(string file, Encoding encoding, string placeholder, string newUrl)
  {
    string content;
    using(StreamReader reader = new StreamReader(file, encoding)) content = reader.ReadToEnd();
    content = content.Replace(placeholder, newUrl);
    using(StreamWriter writer = new StreamWriter(file, false, encoding)) writer.Write(content);
  }

  /// <summary>Given a service point, sets its properties based on the configuration of the crawler.</summary>
  void SetServicePointProperties(ServicePoint point)
  {
    point.MaxIdleTime     = ConnectionIdleTimeout * 1000;
    point.ConnectionLimit = MaxConnectionsPerServer;
  }

  /// <summary>Given a thread and a service, associates the thread with the service and starts downloading resources
  /// from it.
  /// </summary>
  void StartThread(ConnectionThread thread, Service service)
  {
    thread.Start(service);
  }

  /// <summary>Given a URI and post data for the URI, ensures that the URI can be handled by the crawler.</summary>
  static void ValidateNewUri(Uri uri, string postData)
  {
    if(uri == null) throw new ArgumentNullException();
    if(!uri.IsAbsoluteUri) throw new ArgumentNullException("The uri must be absolute.");

    if(!string.Equals(uri.Scheme, "http", StringComparison.Ordinal) &&
       !string.Equals(uri.Scheme, "https", StringComparison.Ordinal))
    {
      if(postData != null) throw new ArgumentException("The URI scheme "+uri.Scheme+" does not support post data.");

      if(!string.Equals(uri.Scheme, "ftp", StringComparison.Ordinal))
      {
        throw new NotImplementedException("Unsupported URI scheme: " + uri.Scheme);
      }
    }
  }

  /// <summary>Determines whether the given resource type is one that the crawler is interested in downloading.</summary>
  bool WantResource(ResourceType resourceType)
  {
    if(resourceType == ResourceType.Unknown) throw new ArgumentException();
    DownloadFlags flags = resourceType == ResourceType.Binary ? DownloadFlags.NonHtml : DownloadFlags.Html;
    return (Download & flags) != 0;
  }

  long maxSize = 50*1024*1024, bytesDownloaded;
  /// <summary>A dictionary mapping file extensions to the MIME types that are assumed for resources with the
  /// extension.
  /// </summary>
  readonly Dictionary<string,string> extensionToMime = new Dictionary<string,string>();
  /// <summary>A dictionary mapping MIME types to their most common file extension.</summary>
  readonly Dictionary<string,string> mimeToExtension = new Dictionary<string,string>();
  /// <summary>A dictionary mapping service keys to the corresponding services.</summary>
  readonly Dictionary<string,Service> services = new Dictionary<string,Service>();
  /// <summary>A list of base URIs that have been added to the crawler.</summary>
  readonly List<Uri> baseUris = new List<Uri>();
  /// <summary>A list of connection threads.</summary>
  readonly List<ConnectionThread> threads = new List<ConnectionThread>();
  /// <summary>The user agent string reported to the web server, or null to report no user agent.</summary>
  string userAgent = "Mozilla/4.5 (compatible; AdamMil 1.0; Windows XP)";
  /// <summary>The preferred language reported to the web server, or null to report no preference.</summary>
  string language;
  /// <summary>The base directory into which all downloaded resources will be saved.</summary>
  string baseDir;
  /// <summary>The referrer sent to the web server for root URIs that have no referrers.</summary>
  Uri defaultReferrer;
  /// <summary>The thread that handles rewriting links in downloaded resources.</summary>
  Thread rewriteThread;
  /// <summary>An event to wake up the rewrite thread.</summary>
  AutoResetEvent rewriteEvent;
  /// <summary>A list of resources that are awaiting being rewritten.</summary>
  readonly Queue<Resource> resourcesToRewrite = new Queue<Resource>();
  int idleTimeout = 30, connsPerServer = 2, maxConnections = 10, depthLimit = 50, retries = 1,
      maxQueuedLinks = Infinite, ioTimeout = 60, transferTimeout = 5*60, maxRedirects = 20, currentActiveThreads,
      maxQueryStrings = 500, rewriteTag, resourcesDownloaded;
  /// <summary>The allowed directory navigation.</summary>
  DirectoryNavigation dirNav = DirectoryNavigation.Down;
  /// <summary>The allowed domain navigation.</summary>
  DomainNavigation domainNav = DomainNavigation.SameHostName;
  /// <summary>The types of resources the crawler is interested in downloading, and their priorities.</summary>
  DownloadFlags download = DownloadFlags.Everything | DownloadFlags.ExternalResources;
  bool rewriteLinks = true, useCookies = true, errorFiles = true, passiveFtp = true, caseSensitive=true, disposed,
       running, baseDirInitialized, terminating;

  /// <summary>Matches the domain name (not including subdomains) in a host name.</summary>
  static readonly Regex domainRe = new Regex(@"[\w-]+\.[\w]+$", RegexOptions.Compiled | RegexOptions.Singleline);
  /// <summary>Matches the top-level domain in a host name.</summary>
  static readonly Regex tldRe = new Regex(@"(?<=\.)[\w]+$", RegexOptions.Compiled | RegexOptions.Singleline);
}
#endregion

#region UrlFilters
/// <summary>Provides some common URL filters that can be used.</summary>
public static class UrlFilters
{
  public static Uri NormalizeQuery(Uri uri)
  {
    if(uri == null) throw new ArgumentNullException();

    if(!string.IsNullOrEmpty(uri.Query))
    {
      Match m = queryRe.Match(uri.Query);
      if(m.Success)
      {
        // sort the key/value pairs in the query string
        List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();

        CaptureCollection keys = m.Groups["key"].Captures, values = m.Groups["value"].Captures;
        int stringLength = 0;
        for(int i=0; i<keys.Count; i++)
        {
          KeyValuePair<string, string> pair = new KeyValuePair<string, string>(keys[i].Value, values[i].Value);
          pairs.Add(pair);
          stringLength += pair.Key.Length + pair.Value.Length + 1; // +1 for '=' sign
          if(i != 0) stringLength++; // +1  for '&' sign
        }

        pairs.Sort(QueryKeyComparer.Instance);

        StringBuilder sb = new StringBuilder(stringLength+1);
        sb.Append('?');

        bool sep = false;
        foreach(KeyValuePair<string, string> pair in pairs)
        {
          if(sep) sb.Append('&');
          else sep = true;
          sb.Append(pair.Key).Append('=').Append(pair.Value);
        }

        uri = new Uri(uri.GetLeftPart(UriPartial.Path) + sb.ToString() + uri.Fragment);
      }
    }

    return uri;
  }

  public static Uri StripWWWPrefix(Uri uri)
  {
    if(uri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
    {
      uri = new Uri(uri.GetLeftPart(UriPartial.Scheme) + uri.Authority.Substring(4) +
                    uri.AbsolutePath + uri.Query + uri.Fragment);
    }
    return uri;
  }

  /// <summary>A comparer that sorts key/value pairs by their keys.</summary>
  sealed class QueryKeyComparer : IComparer<KeyValuePair<string, string>>
  {
    QueryKeyComparer() { }

    public int Compare(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
    {
      return string.CompareOrdinal(a.Key, b.Key);
    }

    public readonly static QueryKeyComparer Instance = new QueryKeyComparer();
  }

  /// <summary>A regular expression to parse query string keys and values.</summary>
  static readonly Regex queryRe = new Regex(@"^\?(?<key>[\w\-/.!~*'()]+)=(?<value>[\w\-/.!~*'()]*)(?:&(?<key>[\w\-/.!~*'()]+)=(?<value>[\w\-/.!~*'()]*))*&?$",
                                            RegexOptions.Compiled | RegexOptions.ECMAScript);
}
#endregion

} // namespace WebCrawl.Backend