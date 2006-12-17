using System;
using WebCrawl.Backend;

namespace WebCrawl
{

static class App
{
  static Crawler crawl;
  static void Main()
  {
    crawl = new Crawler();
    crawl.Progress += new ProgressHandler(c_Progress);
    crawl.DomainNavigation = DomainNavigation.SameDomain;
    crawl.Download = Download.Everything;
    crawl.MaxConnectionsPerServer = 1;
    crawl.MaxConnections = 20;
    crawl.ProgressFilter = ProgressType.AnyErrorOccurred;

    if(System.IO.Directory.Exists("e:/osho")) System.IO.Directory.Delete("e:/osho", true);
    crawl.Initialize("e:/osho");
    crawl.AddStandardMimeOverrides();
    crawl.AddBaseUri("http://www.oshofriendsinternational.com/");
    crawl.AddBaseUri("http://www.oshoworld.com/");
    crawl.AddBaseUri("http://www.iosho.com/");
    crawl.AddBaseUri("http://www.otantra.net/");
    
    crawl.Start();

    while(crawl.CurrentDownloadCount != 0)
    {
      if(Console.KeyAvailable)
      {
        char c = char.ToLowerInvariant(Console.ReadKey(false).KeyChar);
        if(c == 'q') crawl.Stop();
        else if(c == 'x') break;
      }

      System.Threading.Thread.Sleep(5000);

      Console.WriteLine(string.Format("{0} downloading, {1} queued, {2}kps", crawl.CurrentDownloadCount,
                                      crawl.CurrentLinksQueued, Math.Round(crawl.CurrentBytesPerSecond/1024.0, 1)));
    }

    crawl.Deinitialize();
  }

  static void c_Progress(Resource resource)
  {
    string prefix, suffix = null;
suffix = " (from "+resource.Referrer+")";

    switch(resource.Status)
    {
      case ProgressType.DownloadStarted: prefix = ". "; break;
      case ProgressType.DownloadFinished:
        prefix = "- ";
        suffix = " ("+crawl.CurrentLinksQueued+" left, avg "+Math.Round(crawl.CurrentBytesPerSecond/1024.0, 1)+"kps)";
        break;
      case ProgressType.NonFatalErrorOccurred: prefix = "? "; break;
      case ProgressType.FatalErrorOccurred: prefix = "! "; break;
      case ProgressType.UrlQueued:
        prefix = "+["+resource.Depth+"] ";
        suffix = " (from "+resource.Referrer+")";
        break;
      default: prefix = "UNKNOWN "; break;
    }

    Console.WriteLine(prefix + resource.Uri.PathAndQuery + suffix);
  }
}

} // namespace Crawler