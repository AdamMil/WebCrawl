using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WebCrawl.Backend;
using BinaryReader = AdamMil.IO.BinaryReader;
using BinaryWriter = AdamMil.IO.BinaryWriter;

namespace WebCrawl.Gui
{

public partial class MainForm : Form
{
  const int ProjectVersion = 1;

  public MainForm()
  {
    InitializeComponent();
    InitializeItems();

    NewProject();
  }

  #region ColumnComparer
  sealed class ColumnComparer : System.Collections.IComparer
  {
    public ColumnComparer(int column)
    {
      this.column = column;
    }

    public int Compare(object x, object y)
    {
      ListViewItem a = (ListViewItem)x, b = (ListViewItem)y;
      return string.Compare(a.SubItems[column].Text, b.SubItems[column].Text, StringComparison.OrdinalIgnoreCase);
    }

    readonly int column;
  }
  #endregion

  struct Filter
  {
    public Filter(string pattern) : this(pattern, true) { }

    public Filter(string pattern, bool explicitCapture)
    {
      Pattern = pattern;
      Regex   = new Regex(pattern, FilterOptions | (explicitCapture ? RegexOptions.ExplicitCapture : 0));
    }

    public string Pattern;
    public Regex  Regex;
  }

  struct ChangeFilter
  {
    public ChangeFilter(string regex, string replacement) 
    { 
      Filter = new Filter(regex, false); 
      Replacement = replacement;
    }

    public Filter Filter;
    public string Replacement;
  }

  delegate void InvokeDelegate();

  const RegexOptions FilterOptions =
    RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

  bool ProjectOpen
  {
    get { return crawler != null; }
  }

  bool ApplyChanges()
  {
    if(!ValidateFields()) return false;

    // setup page
    if(txtOutDir.Enabled)
    {
      crawler.BaseDirectory = Null(txtOutDir.Text);
    }

    if(txtBaseUrls.Enabled)
    {
      crawler.ClearBaseUris();
      foreach(string line in txtBaseUrls.Lines)
      {
        if(!string.IsNullOrEmpty(line)) crawler.AddBaseUri(new Uri(line), false);
      }
    }

    crawler.DirectoryNavigation = (DirectoryNavigation)dirNav.SelectedItem;
    crawler.DomainNavigation = (DomainNavigation)domainNav.SelectedItem;
    crawler.MaxConnections = ParseLimit(maxConnections.Text);
    crawler.MaxConnectionsPerServer = ParseLimit(connsPerServer.Text);
    ResourceType want = 0;
    if(download.GetItemChecked(0)) want |= ResourceType.Html;
    if(download.GetItemChecked(1)) want |= ResourceType.NonHtml;
    if(download.GetItemChecked(2)) want |= ResourceType.ExternalResources;
    if(download.GetItemChecked(3)) want |= ResourceType.PrioritizeHtml;
    else if(download.GetItemChecked(4)) want |= ResourceType.PrioritizeNonHtml;
    crawler.Download = want;
    crawler.RewriteLinks = chkLinkRewrite.Checked;
    crawler.UseCookies = chkCookies.Checked;
    crawler.GenerateErrorFiles = chkGenerateErrorFiles.Checked;

    // advanced setup page
    extraUrls.Clear();
    foreach(string line in additionalUrls.Lines)
    {
      if(!string.IsNullOrEmpty(line)) extraUrls.Add(new Uri(line));
    }
    crawler.MaxFileSize = ParseSizeLimit(fileSize.Text);
    crawler.MaxQueryStringsPerFile = ParseLimit(queryStrings.Text);
    crawler.DepthLimit = ParseLimit(maxDepth.Text);
    crawler.MaxRetries = ParseLimit(retries.Text);
    crawler.ConnectionIdleTimeout = ParseTimeLimit(idleTimeout.Text);
    crawler.ReadTimeout = ParseTimeLimit(readTimeout.Text);
    crawler.TransferTimeout = ParseTimeLimit(transferTimeout.Text);
    crawler.DefaultReferrer = string.IsNullOrEmpty(referrer.Text) ? null : new Uri(referrer.Text);
    crawler.UserAgent = Null(userAgent.Text);
    normalizeQueries = chkNormalizeQueries.Checked;
    normalizeHosts = chkNormalizeHosts.Checked;
    crawler.PassiveFtp = chkPassiveFtp.Checked;
    clearDownloadDir = chkClear.Checked;
    crawler.PreferredLanguage = GetLanguage();

    // url filters
    positiveFilters.Clear();
    negativeFilters.Clear();
    changeFilters.Clear();
    foreach(ListViewItem item in filters.Items)
    {
      switch(item.SubItems[1].Text)
      {
        case "Change":
          changeFilters.Add(new ChangeFilter(item.SubItems[0].Text, item.SubItems[2].Text));
          break;
        case "Must Match":
          positiveFilters.Add(new Filter(item.SubItems[0].Text));
          break;
        case "Must Not Match":
          negativeFilters.Add(new Filter(item.SubItems[0].Text));
          break;
      }
    }

    // mime types
    crawler.ClearMimeOverrides();
    foreach(ListViewItem item in mimeTypes.Items)
    {
      crawler.AddMimeOverride(item.SubItems[0].Text, item.SubItems[1].Text);
    }

    projectChanged = true;
    fieldChanged = false;
    UpdateApplyButtons();
    UpdateCrawlerMenu();
    return true;
  }

