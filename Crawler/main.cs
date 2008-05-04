using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using WebCrawl.Backend;

namespace WebCrawl
{

[Flags]
enum ProgressMask
{
  None=0, UrlQueued=1<<(int)Status.UrlQueued, DownloadStarted=1<<(int)Status.DownloadStarted,
  DownloadFinished=1<<(int)Status.DownloadFinished, NonFatalErrorOccurred=1<<(int)Status.NonFatalErrorOccurred,
  FatalErrorOccurred=1<<(int)Status.FatalErrorOccurred,
  AnyErrorOccurred=NonFatalErrorOccurred | FatalErrorOccurred,
  All=UrlQueued|DownloadStarted|DownloadFinished|AnyErrorOccurred
}

static class App
{
  const RegexOptions UriRegexOptions =
    RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

  static readonly Crawler crawl = new Crawler();
  static ProgressMask progressMask;
  static bool urlHacks;

  static App()
  {
    propertyMap = new Dictionary<string,string>(21);
    propertyMap["casesens"]        = "CaseSensitivePaths";
    propertyMap["idletimeout"]     = "ConnectionIdleTimeout";
    propertyMap["referrer"]        = "DefaultReferrer";
    propertyMap["dirnav"]          = "DirectoryNavigation";
    propertyMap["domainnav"]       = "DomainNavigation";
    propertyMap["download"]        = "Download";
    propertyMap["errorfiles"]      = "GenerateErrorFiles";
    propertyMap["maxconns"]        = "MaxConnections";
    propertyMap["connsperserver"]  = "MaxConnectionsPerServer";
    propertyMap["depthlimit"]      = "DepthLimit";
    propertyMap["maxsize"]         = "MaxFileSize";
    propertyMap["maxqueries"]      = "MaxQueryStringsPerFile";
    propertyMap["maxqueued"]       = "MaxQueuedLinks";
    propertyMap["maxredirects"]    = "MaxRedirects";
    propertyMap["maxretries"]      = "MaxRetries";
    propertyMap["passiveftp"]      = "PassiveFtp";
    propertyMap["language"]        = "PreferredLanguage";
    propertyMap["rewritelinks"]    = "RewriteLinks";
    propertyMap["readtimeout"]     = "ReadTimeout";
    propertyMap["transfertimeout"] = "TransferTimeout";
    propertyMap["cookies"]         = "UseCookies";
    propertyMap["useragent"]       = "UserAgent";
  }

