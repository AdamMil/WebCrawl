using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WebCrawl.Gui
{
  static class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      MainForm form = new MainForm();
      if(args.Length != 0) form.OpenProject(args[0]);
      Application.Run(form);
    }
  }
}