  void ClearDownloadDirectory()
  {
    foreach(string directory in Directory.GetDirectories(crawler.BaseDirectory))
    {
      try { Directory.Delete(directory, true); }
      catch { }
    }
  }

  bool CloseProject()
  {
    if(!Terminate()) return false;

    if(fieldChanged || projectChanged)
    {
      DialogResult result =
        MessageBox.Show("The project has not been saved since the last change. Save it?", "Save project?",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
      if(result == DialogResult.Cancel || result == DialogResult.Yes && !SaveProject()) return false;
    }

    crawler         = null;
    extraUrls       = null;
    positiveFilters = negativeFilters = null;
    changeFilters   = null;
    saveFileName    = null;

    return true;
  }

  bool NewProject()
  {
    if(ProjectOpen && !CloseProject()) return false;

    crawler          = new Crawler();
    extraUrls        = new List<Uri>();
    positiveFilters  = new List<Filter>();
    negativeFilters  = new List<Filter>();
    changeFilters    = new List<ChangeFilter>();
    clearDownloadDir = normalizeHosts = normalizeQueries = false;

    crawler.AddStandardMimeOverrides();
    crawler.FilterUris += crawler_FilterUris;
    crawler.Progress   += crawler_Progress;

    RevertChanges();
    return true;
  }

  void OpenProject(string fileName)
  {
    if(!NewProject()) return;

    try
    {
      using(Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
      using(BinaryReader reader = new BinaryReader(stream))
      {
        int version = reader.ReadInt32();
        if(version != ProjectVersion)
        {
          MessageBox.Show("This is either not a WebCrawl project, or it was created by a different version of the "+
                          "program", "File version mismatch", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          return;
        }

        int count = reader.ReadInt32();
        while(count-- > 0) extraUrls.Add(new Uri(reader.ReadStringWithLength()));

        count = reader.ReadInt32();
        while(count-- > 0) positiveFilters.Add(new Filter(reader.ReadStringWithLength()));

        count = reader.ReadInt32();
        while(count-- > 0) negativeFilters.Add(new Filter(reader.ReadStringWithLength()));

        count = reader.ReadInt32();
        while(count-- > 0)
        {
          changeFilters.Add(new ChangeFilter(reader.ReadStringWithLength(), reader.ReadStringWithLength()));
        }

        clearDownloadDir = reader.ReadBool();
        normalizeQueries = reader.ReadBool();
        normalizeHosts   = reader.ReadBool();

        crawler.LoadSettings(reader);
        RevertChanges();
        saveFileName = fileName;
        status.Text = "Project loaded.";
      }
    }
    catch
    {
      NewProject();
    }
  }

  void RevertChanges()
  {
    // setup page
    List<string> lines = new List<string>();
    foreach(Uri uri in crawler.GetBaseUris()) lines.Add(uri.ToString());
    txtBaseUrls.Lines = lines.ToArray();
    txtOutDir.Text = NoNull(crawler.BaseDirectory);
    dirNav.SelectedItem = crawler.DirectoryNavigation;
    domainNav.SelectedItem = crawler.DomainNavigation;
    maxConnections.Text = LimitToString(crawler.MaxConnections);
    connsPerServer.Text = LimitToString(crawler.MaxConnectionsPerServer);
    download.SetItemChecked(0, (crawler.Download & ResourceType.Html) != 0);
    download.SetItemChecked(1, (crawler.Download & ResourceType.NonHtml) != 0);
    download.SetItemChecked(2, (crawler.Download & ResourceType.ExternalResources) != 0);
    download.SetItemChecked(3, (crawler.Download & ResourceType.PriorityMask) == ResourceType.PrioritizeHtml);
    download.SetItemChecked(4, (crawler.Download & ResourceType.PriorityMask) == ResourceType.PrioritizeNonHtml);
    chkLinkRewrite.Checked = crawler.RewriteLinks;
    chkCookies.Checked = crawler.UseCookies;
    chkGenerateErrorFiles.Checked = crawler.GenerateErrorFiles;

    // advanced setup page
    lines.Clear();
    foreach(Uri uri in extraUrls) lines.Add(uri.ToString());
    additionalUrls.Lines = lines.ToArray();

    fileSize.Text = SizeLimitToString(crawler.MaxFileSize);
    queryStrings.Text = LimitToString(crawler.MaxQueryStringsPerFile);
    maxDepth.Text = LimitToString(crawler.DepthLimit);
    retries.Text = LimitToString(crawler.MaxRetries);
    idleTimeout.Text = TimeLimitToString(crawler.ConnectionIdleTimeout);
    readTimeout.Text = TimeLimitToString(crawler.ReadTimeout);
    transferTimeout.Text = TimeLimitToString(crawler.TransferTimeout);
    referrer.Text = crawler.DefaultReferrer == null ? string.Empty : crawler.DefaultReferrer.ToString();
    userAgent.Text = NoNull(crawler.UserAgent);
    chkNormalizeQueries.Checked = normalizeQueries;
    chkNormalizeHosts.Checked   = normalizeHosts;
    chkPassiveFtp.Checked = crawler.PassiveFtp;
    chkClear.Checked = clearDownloadDir;
    language.SelectedIndex = FindLanguage(crawler.PreferredLanguage);

    // url filters
    filters.Items.Clear();
    foreach(Filter filter in positiveFilters)
    {
      filters.Items.Add(MakeListItem(filter, "Must Match"));
    }
    foreach(Filter filter in negativeFilters)
    {
      filters.Items.Add(MakeListItem(filter, "Must Not Match"));
    }
    foreach(ChangeFilter filter in changeFilters)
    {
      filters.Items.Add(MakeListItem(filter));
    }

    // mime types
    mimeTypes.Items.Clear();
    foreach(MimeOverride mime in Sort(crawler.GetMimeOverrides()))
    {
      mimeTypes.Items.Add(MakeListItem(mime));
    }

    fieldChanged = false;
    UpdateApplyButtons();
  }

  bool SaveProject()
  {
    return string.IsNullOrEmpty(saveFileName) ? SaveProjectAs() : SaveProject(saveFileName);
  }

  bool SaveProject(string fileName)
  {
    if(!TryApplyChanges("saving")) return false;

    using(Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write))
    using(BinaryWriter writer = new BinaryWriter(stream))
    {
      writer.Write(ProjectVersion);
      
      writer.Write(extraUrls.Count);
      foreach(Uri uri in extraUrls) writer.WriteStringWithLength(uri.ToString());

      writer.Write(positiveFilters.Count);
      foreach(Filter filter in positiveFilters) writer.WriteStringWithLength(filter.Pattern);

      writer.Write(negativeFilters.Count);
      foreach(Filter filter in negativeFilters) writer.WriteStringWithLength(filter.Pattern);

      writer.Write(changeFilters.Count);
      foreach(ChangeFilter filter in changeFilters)
      {
        writer.WriteStringWithLength(filter.Filter.Pattern);
        writer.WriteStringWithLength(filter.Replacement);
      }

      writer.Write(chkClear.Checked);
      writer.Write(chkNormalizeQueries.Checked);
      writer.Write(chkNormalizeHosts.Checked);

      crawler.SaveSettings(writer);
    }

    projectChanged = false;
    saveFileName = fileName;
    status.Text = "Project saved.";

    return true;
  }

  bool SaveProjectAs()
  {
    SaveFileDialog fd = new SaveFileDialog();
    fd.DefaultExt = "wcProj";
    fd.Filter = "WebCrawl Projects (*.wcProj)|*.wcProj|All Files (*.*)|*.*";
    if(!string.IsNullOrEmpty(saveFileName)) fd.InitialDirectory = Path.GetDirectoryName(saveFileName);
    else if(!string.IsNullOrEmpty(txtOutDir.Text)) fd.InitialDirectory = txtOutDir.Text;
    fd.Title = "Save Project";

    return fd.ShowDialog() == DialogResult.OK ? SaveProject(fd.FileName) : false;
  }

  bool Terminate()
  {
    if(!crawler.IsStopped)
    {
      if(MessageBox.Show("The crawler is currently running. Terminate it?", "Terminate crawl?",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) ==
         DialogResult.No)
      {
        return false;
      }
      crawler.Terminate(0);
    }

    return true;
  }

  bool TryApplyChanges(string action)
  {
    if(fieldChanged)
    {
      DialogResult result =
          MessageBox.Show("Changes have not been applied. Apply them before "+action+"?", "Apply changes?",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
      if(result == DialogResult.Cancel || result == DialogResult.Yes && !ApplyChanges()) return false;
    }

    return true;
  }

  void UpdateApplyButtons()
  {
    btnApply.Enabled = btnApply2.Enabled = btnApply3.Enabled = btnApply4.Enabled =
      btnRevert.Enabled = btnRevert2.Enabled = btnRevert3.Enabled = btnRevert4.Enabled = fieldChanged;
  }

  bool ValidateFields()
  {
    foreach(string line in txtBaseUrls.Lines)
    {
      if(!ValidateUrl("base", line)) return false;
    }

    foreach(string line in additionalUrls.Lines)
    {
      if(!ValidateUrl("additional", line)) return false;
    }

    if(!string.IsNullOrEmpty(txtOutDir.Text))
    {
      try { new DirectoryInfo(txtOutDir.Text); }
      catch
      {
        MessageBox.Show("Invalid output directory.", "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }

    return ValidateLimit("Maximum connections", maxConnections.Text)    &&
           ValidateLimit("Connections per server", connsPerServer.Text) &&
           ValidateLimit("Query string limit", queryStrings.Text)       &&
           ValidateLimit("Maximum depth", maxDepth.Text)                &&
           ValidateLimit("Maximum retries", retries.Text)               &&
           ValidateSizeLimit("Maximum file size", fileSize.Text)        &&
           ValidateTimeLimit("Idle timeout", idleTimeout.Text)          &&
           ValidateTimeLimit("Read timeout", readTimeout.Text)          &&
           ValidateTimeLimit("Transfer timeout", transferTimeout.Text)  &&
           ValidateUrl("referrer", referrer.Text);
  }

  bool ValidateLimit(string name, string value)
  {
    try
    {
      ParseLimit(value);
      return true;
    }
    catch
    {
      ShowLimitError(name);
      return false;
    }
  }

  bool ValidateSizeLimit(string name, string value)
  {
    try
    {
      ParseSizeLimit(value);
      return true;
    }
    catch
    {
      ShowLimitError(name);
      return false;
    }
  }

  bool ValidateTimeLimit(string name, string value)
  {
    try
    {
      ParseTimeLimit(value);
      return true;
    }
    catch
    {
      ShowLimitError(name);
      return false;
    }
  }

  bool ValidateUrl(string name, string url)
  {
    if(!string.IsNullOrEmpty(url))
    {
      try
      {
        new Uri(url, UriKind.Absolute);
      }
      catch(FormatException)
      {
        MessageBox.Show("Invalid "+name+" url: "+url, "Invalid Url", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }

    return true;
  }

  int FindLanguage(string code)
  {
    for(int i=1; i<language.Items.Count; i++)
    {
      string lang = (string)language.Items[i];
      int paren = lang.LastIndexOf('(');
      if(string.Equals(code, lang.Substring(paren, lang.Length-paren-2), StringComparison.OrdinalIgnoreCase))
      {
        return i;
      }
    }

    return 0;
  }

  string GetLanguage()
  {
    string lang = (string)language.SelectedItem;
    int paren = lang.LastIndexOf('(');
    return paren == -1 ? null : lang.Substring(paren, lang.Length-paren-2);
  }

  void UpdateCrawlerInfo()
  {
    Download[] downloads = crawler.GetCurrentDownloads();
    connections.Text = downloads.Length.ToString();
    queued.Text      = crawler.CurrentLinksQueued.ToString();
    speed.Text       = (crawler.CurrentBytesPerSecond / 1024.0).ToString("f1") + " kps";

    if(crawler.IsRunning || crawler.IsStopping) speedLabel.Text = speed.Text;
    else if(crawler.IsStopped && !crawler.IsDone) speedLabel.Text = "Paused.";

    this.downloads.Items.Clear();
    foreach(Download download in downloads)
    {
      long bytes = download.Resource.Downloaded;
      string bytesStr = bytes >= 1024*1024 ?
        (bytes / (double)(1024*1024)).ToString("f2")+" mb" : (bytes / 1024.0).ToString("f2")+" kb";

      this.downloads.Items.Add(new ListViewItem(new string[] {
        download.Resource.Uri.AbsolutePath + download.Resource.Uri.Query,
        (download.CurrentSpeed / 1024.0).ToString("f1")+" kps",
        bytesStr, download.Resource.Uri.Authority }));
    }
  }

  void UpdateCrawlerMenu()
  {
    startCrawlingMenuItem.Enabled = crawler.IsDone && !crawler.IsRunning &&
      !string.IsNullOrEmpty(crawler.BaseDirectory) && crawler.GetBaseUris().Length != 0;
    pauseCrawlingMenuItem.Enabled = crawler.IsRunning || !crawler.IsDone;
    abortCrawlingMenuItem.Enabled = !crawler.IsDone;
  }

  void OnApplyClicked(object sender, EventArgs e)
  {
    ApplyChanges();
  }

  void OnCrawlStarted()
  {
    txtBaseUrls.Enabled = txtOutDir.Enabled = false;
  }

  void OnCrawlStopped()
  {
    txtBaseUrls.Enabled = txtOutDir.Enabled = true;
    downloads.Items.Clear();
    crawler.Deinitialize();
    UpdateCrawlerMenu();
    speedLabel.Text = "Stopped.";
  }

  void OnFormChanged(object sender, EventArgs e)
  {
    fieldChanged = true;
    UpdateApplyButtons();
  }

  void OnRevertClicked(object sender, EventArgs e)
  {
    RevertChanges();
  }

  void browseOutDir_Click(object sender, EventArgs e)
  {
    FolderBrowserDialog fd = new FolderBrowserDialog();
    fd.Description = "Select the output folder...";
    if(!string.IsNullOrEmpty(txtOutDir.Text)) fd.SelectedPath = txtOutDir.Text;
    if(fd.ShowDialog() == DialogResult.OK) txtOutDir.Text = fd.SelectedPath;
  }

  void download_ItemCheck(object sender, ItemCheckEventArgs e)
  {
    if(e.CurrentValue == e.NewValue) return;

    if(e.NewValue == CheckState.Checked)
    {
      if(e.Index == 3) download.SetItemChecked(4, false);
      else if(e.Index == 4) download.SetItemChecked(3, false);
    }

    OnFormChanged(sender, e);
  }

  void mimeTypes_SelectedIndexChanged(object sender, EventArgs e)
  {
    if(mimeTypes.SelectedIndices.Count != 0)
    {
      ListViewItem item = mimeTypes.Items[mimeTypes.SelectedIndices[0]];
      txtExtension.Text = item.SubItems[0].Text;
      txtMimeType.Text  = item.SubItems[1].Text;
      btnDeleteMime.Enabled = true;
    }
    else
    {
      txtExtension.Text = txtMimeType.Text = string.Empty;
      btnDeleteMime.Enabled = false;
    }
  }

  void filters_SelectedIndexChanged(object sender, EventArgs e)
  {
    if(filters.SelectedIndices.Count != 0)
    {
      ListViewItem item = filters.Items[filters.SelectedIndices[0]];
      txtRegex.Text = item.SubItems[0].Text;
      filterType.SelectedItem = item.SubItems[1].Text;
      if(filterType.SelectedIndex == 2) txtReplacement.Text = item.SubItems[2].Text;

      btnDeleteFilter.Enabled = true;
    }
    else
    {
      txtRegex.Text = txtReplacement.Text = string.Empty;
      btnDeleteFilter.Enabled = false;
    }
  }

  void txtMimeType_TextChanged(object sender, EventArgs e)
  {
    btnAddMime.Enabled = txtMimeType.Text.Trim().Length != 0;
  }

  void btnAddMime_Click(object sender, EventArgs e)
  {
    string extension = txtExtension.Text.Trim(), mimeType = txtMimeType.Text.Trim();
    foreach(ListViewItem item in mimeTypes.Items)
    {
      if(string.Equals(extension, item.SubItems[0].Text, StringComparison.OrdinalIgnoreCase))
      {
        if(!string.Equals(mimeType, item.SubItems[1].Text, StringComparison.OrdinalIgnoreCase))
        {
          item.SubItems[1].Text = mimeType;
          OnFormChanged(sender, e);
        }
        return;
      }
    }

    mimeTypes.Items.Add(MakeListItem(new MimeOverride(extension, txtMimeType.Text.Trim())));
    OnFormChanged(sender, e);
  }

  void btnAddFilter_Click(object sender, EventArgs e)
  {
    string pattern = txtRegex.Text;
    try { new Regex(pattern, FilterOptions & ~RegexOptions.Compiled); }
    catch
    {
      MessageBox.Show("Invalid regular expression: "+pattern, "Invalid regex", MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return;
    }

    filters.Items.Add(filterType.SelectedIndex == 2
      ? MakeListItem(new ChangeFilter(pattern, txtReplacement.Text)) :
        MakeListItem(new Filter(pattern), (string)filterType.SelectedItem));
    OnFormChanged(sender, e);
  }

  void btnDeleteMime_Click(object sender, EventArgs e)
  {
    int index = mimeTypes.SelectedIndices[0];
    mimeTypes.Items.RemoveAt(index);
    if(mimeTypes.Items.Count != 0) mimeTypes.SelectedIndices.Add(Math.Min(index, mimeTypes.Items.Count-1));
    OnFormChanged(sender, e);
    mimeTypes.Focus();
  }

  void btnDeleteFilter_Click(object sender, EventArgs e)
  {
    int index = filters.SelectedIndices[0];
    filters.Items.RemoveAt(index);
    if(filters.Items.Count != 0) filters.SelectedIndices.Add(Math.Min(index, filters.Items.Count-1));
    OnFormChanged(sender, e);
    filters.Focus();
  }

  void filterType_SelectedIndexChanged(object sender, EventArgs e)
  {
    txtReplacement.Enabled = filterType.SelectedIndex == 2;
  }

  void listView_ColumnClick(object sender, ColumnClickEventArgs e)
  {
    ((ListView)sender).ListViewItemSorter = new ColumnComparer(e.Column);
  }

  void crawlerMenu_DropDownOpening(object sender, EventArgs e)
  {
    UpdateCrawlerMenu();
  }

  void exitMenuItem_Click(object sender, EventArgs e)
  {
    Close();
  }

  void MainForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if(!NewProject()) e.Cancel = true;
  }

  void openProjectMenuItem_Click(object sender, EventArgs e)
  {
    if(!Terminate()) return;

    OpenFileDialog fd = new OpenFileDialog();
    fd.DefaultExt = "wcProj";
    fd.Filter = "WebCrawl Projects (*.wcProj)|*.wcProj|All Files (*.*)|*.*";
    fd.Title  = "Open project";

    if(fd.ShowDialog() == DialogResult.OK) OpenProject(fd.FileName);
  }

  void newProjectMenuItem_Click(object sender, EventArgs e)
  {
    NewProject();
  }

  void saveProjectMenuItem_Click(object sender, EventArgs e)
  {
    SaveProject();
  }

  void saveProjectAsMenuItem_Click(object sender, EventArgs e)
  {
    SaveProjectAs();
  }

  void startCrawlingMenuItem_Click(object sender, EventArgs e)
  {
    if(startCrawlingMenuItem.Enabled && TryApplyChanges("starting the crawl"))
    {
      if(clearDownloadDir) ClearDownloadDirectory();

      recentErrors.Items.Clear();

      crawler.ClearUris();
      foreach(Uri baseUri in crawler.GetBaseUris()) crawler.EnqueueUri(baseUri, true);
      foreach(Uri uri in extraUrls) crawler.EnqueueUri(uri, true);

      crawler.Start();
      status.Text = "Crawl started.";

      updateTimer = new Timer();
      updateTimer.Interval = 500;
      updateTimer.Tick += updateTimer_Tick;
      updateTimer.Start();

      OnCrawlStarted();
    }
  }

  void pauseCrawlingMenuItem_Click(object sender, EventArgs e)
  {
    if(!crawler.IsRunning)
    {
      crawler.Start();
      pauseCrawlingMenuItem.Text = "&Pause Crawling";
      status.Text = "Crawl resumed.";
      speedLabel.Text = "Resumed.";
    }
    else
    {
      crawler.Stop();
      pauseCrawlingMenuItem.Text = "Resume Crawling";
      status.Text = "Crawl paused.";
    }
  }

  void abortCrawlingMenuItem_Click(object sender, EventArgs e)
  {
    crawler.Terminate(0);
    updateTimer.Stop();
    updateTimer = null;
    status.Text = "Crawl aborted.";
    OnCrawlStopped();
  }

  void updateTimer_Tick(object sender, EventArgs e)
  {
    UpdateCrawlerInfo();

    if(crawler.IsDone)
    {
      crawler.Stop();
      OnCrawlStopped();
      status.Text = "Crawl finished.";
    }
  }

  void crawler_Progress(Resource resource, string extraMessage)
  {
    if(resource.Status == Status.FatalErrorOccurred || resource.Status == Status.NonFatalErrorOccurred)
    {
      recentErrors.BeginInvoke((InvokeDelegate)delegate()
      {
        if(recentErrors.Items.Count == 100) recentErrors.Items.RemoveAt(0);
        recentErrors.Items.Add(new ListViewItem(new string[] {
          resource.Uri.ToString(), extraMessage, resource.Referrer == null ? "" : resource.Referrer.ToString() }));
      });
    }
  }

  Uri crawler_FilterUris(Uri uri)
  {
    string uriString = uri.ToString();

    if(changeFilters != null)
    {
      bool uriChanged = false;
      for(int i=0; i<changeFilters.Count; i++)
      {
        Match uriMatch = changeFilters[i].Filter.Regex.Match(uriString);
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
      for(int i=0; i<positiveFilters.Count; i++)
      {
        if(!positiveFilters[i].Regex.IsMatch(uriString)) return null;
      }
    }

    if(negativeFilters != null)
    {
      for(int i=0; i<negativeFilters.Count; i++)
      {
        if(negativeFilters[i].Regex.IsMatch(uriString)) return null;
      }
    }

    if(normalizeHosts) uri = UrlFilters.StripWWWPrefix(uri);
    if(normalizeQueries) uri = UrlFilters.NormalizeQuery(uri);

    return uri;
  }

  void text_KeyDown(object sender, KeyEventArgs e)
  {
    if(e.Modifiers == Keys.Control && e.KeyCode == Keys.A) // ctrl-A selects all
    {
      TextBox textBox = (TextBox)sender;
      textBox.SelectAll();
    }
  }

  void InitializeItems()
  {
    domainNav.Items.Add(DomainNavigation.Everywhere);
    domainNav.Items.Add(DomainNavigation.SameDomain);
    domainNav.Items.Add(DomainNavigation.SameHostName);
    domainNav.Items.Add(DomainNavigation.SameTLD);

    dirNav.Items.Add(DirectoryNavigation.Down);
    dirNav.Items.Add(DirectoryNavigation.Same);
    dirNav.Items.Add(DirectoryNavigation.Up);
    dirNav.Items.Add(DirectoryNavigation.UpAndDown);

    download.Items.Add("Save Html");
    download.Items.Add("Save Non-Html");
    download.Items.Add("Save External Resources");
    download.Items.Add("Prioritize Html");
    download.Items.Add("Prioritize Non-Html");

    List<string> languages = new List<string>();
    foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
    {
      if(!string.IsNullOrEmpty(culture.IetfLanguageTag))
      {
        languages.Add(culture.EnglishName+" ("+culture.IetfLanguageTag+")");
      }
    }

    languages.Sort();
    languages.Insert(0, "No preference");
    foreach(string language in languages) this.language.Items.Add(language);

    filterType.SelectedIndex = 0;
  }

  static string LimitToString(long limit)
  {
    if(limit == Crawler.Infinite) return string.Empty;
    else return limit.ToString(CultureInfo.InvariantCulture);
  }

  static ListViewItem MakeListItem(Filter filter, string type)
  {
    return new ListViewItem(new string[] { filter.Pattern, type });
  }

  static ListViewItem MakeListItem(ChangeFilter filter)
  {
    return new ListViewItem(new string[] { filter.Filter.Pattern, "Change", filter.Replacement });
  }

  static ListViewItem MakeListItem(MimeOverride mime)
  {
    return new ListViewItem(new string[] { mime.Extension, mime.MimeType });
  }

  static string NoNull(string str)
  {
    return str == null ? string.Empty : str;
  }

  static string Null(string str)
  {
    if(str != null) str = str.Trim();
    return string.IsNullOrEmpty(str) ? null : str;
  }

  static int ParseLimit(string str)
  {
    return string.IsNullOrEmpty(str) ? Crawler.Infinite : int.Parse(str);
  }

  static long ParseSizeLimit(string str)
  {
    if(string.IsNullOrEmpty(str)) return Crawler.Infinite;
    else
    {
      Match m = sizeRe.Match(str);
      if(!m.Success) throw new ArgumentException("Invalid time limit: "+str);
      long num = int.Parse(m.Groups[1].Value);
      if(m.Groups[2].Success)
      {
        switch(m.Groups[2].Value[0])
        {
          case 'k': num *= 1024; break;
          case 'm': num *= 1024*1024; break;
          case 'g': num *= 1024*1024*1024; break;
        }
      }
      return num;
    }
  }

  static int ParseTimeLimit(string str)
  {
    if(string.IsNullOrEmpty(str)) return Crawler.Infinite;
    else
    {
      Match m = timeRe.Match(str);
      if(!m.Success) throw new ArgumentException("Invalid time limit: "+str);
      int num = int.Parse(m.Groups[1].Value);
      if(m.Groups[2].Success)
      {
        switch(m.Groups[2].Value[0])
        {
          case 'm': num *= 60; break;
          case 'h': num *= 3600; break;
        }
      }
      return num;
    }
  }

  static string SizeLimitToString(long limit)
  {
    if(limit == Crawler.Infinite) return string.Empty;
    else if(limit >= 1024*1024*1024 && limit % 1024*1024*1024 == 0)
    {
      return (limit / (1024*1024*1024)).ToString(CultureInfo.InvariantCulture) + " gb";
    }
    else if(limit >= 1024*1024 && limit % 1024*1024 == 0)
    {
      return (limit / (1024*1024)).ToString(CultureInfo.InvariantCulture) + " mb";
    }
    else if(limit >= 1024 && limit % 1024 == 0)
    {
      return (limit / 1024).ToString(CultureInfo.InvariantCulture) + " kb";
    }
    else return limit.ToString(CultureInfo.InvariantCulture);
  }

  static void ShowLimitError(string name)
  {
    MessageBox.Show(name+" is not a valid limit.", "Invalid Limit", MessageBoxButtons.OK, MessageBoxIcon.Error);
  }

  static MimeOverride[] Sort(MimeOverride[] overrides)
  {
    Array.Sort(overrides, delegate(MimeOverride a, MimeOverride b) { return a.Extension.CompareTo(b.Extension); });
    return overrides;
  }

  static string TimeLimitToString(long limit)
  {
    if(limit == Crawler.Infinite) return string.Empty;
    else if(limit >= 3600 && limit % 3600 == 0) return (limit / 3600).ToString(CultureInfo.InvariantCulture) + "h";
    else if(limit >= 60 && limit % 60 == 0) return (limit / 60).ToString(CultureInfo.InvariantCulture) + "m";
    else return limit.ToString(CultureInfo.InvariantCulture) + "s";
  }

  Crawler crawler;
  List<Uri> extraUrls;
  List<Filter> positiveFilters, negativeFilters;
  List<ChangeFilter> changeFilters;
  string saveFileName;
  Timer updateTimer;
  bool fieldChanged, projectChanged, clearDownloadDir, normalizeQueries, normalizeHosts;

  static readonly Regex timeRe = new Regex(@"^\s*(\d+)\s*([smh])?\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
  static readonly Regex sizeRe = new Regex(@"^\s*(\d+)\s*([kmg]b)?\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
  static readonly Regex varRe = new Regex(@"\$(?:\{(?<group>\d+)\}|(?<group>\d+))",
                                          RegexOptions.Singleline|RegexOptions.Compiled);
}

} // namespace WebCrawl.Gui