  static void Main()
  {
    Console.WriteLine("Crawler v. 0.71 copyright Adam Milazzo 2006-2007");

    progressMask = ProgressMask.DownloadStarted | ProgressMask.AnyErrorOccurred;

    crawl.AddStandardMimeOverrides();
    crawl.Progress += new ProgressHandler(crawl_Progress);
    crawl.FilterUris += crawl_FilterUris;

    while(true)
    {
      try
      {
        Console.Write("> ");
        string line = Console.ReadLine();
        if(string.IsNullOrEmpty(line) || line[0] == '#') continue;
        
        Match match = cmdRe.Match(line);
        if(!match.Success)
        {
          Console.WriteLine("Unrecognized command '"+line+"'");
          continue;
        }
        
        string parameters = match.Groups[2].Value;
        switch(match.Groups[1].Value.ToLowerInvariant())
        {
          case "addbase":
          {
            Uri uri = new Uri(parameters);
            if(uri != null) crawl.AddBaseUri(uri, true);
            break;
          }

          case "add":
          {
            Uri uri = new Uri(parameters);
            if(uri != null) crawl.EnqueueUri(uri);
            break;
          }

          case "clearfilters":
            ClearFilters();
            break;

          case "filter":
            if(string.IsNullOrEmpty(parameters) ||
               parameters[0] != '-' && parameters[0] != '+' && parameters[0] != '*')
            {
              Console.WriteLine("Usage: filter [+|-]<uriRegex>");
              Console.WriteLine("       filter *<uriRegex> <replacement>");
            }
            else
            {
              AddFilter(parameters);
            }
            break;

          case "remove":
            if(string.IsNullOrEmpty(parameters)) Console.WriteLine("Usage: remove <uriRegex>");
            else crawl.RemoveUris(new Regex(parameters, UriRegexOptions), false);
            break;

          case "setup":
            if(string.IsNullOrEmpty(parameters))
            {
              Console.WriteLine("Usage: setup <outputDirectory>");
            }
            else
            {
              crawl.BaseDirectory = parameters;
              Console.WriteLine("Crawler initialized. Add base URIs with 'addbase' and start it with 'start'.");
            }
            break;
          
          case "start":
            if(VerifyInitialized())
            {
              crawl.Start();
              Console.WriteLine("Crawler is running.");
            }
            break;
          
          case "stop":
            if(VerifyInitialized())
            {
              crawl.Stop();
              Console.WriteLine("Crawler is stopped, but current downloads are allowed to finish.");
            }
            break;
          
          case "kill":
            if(VerifyInitialized())
            {
              crawl.Terminate(0);
              Console.WriteLine("Crawler has been terminated. All downloads stopped.");
            }
            break;
          
          case "teardown":
            crawl.Deinitialize();
            Console.WriteLine("Crawler has been terminated and deinitialized.");
            break;

          case "stats":
          {
            Console.Write(string.Format("{0} current connections\n{1} queued links\n{2}kps download rate\n",
                                        crawl.CurrentDownloadCount, crawl.CurrentLinksQueued,
                                        Math.Round(crawl.CurrentBytesPerSecond/1024.0, 1)));
            Download[] resources = crawl.GetCurrentDownloads();
            if(resources.Length != 0)
            {
              Console.WriteLine();
              Console.WriteLine("URIs being downloaded:");
              foreach(Download download in resources) Console.WriteLine(download.Resource.Uri.AbsoluteUri);
            }
            break;
          }

          case "get":
            Console.Write(string.Format(
              "Base directory = {0}\nCase sensitive = {1}\nIdle timeout = {2}\nDefault referrer = {3}\n"+
              "Directory navigation = {4}\nDomain navigation = {5}\nDownload filter = {6}\nUrl hacks enabled = {7}\n"+
              "Generate error files = {8}\nMax. connections = {10}\n"+
              "Max. connections per server = {11}\nMax. crawl depth = {12}\nMax. file size = {25}\n"+
              "Max. query strings per file = {13}\n"+
              "Max. queued links = {14}\nMax. redirects = {15}\nMax. retries = {16}\nPassive ftp = {17}\n"+
              "Preferred language = {18}\nProgress notifications = {19}\nLink rewriting = {20}\n"+
              "Read timeout = {21}\nTransfer timeout = {22}\nEnable cookies = {23}\nUser agent = {24}\n",
              crawl.BaseDirectory, crawl.CaseSensitivePaths, crawl.ConnectionIdleTimeout, crawl.DefaultReferrer,
              crawl.DirectoryNavigation, crawl.DomainNavigation, crawl.Download, urlHacks,
              crawl.GenerateErrorFiles, null, crawl.MaxConnections, crawl.MaxConnectionsPerServer,
              GetMax(crawl.DepthLimit), GetMax(crawl.MaxQueryStringsPerFile), GetMax(crawl.MaxQueuedLinks),
              crawl.MaxRedirects, GetMax(crawl.MaxRetries), crawl.PassiveFtp, crawl.PreferredLanguage,
              progressMask, crawl.RewriteLinks, GetMax(crawl.ReadTimeout), GetMax(crawl.TransferTimeout),
              crawl.UseCookies, crawl.UserAgent, GetMax(crawl.MaxFileSize)));
            break;

          case "set":
          {
            string[] words = parameters.Split(null, 2);
            if(words.Length == 1)
            {
              Console.WriteLine("Usage: set PROPERTY VALUE");
            }
            if(words.Length < 2 || !SetProperty(words[0], words[1]))
            {
              Console.Write("Available options:\n"+
                            "  caseSens, idleTimeout, referrer, dirNav, domainNav, download, urlHacks,\n"+
                            "  errorFiles, maxConnections, connsPerServer, maxDepth, maxSize, maxQueries,\n"+
                            "  maxQueued, maxRedirects, maxRetries, passiveFtp, language, progress,\n"+
                            "  rewriteLinks, readTimeout, transferTimeout, cookies, userAgent\n");
            }
            break;
          }

          case "quit": case "exit":
            if(crawl.CurrentDownloadCount != 0)
            {
              Console.WriteLine("Crawler is still downloading files. Deinitialize the crawler first.");
              break;
            }
            else
            {
              goto done;
            }
        
          default:
            Console.WriteLine("Unrecognized command '"+match.Groups[1].Value+"'");
            break;
        }
      }
      catch(ArgumentException ex)
      {
        Console.WriteLine("ERROR: "+ex.Message);
      }
      catch(Exception ex)
      {
        Console.WriteLine("ERROR OCCURRED:\n"+ex.ToString());
      }
    }
    
    done:;
  }

  static void AddFilter(string filter)
  {
    Regex regex;
    string replacement = null;

    if(filter[0] == '*')
    {
      string[] bits = filter.Substring(1).Split();
      if(bits.Length != 2) throw new ArgumentException("Invalid change filter.");
      regex = new Regex(bits[0], UriRegexOptions & ~RegexOptions.ExplicitCapture | RegexOptions.Compiled);
      replacement = bits[1];
    }
    else
    {
      regex = new Regex(filter.Substring(1), UriRegexOptions | RegexOptions.Compiled);
    }
    
    if(filter[0] == '+')
    {
      if(positiveFilters == null) positiveFilters = new List<Regex>();
      positiveFilters.Add(regex);
    }
    else if(filter[0] == '-')
    {
      if(negativeFilters == null) negativeFilters = new List<Regex>();
      negativeFilters.Add(regex);
    }
    else if(filter[0] == '*')
    {
      if(changeFilters == null) changeFilters = new List<ChangeFilter>();
      changeFilters.Add(new ChangeFilter(regex, replacement));
    }
  }

