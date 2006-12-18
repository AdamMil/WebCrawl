using System;
using System.Collections.Generic;
using System.Reflection;
using WebCrawl.Backend;

namespace WebCrawl
{

static class App
{
  static readonly Crawler crawl = new Crawler();

  static App()
  {
    propertyMap = new Dictionary<string,string>(21);
    propertyMap["casesens"]        = "CaseSensitivePaths";
    propertyMap["idletimeout"]     = "ConnectionIdleTimeout";
    propertyMap["referrer"]        = "DefaultReferrer";
    propertyMap["dirnav"]          = "DirectoryNavigation";
    propertyMap["domainnav"]       = "DomainNavigation";
    propertyMap["download"]        = "Download";
    propertyMap["urlhacks"]        = "EnableUrlHacks";
    propertyMap["errorfiles"]      = "GenerateFilesOnError";
    propertyMap["maxconns"]        = "MaxConnections";
    propertyMap["connsperserver"]  = "MaxConnectionsPerServer";
    propertyMap["maxdepth"]        = "MaxDepth";
    propertyMap["maxqueries"]      = "MaxQueryStringsPerFile";
    propertyMap["maxqueued"]       = "MaxQueuedLinks";
    propertyMap["maxredirects"]    = "MaxRedirects";
    propertyMap["maxretries"]      = "MaxRetries";
    propertyMap["passiveftp"]      = "PassiveFtp";
    propertyMap["language"]        = "PreferredLanguage";
    propertyMap["progress"]        = "ProgressFilter";
    propertyMap["rewritelinks"]    = "RewriteLinks";
    propertyMap["readtimeout"]     = "ReadTimeout";
    propertyMap["transfertimeout"] = "TransferTimeout";
    propertyMap["cookies"]         = "UseCookies";
    propertyMap["useragent"]       = "UserAgent";
  }

