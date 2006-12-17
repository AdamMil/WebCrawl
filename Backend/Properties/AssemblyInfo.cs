using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("WebCrawl Backend")]
[assembly: AssemblyDescription("An API for web crawling applications.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyProduct("WebCrawl")]
[assembly: AssemblyCopyright("Copyright © Adam Milazzo 2006")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