  static void ClearFilters()
  {
    if(positiveFilters != null || negativeFilters != null || changeFilters != null)
    {
      positiveFilters = negativeFilters = null;
      changeFilters = null;
    }
  }

  static Uri crawl_FilterUris(Uri uri)
  {
    string uriString = uri.ToString();

    if(changeFilters != null)
    {
      bool uriChanged = false;
      for(int i=0; i<changeFilters.Count; i++)
      {
        Match uriMatch = changeFilters[i].Regex.Match(uriString);
        if(uriMatch.Success)
        {
          uriString = varRe.Replace(changeFilters[i].Replacement,
                                    delegate(Match m)
                                    { return uriMatch.Groups[int.Parse(m.Groups["group"].Value)].Value; });
          uriChanged = true;
        }
      }
      if(uriChanged) uri = new Uri(uriString);
    }

    if(positiveFilters != null)
    {
      for(int i=0; i<positiveFilters.Count; i++) if(!positiveFilters[i].IsMatch(uriString)) return null;
    }
    
    if(negativeFilters != null)
    {
      for(int i=0; i<negativeFilters.Count; i++) if(negativeFilters[i].IsMatch(uriString)) return null;
    }

    if(urlHacks) uri = UrlFilters.NormalizeQuery(UrlFilters.StripWWWPrefix(uri));

    return uri;
  }

  static void crawl_Progress(Resource resource, string message)
  {
    if((progressMask & (ProgressMask)(1<<(int)resource.Status)) == 0) return;
    
    string prefix, suffix = null;
    switch(resource.Status)
    {
      case Status.DownloadStarted: prefix = ". "; break;
      case Status.DownloadFinished: prefix = "- "; break;
      case Status.NonFatalErrorOccurred:
        prefix = "? ";
        suffix = " ("+resource.ResponseCode+" - "+message+")";
        break;
      case Status.FatalErrorOccurred:
        prefix = "! ";
        suffix = " ('"+message+"' from "+resource.Referrer.ToString()+")";
        break;
      case Status.UrlQueued:
        prefix = "+["+resource.Depth+"] ";
        suffix = " (from "+resource.Referrer+")";
        break;
      default: prefix = "UNKNOWN "; break;
    }

    Console.WriteLine(prefix + resource.Uri.PathAndQuery + suffix);
  }
  
  static object GetMax(long max)
  {
    return max == Crawler.Infinite ? "undefined" : (object)max;
  }

  static bool SetProperty(string property, string value)
  {
    property = property.ToLowerInvariant();
    if(property == "progress")
    {
      progressMask = (ProgressMask)Enum.Parse(typeof(ProgressMask), value);
      return true;
    }
    else if(property == "urlhacks")
    {
      urlHacks = (bool)Convert.ChangeType(value, typeof(bool));
      return true;
    }

    string propertyName;
    if(!propertyMap.TryGetValue(property, out propertyName)) return false;

    PropertyInfo prop = crawl.GetType().GetProperty(propertyName);
    MethodInfo setter = prop.GetSetMethod();
    
    object typedValue = prop.PropertyType.IsEnum ?
      Enum.Parse(prop.PropertyType, value, true) : Convert.ChangeType(value, prop.PropertyType);
    setter.Invoke(crawl, new object[] { typedValue });
    return true;
  }

  static bool VerifyInitialized()
  {
    if(!string.IsNullOrEmpty(crawl.BaseDirectory))
    {
      return true;
    }
    else
    {
      Console.WriteLine("Crawler is not initialized. Use the 'setup' command first.");
      return false;
    }
  }

  struct ChangeFilter
  {
    public ChangeFilter(Regex regex, string replacement) { Regex = regex; Replacement = replacement; }
    public Regex Regex;
    public string Replacement;
  }
  
  static readonly Dictionary<string,string> propertyMap;
  static List<Regex> positiveFilters, negativeFilters;
  static List<ChangeFilter> changeFilters;

  static readonly Regex cmdRe = new Regex(@"^\s*(\w+)\s*(.*?)\s*$", RegexOptions.Singleline);
  static readonly Regex varRe = new Regex(@"\$(?:\{(?<group>\d+)\}|(?<group>\d+))",
                                          RegexOptions.Singleline|RegexOptions.Compiled);
}

} // namespace Crawler