  static void Main()
  {
    Console.WriteLine("Crawler v. 0.5 copyright Adam Milazzo 2006");

    crawl.AddStandardMimeOverrides();

    crawl.ProgressFilter = ProgressType.FatalErrorOccurred;
    crawl.Progress += new ProgressHandler(crawl_Progress);

    while(true)
    {
      try
      {
        Console.Write("> ");
        string line = Console.ReadLine();
        if(line == null) continue;
        line = line.Trim();
        if(string.IsNullOrEmpty(line)) continue;
        
        string[] words = line.Split();
        switch(words[0].ToLowerInvariant())
        {
          case "addbase":
          {
            Uri uri = GetUri("Base uri");
            if(uri != null) crawl.AddBaseUri(uri, true);
            break;
          }

          case "add":
          {
            Uri uri = GetUri("Uri");
            if(uri != null) crawl.EnqueueUri(uri);
            break;
          }

          case "setup":
          {
            string value = GetInput("Base output directory");
            if(value != null)
            {
              crawl.Initialize(value);
              Console.WriteLine("Crawler initialized. Add base URIs with 'addbase' and start it with 'start'.");
            }
            break;
          }
          
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
            Console.Write(string.Format("{0} current connections\n{1} queued links\n{2}kps download rate\n",
                                        crawl.CurrentDownloadCount, crawl.CurrentLinksQueued,
                                        Math.Round(crawl.CurrentBytesPerSecond/1024.0, 1)));
            break;

          case "get":
            Console.Write(string.Format(
              "Base directory = {0}\nCase sensitive = {1}\nIdle timeout = {2}\nDefault referrer = {3}\n"+
              "Directory navigation = {4}\nDomain navigation = {5}\nDownload filter = {6}\nUrl hacks enabled = {7}\n"+
              "Generate error files = {8}\nIs inititialized = {9}\nMax. connections = {10}\n"+
              "Max. connections per server = {11}\nMax. crawl depth = {12}\nMax. query strings per file = {13}\n"+
              "Max. queued links = {14}\nMax. redirects = {15}\nMax. retries = {16}\nPassive ftp = {17}\n"+
              "Preferred language = {18}\nProgress notifications = {19}\nLink rewriting = {20}\n"+
              "Read timeout = {21}\nTransfer timeout = {22}\nEnable cookies = {23}\nUser agent = {24}\n",
              crawl.BaseDirectory, crawl.CaseSensitivePaths, crawl.ConnectionIdleTimeout, crawl.DefaultReferrer,
              crawl.DirectoryNavigation, crawl.DomainNavigation, crawl.Download, crawl.EnableUrlHacks,
              crawl.GenerateFilesOnError, crawl.IsInitialized, crawl.MaxConnections, crawl.MaxConnectionsPerServer,
              GetMax(crawl.MaxDepth), GetMax(crawl.MaxQueryStringsPerFile), GetMax(crawl.MaxQueuedLinks),
              crawl.MaxRedirects, GetMax(crawl.MaxRetries), crawl.PassiveFtp, crawl.PreferredLanguage,
              crawl.ProgressFilter, crawl.RewriteLinks, GetMax(crawl.ReadTimeout), GetMax(crawl.TransferTimeout),
              crawl.UseCookies, crawl.UserAgent));
            break;

          case "set":
            if(words.Length == 2)
            {
              Console.WriteLine("Usage: set PROPERTY VALUE");
            }
            if(words.Length < 3 || !SetProperty(words[1], string.Join(" ", words, 2, words.Length-2)))

            {
              Console.Write("Available options:\n"+
                            "  caseSens, idleTimeout, referrer, dirNav, domainNav, download, urlHacks,\n"+
                            "  errorFiles, maxConnections, connsPerServer, maxDepth, maxQueries, maxQueued,\n"+
                            "  maxRedirects, maxRetries, passiveFtp, language, progress, rewriteLinks,\n"+
                            "  readTimeout, transferTimeout, cookies, userAgent\n");
            }
            break;

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
            Console.WriteLine("Unrecognized command '"+words[0]+"'");
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

  static void crawl_Progress(Resource resource)
  {
    string prefix, suffix = null;
    switch(resource.Status)
    {
      case ProgressType.DownloadStarted: prefix = ". "; break;
      case ProgressType.DownloadFinished: prefix = "- "; break;
      case ProgressType.NonFatalErrorOccurred: prefix = "? "; break;
      case ProgressType.FatalErrorOccurred:
        prefix = "! ";
        suffix = " (from "+resource.Referrer+")";
        break;
      case ProgressType.UrlQueued:
        prefix = "+["+resource.Depth+"] ";
        suffix = " (from "+resource.Referrer+")";
        break;
      default: prefix = "UNKNOWN "; break;
    }

    Console.WriteLine(prefix + resource.Uri.PathAndQuery + suffix);
  }
  
  static object GetMax(int max)
  {
    return max == 0 ? "undefined" : (object)max;
  }

  static Uri GetUri(string prompt)
  {
    return new Uri(GetInput(prompt));
  }
  
  static string GetInput(string prompt)
  {
    Console.Write(prompt+"? ");
    prompt = Console.ReadLine();
    if(prompt != null) prompt = prompt.Trim();
    return string.IsNullOrEmpty(prompt) ? null : prompt;
  }

  static bool SetProperty(string property, string value)
  {
    string propertyName;
    if(!propertyMap.TryGetValue(property.ToLowerInvariant(), out propertyName)) return false;

    PropertyInfo prop = crawl.GetType().GetProperty(propertyName);
    MethodInfo setter = prop.GetSetMethod();
    
    object typedValue = prop.PropertyType.IsEnum ?
      Enum.Parse(prop.PropertyType, value, true) : Convert.ChangeType(value, prop.PropertyType);
    setter.Invoke(crawl, new object[] { typedValue });
    return true;
  }

  static bool VerifyInitialized()
  {
    if(crawl.IsInitialized)
    {
      return true;
    }
    else
    {
      Console.WriteLine("Crawler is not initialized. Use the 'setup' command first.");
      return false;
    }
  }
  
  static readonly Dictionary<string,string> propertyMap;
}

} // namespace Crawler