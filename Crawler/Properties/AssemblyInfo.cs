using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("WebCrawl Text Frontend")]
[assembly: AssemblyDescription("A console-based web crawling application.")]
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
