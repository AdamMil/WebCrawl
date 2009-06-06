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
  const int ProjectVersion = 3;

  public MainForm()
  {
    InitializeComponent();
    InitializeItems();

    NewProject();
  }

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    base.OnFormClosing(e);
    if(!e.Cancel && !NewProject()) e.Cancel = true;
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

  #region ListViewNF
  /// <summary>A list view with less flicker.</summary>
  sealed class ListViewNF : System.Windows.Forms.ListView
  {
    public ListViewNF()
    {
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.EnableNotifyMessage, true);
    }

    protected override void OnNotifyMessage(Message m)
    {
      //Filter out the WM_ERASEBKGND message
      if(m.Msg != 0x14) base.OnNotifyMessage(m);
    }
  }
  #endregion

  #region Filter
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
  #endregion

  #region ChangeFilter
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
  #endregion

  #region InputUri
  class InputUri
  {
    public InputUri(string input)
    {
      int space = input.IndexOf(' ');

      if(space == -1) Uri = new Uri(input, UriKind.Absolute);
      else
      {
        Uri = new Uri(input.Substring(0, space), UriKind.Absolute);
        PostData = input.Substring(space+1);
      }
    }

    public readonly Uri Uri;
    public readonly string PostData;
  }
  #endregion

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

    crawler.DirectoryNavigation = (DirectoryNavigation)cmbDirNav.SelectedItem;
    crawler.DomainNavigation = (DomainNavigation)cmbDomainNav.SelectedItem;
    crawler.MaxConnections = ParseLimit(txtMaxConnections.Text);
    crawler.MaxConnectionsPerServer = ParseLimit(txtConnsPerServer.Text);
    DownloadFlags want = 0;
    if(lstDownload.GetItemChecked(0)) want |= DownloadFlags.Html;
    if(lstDownload.GetItemChecked(1)) want |= DownloadFlags.NonHtml;
    if(lstDownload.GetItemChecked(2)) want |= DownloadFlags.ExternalResources;
    if(lstDownload.GetItemChecked(3)) want |= DownloadFlags.PrioritizeHtml;
    else if(lstDownload.GetItemChecked(4)) want |= DownloadFlags.PrioritizeNonHtml;
    crawler.Download = want;
    crawler.RewriteLinks = chkLinkRewrite.Checked;
    crawler.UseCookies = chkCookies.Checked;
    crawler.GenerateErrorFiles = chkGenerateErrorFiles.Checked;

    // advanced setup page
    extraUrls.Clear();
    foreach(string line in txtAdditionalUrls.Lines)
    {
      if(!string.IsNullOrEmpty(line)) extraUrls.Add(new InputUri(line));
    }
    crawler.MaxFileSize = ParseSizeLimit(txtFileSize.Text);
    crawler.MaxQueryStringsPerFile = ParseLimit(txtQueryStrings.Text);
    crawler.DepthLimit = ParseLimit(txtMaxDepth.Text);
    crawler.MaxRetries = ParseLimit(txtRetries.Text);
    crawler.ConnectionIdleTimeout = ParseTimeLimit(txtIdleTimeout.Text);
    crawler.ReadTimeout = ParseTimeLimit(txtReadTimeout.Text);
    crawler.TransferTimeout = ParseTimeLimit(txtTransferTimeout.Text);
    crawler.MaxQueuedLinks = ParseLimit(txtMaxQueue.Text);
    crawler.DefaultReferrer = string.IsNullOrEmpty(txtReferrer.Text) ? null : new Uri(txtReferrer.Text);
    crawler.UserAgent = Null(txtUserAgent.Text);
    normalizeQueries = chkNormalizeQueries.Checked;
    normalizeHosts = chkNormalizeHosts.Checked;
    crawler.PassiveFtp = chkPassiveFtp.Checked;
    clearDownloadDir = chkClear.Checked;
    enqueueBaseUrls = chkEnqueueBaseUrls.Checked;
    crawler.PreferredLanguage = GetLanguage();

    // filters
    positiveFilters.Clear();
    negativeFilters.Clear();
    changeFilters.Clear();
    contentFilters.Clear();
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
        case "Content":
          contentFilters.Add(new ChangeFilter(item.SubItems[0].Text, item.SubItems[2].Text));
          break;
      }
    }

    // mime types
    crawler.ClearMimeOverrides();
    foreach(ListViewItem item in mimeTypes.Items)
    {
      crawler.AddMimeOverride(item.SubItems[0].Text, item.SubItems[1].Text, item.SubItems[2].Text == "Yes");
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

    if(crawler != null) crawler.Dispose();
    crawler         = null;
    extraUrls       = null;
    positiveFilters = negativeFilters = null;
    changeFilters   = contentFilters  = null;
    saveFileName    = null;

    return true;
  }

  bool CreateDownloadDirectory()
  {
    if(!Directory.Exists(crawler.BaseDirectory))
    {
      if(MessageBox.Show("The download directory does not exist. Do you want to create it now?",
                         "Create download directory?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
           == DialogResult.Yes)
      {
        try { Directory.CreateDirectory(crawler.BaseDirectory); }
        catch { return false; }
      }
      else
      {
        return false;
      }
    }

    return true;
  }

  bool NewProject()
  {
    if(ProjectOpen && !CloseProject()) return false;

    crawler          = new Crawler();
    extraUrls        = new List<InputUri>();
    positiveFilters  = new List<Filter>();
    negativeFilters  = new List<Filter>();
    changeFilters    = new List<ChangeFilter>();
    contentFilters   = new List<ChangeFilter>();
    clearDownloadDir = normalizeHosts = normalizeQueries = false;
    enqueueBaseUrls  = true;

    crawler.AddStandardMimeOverrides();
    crawler.FilterContent += crawler_FilterContent;
    crawler.FilterUris += crawler_FilterUris;
    crawler.Progress   += crawler_Progress;

    RevertChanges();
    return true;
  }

  public void OpenProject(string fileName)
  {
    if(!NewProject()) return;

    try
    {
      using(Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
      using(BinaryReader reader = new BinaryReader(stream))
      {
        int version = reader.ReadInt32();
        if(version > ProjectVersion)
        {
          MessageBox.Show("This is either not a WebCrawl project, or it was created by a different version of the "+
                          "program", "File version mismatch", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          return;
        }

        int count = reader.ReadInt32();
        while(count-- > 0) extraUrls.Add(new InputUri(reader.ReadStringWithLength()));

        count = reader.ReadInt32();
        while(count-- > 0) positiveFilters.Add(new Filter(reader.ReadStringWithLength()));

        count = reader.ReadInt32();
        while(count-- > 0) negativeFilters.Add(new Filter(reader.ReadStringWithLength()));

        count = reader.ReadInt32();
        while(count-- > 0)
        {
          changeFilters.Add(new ChangeFilter(reader.ReadStringWithLength(), reader.ReadStringWithLength()));
        }

        if(version >= 2)
        {
          count = reader.ReadInt32();
          while(count-- > 0)
          {
            contentFilters.Add(new ChangeFilter(reader.ReadStringWithLength(), reader.ReadStringWithLength()));
          }
        }

        clearDownloadDir = reader.ReadBool();
        normalizeQueries = reader.ReadBool();
        normalizeHosts   = reader.ReadBool();

        if(version >= 2)
        {
          enqueueBaseUrls = reader.ReadBool();
        }

        crawler.LoadSettings(reader);
        RevertChanges();
        saveFileName = fileName;
        status.Text = "Project loaded.";
      }
    }
    catch
    {
      NewProject();
      status.Text = "An error occured while loading project.";
      MessageBox.Show("An error occurred while loading the project file.", "Error loading project",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  void RevertChanges()
  {
    // setup page
    List<string> lines = new List<string>();
    foreach(Uri uri in crawler.GetBaseUris()) lines.Add(uri.AbsoluteUri);
    txtBaseUrls.Lines = lines.ToArray();
    txtOutDir.Text = NoNull(crawler.BaseDirectory);
    cmbDirNav.SelectedItem = crawler.DirectoryNavigation;
    cmbDomainNav.SelectedItem = crawler.DomainNavigation;
    txtMaxConnections.Text = LimitToString(crawler.MaxConnections);
    txtConnsPerServer.Text = LimitToString(crawler.MaxConnectionsPerServer);
    lstDownload.SetItemChecked(0, (crawler.Download & DownloadFlags.Html) != 0);
    lstDownload.SetItemChecked(1, (crawler.Download & DownloadFlags.NonHtml) != 0);
    lstDownload.SetItemChecked(2, (crawler.Download & DownloadFlags.ExternalResources) != 0);
    lstDownload.SetItemChecked(3, (crawler.Download & DownloadFlags.PriorityMask) == DownloadFlags.PrioritizeHtml);
    lstDownload.SetItemChecked(4, (crawler.Download & DownloadFlags.PriorityMask) == DownloadFlags.PrioritizeNonHtml);
    chkLinkRewrite.Checked = crawler.RewriteLinks;
    chkCookies.Checked = crawler.UseCookies;
    chkGenerateErrorFiles.Checked = crawler.GenerateErrorFiles;

    // advanced setup page
    lines.Clear();
    foreach(InputUri r in extraUrls) lines.Add(r.Uri.AbsoluteUri + (r.PostData == null ? null : " "+r.PostData));
    txtAdditionalUrls.Lines = lines.ToArray();

    txtFileSize.Text = SizeLimitToString(crawler.MaxFileSize);
    txtQueryStrings.Text = LimitToString(crawler.MaxQueryStringsPerFile);
    txtMaxDepth.Text = LimitToString(crawler.DepthLimit);
    txtRetries.Text = LimitToString(crawler.MaxRetries);
    txtIdleTimeout.Text = TimeLimitToString(crawler.ConnectionIdleTimeout);
    txtReadTimeout.Text = TimeLimitToString(crawler.ReadTimeout);
    txtTransferTimeout.Text = TimeLimitToString(crawler.TransferTimeout);
    txtMaxQueue.Text = LimitToString(crawler.MaxQueuedLinks);
    txtReferrer.Text = crawler.DefaultReferrer == null ? string.Empty : crawler.DefaultReferrer.AbsoluteUri;
    txtUserAgent.Text = NoNull(crawler.UserAgent);
    chkNormalizeQueries.Checked = normalizeQueries;
    chkNormalizeHosts.Checked   = normalizeHosts;
    chkPassiveFtp.Checked = crawler.PassiveFtp;
    chkClear.Checked = clearDownloadDir;
    chkEnqueueBaseUrls.Checked = enqueueBaseUrls;
    cmbLanguage.SelectedIndex = FindLanguage(crawler.PreferredLanguage);

    // filters
    filters.Items.Clear();
    foreach(Filter filter in positiveFilters) filters.Items.Add(MakeListItem(filter, "Must Match"));
    foreach(Filter filter in negativeFilters) filters.Items.Add(MakeListItem(filter, "Must Not Match"));
    foreach(ChangeFilter filter in changeFilters) filters.Items.Add(MakeListItem(filter, "Change"));
    foreach(ChangeFilter filter in contentFilters) filters.Items.Add(MakeListItem(filter, "Content"));

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
      foreach(InputUri r in extraUrls)
      {
        writer.WriteStringWithLength(r.Uri.AbsoluteUri + (r.PostData == null ? null : " "+r.PostData));
      }

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

      writer.Write(contentFilters.Count);
      foreach(ChangeFilter filter in contentFilters)
      {
        writer.WriteStringWithLength(filter.Filter.Pattern);
        writer.WriteStringWithLength(filter.Replacement);
      }

      writer.Write(clearDownloadDir);
      writer.Write(normalizeQueries);
      writer.Write(normalizeHosts);
      writer.Write(enqueueBaseUrls);

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
      if(!ValidateUrl("base", line, false)) return false;
    }

    foreach(string line in txtAdditionalUrls.Lines)
    {
      if(!ValidateUrl("additional", line, true)) return false;
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

    return ValidateLimit("Maximum connections", txtMaxConnections.Text)    &&
           ValidateLimit("Connections per server", txtConnsPerServer.Text) &&
           ValidateLimit("Query string limit", txtQueryStrings.Text)       &&
           ValidateLimit("Maximum depth", txtMaxDepth.Text)                &&
           ValidateLimit("Maximum retries", txtRetries.Text)               &&
           ValidateLimit("Maximum queued links", txtMaxQueue.Text)         &&
           ValidateSizeLimit("Maximum file size", txtFileSize.Text)        &&
           ValidateTimeLimit("Idle timeout", txtIdleTimeout.Text)          &&
           ValidateTimeLimit("Read timeout", txtReadTimeout.Text)          &&
           ValidateTimeLimit("Transfer timeout", txtTransferTimeout.Text)  &&
           ValidateUrl("referrer", txtReferrer.Text, false);
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

  bool ValidateUrl(string name, string url, bool allowPostData)
  {
    if(!string.IsNullOrEmpty(url))
    {
      try
      {
        if(allowPostData) new InputUri(url);
        else new Uri(url, UriKind.Absolute);
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
    for(int i=1; i<cmbLanguage.Items.Count; i++)
    {
      string lang = (string)cmbLanguage.Items[i];
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
    string lang = (string)cmbLanguage.SelectedItem;
    int paren = lang.LastIndexOf('(');
    return paren == -1 ? null : lang.Substring(paren, lang.Length-paren-2);
  }

  void UpdateCrawlerInfo()
  {
    Download[] downloads = crawler.GetCurrentDownloads();
    connections.Text         = downloads.Length.ToString();
    queued.Text              = crawler.CurrentLinksQueued.ToString();
    speed.Text               = (crawler.CurrentBytesPerSecond / 1024.0).ToString("f1") + " kps";
    downloadedResources.Text = crawler.TotalResourcesDownloaded.ToString();
    bytesDownloaded.Text     = SizeToString(crawler.TotalBytesDownloaded);

    if(crawler.IsRunning || crawler.IsStopping) speedLabel.Text = speed.Text;
    else if(crawler.IsStopped && !crawler.IsDone) speedLabel.Text = "Paused.";

    this.downloads.SuspendLayout();
    this.downloads.Items.Clear();
    foreach(Download download in downloads)
    {
      this.downloads.Items.Add(new ListViewItem(new string[] {
        download.Resource.Uri.AbsolutePath + download.Resource.Uri.Query,
        (download.CurrentSpeed / 1024.0).ToString("f1")+" kps",
        SizeToString(download.Resource.BytesDownloaded, false),
        download.Resource.TotalSize == -1 ? "" : SizeToString(download.Resource.TotalSize, false),
        download.Resource.ContentType, download.Resource.Uri.Authority }));
    }
    this.downloads.ResumeLayout();
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
    if(updateTimer != null)
    {
      updateTimer.Stop();
      updateTimer.Dispose();
      updateTimer = null;
    }
    UpdateCrawlerInfo();

    txtBaseUrls.Enabled = txtOutDir.Enabled = true;
    downloads.Items.Clear();
    crawler.Deinitialize(0);
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

  void clearErrorsMenuItem_Click(object sender, EventArgs e)
  {
    recentErrors.Items.Clear();
  }

  void download_ItemCheck(object sender, ItemCheckEventArgs e)
  {
    if(e.CurrentValue == e.NewValue) return;

    if(e.NewValue == CheckState.Checked)
    {
      if(e.Index == 3) lstDownload.SetItemChecked(4, false);
      else if(e.Index == 4) lstDownload.SetItemChecked(3, false);
    }

    OnFormChanged(sender, e);
  }

  void mimeTypes_SelectedIndexChanged(object sender, EventArgs e)
  {
    if(mimeTypes.SelectedIndices.Count != 0)
    {
      ListViewItem item     = mimeTypes.Items[mimeTypes.SelectedIndices[0]];
      txtExtension.Text     = item.SubItems[0].Text;
      txtMimeType.Text      = item.SubItems[1].Text;
      chkPreferred.Checked  = item.SubItems[2].Text == "Yes";
      btnDeleteMime.Enabled = true;
    }
    else
    {
      txtExtension.Text = txtMimeType.Text = string.Empty;
      chkPreferred.Checked  = false;
      btnDeleteMime.Enabled = false;
    }
  }

  void filters_SelectedIndexChanged(object sender, EventArgs e)
  {
    if(filters.SelectedIndices.Count != 0)
    {
      ListViewItem item = filters.Items[filters.SelectedIndices[0]];
      txtRegex.Text = item.SubItems[0].Text;
      cmbFilterType.SelectedItem = item.SubItems[1].Text;
      if(cmbFilterType.SelectedIndex == 2 || cmbFilterType.SelectedIndex == 3) txtReplacement.Text = item.SubItems[2].Text;

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

    int index;
    for(index=0; index<mimeTypes.Items.Count; index++)
    {
      ListViewItem item = mimeTypes.Items[index];
      if(string.Equals(extension, item.SubItems[0].Text, StringComparison.OrdinalIgnoreCase))
      {
        item.SubItems[1].Text = mimeType;
        item.SubItems[2].Text = chkPreferred.Checked ? "Yes" : "No";
        break;
      }
    }

    if(index == mimeTypes.Items.Count)
    {
      index = mimeTypes.Items.Add(MakeListItem(new MimeOverride(extension, txtMimeType.Text.Trim(),
                                                                chkPreferred.Checked))).Index;
    }

    if(chkPreferred.Checked) // if this is the preferred extension, make sure no other extensions are also preferred
    {
      for(int i=0; i<mimeTypes.Items.Count; i++)
      {
        if(i != index)
        {
          ListViewItem item = mimeTypes.Items[i];
          if(string.Equals(mimeType, item.SubItems[1].Text, StringComparison.OrdinalIgnoreCase))
          {
            item.SubItems[2].Text = "No";
          }
        }
      }
    }

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

    filters.Items.Add(cmbFilterType.SelectedIndex == 2 || cmbFilterType.SelectedIndex == 3
      ? MakeListItem(new ChangeFilter(pattern, txtReplacement.Text), (string)cmbFilterType.SelectedItem) :
        MakeListItem(new Filter(pattern), (string)cmbFilterType.SelectedItem));
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
    txtReplacement.Enabled = cmbFilterType.SelectedIndex == 2 || cmbFilterType.SelectedIndex == 3;
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
    if(startCrawlingMenuItem.Enabled && TryApplyChanges("starting the crawl") && CreateDownloadDirectory())
    {
      if(clearDownloadDir) ClearDownloadDirectory();

      recentErrors.Items.Clear();

      crawler.ClearUris(true);
      if(enqueueBaseUrls)
      {
        foreach(Uri uri in crawler.GetBaseUris()) crawler.EnqueueUri(uri, true);
      }

      foreach(InputUri resource in extraUrls) crawler.EnqueueUri(resource.Uri, resource.PostData, true);

      crawler.Start();
      status.Text = "Crawl started.";

      updateTimer = new Timer();
      updateTimer.Interval = 750;
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
      speedLabel.Text = "Finished.";
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
          resource.Uri.AbsoluteUri, extraMessage, resource.Referrer == null ? "" : resource.Referrer.AbsoluteUri,
          resource.Status == Status.FatalErrorOccurred ? "Yes" : "No"
        }));
      });
    }
  }

  void crawler_FilterContent(Uri url, string mimeType, ResourceType resourceType, System.Text.Encoding encoding,
                             string contentFileName)
  {
    if(resourceType != ResourceType.Binary && contentFilters.Count != 0)
    {
      string content;
      using(StreamReader reader =
              encoding == null ? new StreamReader(contentFileName) : new StreamReader(contentFileName, encoding))
      {
        encoding = reader.CurrentEncoding;
        content  = reader.ReadToEnd();
      }

      string newContent = content;
      foreach(ChangeFilter filter in contentFilters)
      {
        newContent = filter.Filter.Regex.Replace(newContent,
                                              delegate(Match m) { return ReplaceVars(filter.Replacement, m); });
      }

      if(!string.Equals(content, newContent, StringComparison.Ordinal))
      {
        using(StreamWriter writer =
                new StreamWriter(contentFileName, false, encoding == null ? System.Text.Encoding.UTF8 : encoding))
        {
          writer.Write(newContent);
        }
      }
    }
  }

  Uri crawler_FilterUris(Uri uri)
  {
    string uriString = uri.AbsoluteUri;

    if(changeFilters != null)
    {
      bool uriChanged = false;
      foreach(ChangeFilter filter in changeFilters)
      {
        Match uriMatch = filter.Filter.Regex.Match(uriString);
        if(uriMatch.Success)
        {
          uriString  = ReplaceVars(filter.Replacement, uriMatch);
          uriChanged = true;
        }
      }
      if(uriChanged) uri = new Uri(uriString);
    }

    if(positiveFilters != null)
    {
      foreach(Filter filter in positiveFilters)
      {
        if(!filter.Regex.IsMatch(uriString)) return null;
      }
    }

    if(negativeFilters != null)
    {
      foreach(Filter filter in negativeFilters)
      {
        if(filter.Regex.IsMatch(uriString)) return null;
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
    cmbDomainNav.Items.Add(DomainNavigation.Everywhere);
    cmbDomainNav.Items.Add(DomainNavigation.SameDomain);
    cmbDomainNav.Items.Add(DomainNavigation.SameHostName);
    cmbDomainNav.Items.Add(DomainNavigation.SameTLD);

    cmbDirNav.Items.Add(DirectoryNavigation.Down);
    cmbDirNav.Items.Add(DirectoryNavigation.Same);
    cmbDirNav.Items.Add(DirectoryNavigation.Up);
    cmbDirNav.Items.Add(DirectoryNavigation.UpAndDown);

    lstDownload.Items.Add("Save Html");
    lstDownload.Items.Add("Save Non-Html");
    lstDownload.Items.Add("Save External Resources");
    lstDownload.Items.Add("Prioritize Html");
    lstDownload.Items.Add("Prioritize Non-Html");

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
    foreach(string language in languages) this.cmbLanguage.Items.Add(language);

    cmbFilterType.SelectedIndex = 0;
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

  static ListViewItem MakeListItem(ChangeFilter filter, string type)
  {
    return new ListViewItem(new string[] { filter.Filter.Pattern, type, filter.Replacement });
  }

  static ListViewItem MakeListItem(MimeOverride mime)
  {
    return new ListViewItem(new string[] { mime.Extension, mime.MimeType, mime.IsPreferred ? "Yes" : "No" });
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

  static string ReplaceVars(string replacement, Match match)
  {
    return varRe.Replace(replacement,
      delegate(Match var) { return match.Groups[int.Parse(var.Groups["group"].Value)].Value; });
  }

  static string SizeToString(long size)
  {
    return SizeToString(size, true);
  }

  static string SizeToString(long size, bool showBytes)
  {
    double dblSize = size;
    string suffix = null;
    if(size >= 1024*1024*1024)
    {
      dblSize /= 1024*1024*1024;
      suffix = " gb";
    }
    else if(size >= 1024*1024)
    {
      dblSize /= 1024*1024;
      suffix = " mb";
    }
    else if(size >= 1024 || !showBytes)
    {
      dblSize /= 1024;
      suffix = " kb";
    }

    return dblSize.ToString("f2") + suffix;
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
  List<InputUri> extraUrls;
  List<Filter> positiveFilters, negativeFilters;
  List<ChangeFilter> changeFilters, contentFilters;
  string saveFileName;
  Timer updateTimer;
  bool fieldChanged, projectChanged, clearDownloadDir, normalizeQueries, normalizeHosts, enqueueBaseUrls;

  static readonly Regex timeRe = new Regex(@"^\s*(\d+)\s*([smh])?\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
  static readonly Regex sizeRe = new Regex(@"^\s*(\d+)\s*([kmg]b)?\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
  static readonly Regex varRe = new Regex(@"\$(?:\{(?<group>\d+)\}|(?<group>\d+))",
                                          RegexOptions.Singleline|RegexOptions.Compiled);
}

} // namespace WebCrawl.Gui