namespace WebCrawl.Gui
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if(disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.MenuStrip menuStrip;
      System.Windows.Forms.ToolStripMenuItem fileMenu;
      System.Windows.Forms.ToolStripMenuItem newProjectMenuItem;
      System.Windows.Forms.ToolStripMenuItem openProjectMenuItem;
      System.Windows.Forms.ToolStripMenuItem saveProjectMenuItem;
      System.Windows.Forms.ToolStripMenuItem saveProjectAsMenuItem;
      System.Windows.Forms.ToolStripSeparator menuSep1;
      System.Windows.Forms.StatusStrip statusStrip;
      System.Windows.Forms.TabControl tabControl;
      System.Windows.Forms.TabPage setupTab;
      System.Windows.Forms.Label lblBaseUrls;
      System.Windows.Forms.Label lblConnsPerServer;
      System.Windows.Forms.Label lblConns;
      System.Windows.Forms.Label lblDownload;
      System.Windows.Forms.Label lblDomainNav;
      System.Windows.Forms.Label lblDirNav;
      System.Windows.Forms.Button browseOutDir;
      System.Windows.Forms.Label lblOutDir;
      System.Windows.Forms.TabPage advancedTab;
      System.Windows.Forms.Label lblMaxQueue;
      System.Windows.Forms.Label lblAdditionalUrls;
      System.Windows.Forms.Label lblLanguage;
      System.Windows.Forms.Label lblUserAgent;
      System.Windows.Forms.Label lblReferrer;
      System.Windows.Forms.Label lblTransfer;
      System.Windows.Forms.Label lblRead;
      System.Windows.Forms.Label lblIdle;
      System.Windows.Forms.Label lblRetries;
      System.Windows.Forms.Label lblQueryStrings;
      System.Windows.Forms.Label lblFileSize;
      System.Windows.Forms.Label lblDepth;
      System.Windows.Forms.TabPage filterTab;
      System.Windows.Forms.Label lblType;
      System.Windows.Forms.Label lblReplacement;
      System.Windows.Forms.Label lblRegex;
      System.Windows.Forms.ColumnHeader filterColumn;
      System.Windows.Forms.ColumnHeader typeColumn;
      System.Windows.Forms.TabPage mimeTypesTab;
      System.Windows.Forms.Label lblMime;
      System.Windows.Forms.Label lblExtension;
      System.Windows.Forms.ColumnHeader extensionColumn;
      System.Windows.Forms.ColumnHeader mimeColumn;
      System.Windows.Forms.ColumnHeader preferredColumn;
      System.Windows.Forms.TabPage progressTab;
      System.Windows.Forms.SplitContainer progressSplitter;
      System.Windows.Forms.Label lblCurrentDownloads;
      System.Windows.Forms.ColumnHeader pathColumn;
      System.Windows.Forms.ColumnHeader speedColumn;
      System.Windows.Forms.ColumnHeader dlSizeColumn;
      System.Windows.Forms.ColumnHeader mimeTypeColumn;
      System.Windows.Forms.ColumnHeader hostColumn;
      System.Windows.Forms.Label lblBytesDownloaded;
      System.Windows.Forms.Label lblDownloadedResources;
      System.Windows.Forms.Label lblSpeed;
      System.Windows.Forms.Label lblQueued;
      System.Windows.Forms.Label lblConnections;
      System.Windows.Forms.Label lblErrors;
      System.Windows.Forms.ColumnHeader urlColumn2;
      System.Windows.Forms.ColumnHeader messageColumn;
      System.Windows.Forms.ColumnHeader referrerColumn;
      System.Windows.Forms.ColumnHeader fatalColumn;
      System.Windows.Forms.ContextMenuStrip errorsMenu;
      System.Windows.Forms.ToolStripMenuItem clearErrorsMenuItem;
      System.Windows.Forms.ColumnHeader sizeColumn;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.crawlerMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.startCrawlingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.pauseCrawlingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.abortCrawlingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.status = new System.Windows.Forms.ToolStripStatusLabel();
      this.speedLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.txtBaseUrls = new System.Windows.Forms.TextBox();
      this.chkGenerateErrorFiles = new System.Windows.Forms.CheckBox();
      this.chkCookies = new System.Windows.Forms.CheckBox();
      this.chkLinkRewrite = new System.Windows.Forms.CheckBox();
      this.txtConnsPerServer = new System.Windows.Forms.TextBox();
      this.txtMaxConnections = new System.Windows.Forms.TextBox();
      this.lstDownload = new System.Windows.Forms.CheckedListBox();
      this.cmbDomainNav = new System.Windows.Forms.ComboBox();
      this.cmbDirNav = new System.Windows.Forms.ComboBox();
      this.btnRevert = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.txtOutDir = new System.Windows.Forms.TextBox();
      this.txtMaxQueue = new System.Windows.Forms.TextBox();
      this.chkEnqueueBaseUrls = new System.Windows.Forms.CheckBox();
      this.txtAdditionalUrls = new System.Windows.Forms.TextBox();
      this.chkClear = new System.Windows.Forms.CheckBox();
      this.cmbLanguage = new System.Windows.Forms.ComboBox();
      this.txtUserAgent = new System.Windows.Forms.TextBox();
      this.txtReferrer = new System.Windows.Forms.TextBox();
      this.chkPassiveFtp = new System.Windows.Forms.CheckBox();
      this.chkNormalizeHosts = new System.Windows.Forms.CheckBox();
      this.chkNormalizeQueries = new System.Windows.Forms.CheckBox();
      this.btnRevert2 = new System.Windows.Forms.Button();
      this.btnApply2 = new System.Windows.Forms.Button();
      this.txtTransferTimeout = new System.Windows.Forms.TextBox();
      this.txtReadTimeout = new System.Windows.Forms.TextBox();
      this.txtIdleTimeout = new System.Windows.Forms.TextBox();
      this.txtFileSize = new System.Windows.Forms.TextBox();
      this.txtRetries = new System.Windows.Forms.TextBox();
      this.txtQueryStrings = new System.Windows.Forms.TextBox();
      this.txtMaxDepth = new System.Windows.Forms.TextBox();
      this.btnDeleteFilter = new System.Windows.Forms.Button();
      this.btnAddFilter = new System.Windows.Forms.Button();
      this.cmbFilterType = new System.Windows.Forms.ComboBox();
      this.txtReplacement = new System.Windows.Forms.TextBox();
      this.txtRegex = new System.Windows.Forms.TextBox();
      this.filters = new System.Windows.Forms.ListView();
      this.btnRevert3 = new System.Windows.Forms.Button();
      this.btnApply3 = new System.Windows.Forms.Button();
      this.chkPreferred = new System.Windows.Forms.CheckBox();
      this.btnDeleteMime = new System.Windows.Forms.Button();
      this.btnAddMime = new System.Windows.Forms.Button();
      this.txtMimeType = new System.Windows.Forms.TextBox();
      this.txtExtension = new System.Windows.Forms.TextBox();
      this.mimeTypes = new System.Windows.Forms.ListView();
      this.btnRevert4 = new System.Windows.Forms.Button();
      this.btnApply4 = new System.Windows.Forms.Button();
      this.downloads = new WebCrawl.Gui.MainForm.ListViewNF();
      this.bytesDownloaded = new System.Windows.Forms.Label();
      this.downloadedResources = new System.Windows.Forms.Label();
      this.speed = new System.Windows.Forms.Label();
      this.queued = new System.Windows.Forms.Label();
      this.connections = new System.Windows.Forms.Label();
      this.recentErrors = new WebCrawl.Gui.MainForm.ListViewNF();
      menuStrip = new System.Windows.Forms.MenuStrip();
      fileMenu = new System.Windows.Forms.ToolStripMenuItem();
      newProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      saveProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      saveProjectAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      menuSep1 = new System.Windows.Forms.ToolStripSeparator();
      statusStrip = new System.Windows.Forms.StatusStrip();
      tabControl = new System.Windows.Forms.TabControl();
      setupTab = new System.Windows.Forms.TabPage();
      lblBaseUrls = new System.Windows.Forms.Label();
      lblConnsPerServer = new System.Windows.Forms.Label();
      lblConns = new System.Windows.Forms.Label();
      lblDownload = new System.Windows.Forms.Label();
      lblDomainNav = new System.Windows.Forms.Label();
      lblDirNav = new System.Windows.Forms.Label();
      browseOutDir = new System.Windows.Forms.Button();
      lblOutDir = new System.Windows.Forms.Label();
      advancedTab = new System.Windows.Forms.TabPage();
      lblMaxQueue = new System.Windows.Forms.Label();
      lblAdditionalUrls = new System.Windows.Forms.Label();
      lblLanguage = new System.Windows.Forms.Label();
      lblUserAgent = new System.Windows.Forms.Label();
      lblReferrer = new System.Windows.Forms.Label();
      lblTransfer = new System.Windows.Forms.Label();
      lblRead = new System.Windows.Forms.Label();
      lblIdle = new System.Windows.Forms.Label();
      lblRetries = new System.Windows.Forms.Label();
      lblQueryStrings = new System.Windows.Forms.Label();
      lblFileSize = new System.Windows.Forms.Label();
      lblDepth = new System.Windows.Forms.Label();
      filterTab = new System.Windows.Forms.TabPage();
      lblType = new System.Windows.Forms.Label();
      lblReplacement = new System.Windows.Forms.Label();
      lblRegex = new System.Windows.Forms.Label();
      filterColumn = new System.Windows.Forms.ColumnHeader();
      typeColumn = new System.Windows.Forms.ColumnHeader();
      mimeTypesTab = new System.Windows.Forms.TabPage();
      lblMime = new System.Windows.Forms.Label();
      lblExtension = new System.Windows.Forms.Label();
      extensionColumn = new System.Windows.Forms.ColumnHeader();
      mimeColumn = new System.Windows.Forms.ColumnHeader();
      preferredColumn = new System.Windows.Forms.ColumnHeader();
      progressTab = new System.Windows.Forms.TabPage();
      progressSplitter = new System.Windows.Forms.SplitContainer();
      lblCurrentDownloads = new System.Windows.Forms.Label();
      pathColumn = new System.Windows.Forms.ColumnHeader();
      speedColumn = new System.Windows.Forms.ColumnHeader();
      dlSizeColumn = new System.Windows.Forms.ColumnHeader();
      mimeTypeColumn = new System.Windows.Forms.ColumnHeader();
      hostColumn = new System.Windows.Forms.ColumnHeader();
      lblBytesDownloaded = new System.Windows.Forms.Label();
      lblDownloadedResources = new System.Windows.Forms.Label();
      lblSpeed = new System.Windows.Forms.Label();
      lblQueued = new System.Windows.Forms.Label();
      lblConnections = new System.Windows.Forms.Label();
      lblErrors = new System.Windows.Forms.Label();
      urlColumn2 = new System.Windows.Forms.ColumnHeader();
      messageColumn = new System.Windows.Forms.ColumnHeader();
      referrerColumn = new System.Windows.Forms.ColumnHeader();
      fatalColumn = new System.Windows.Forms.ColumnHeader();
      errorsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      clearErrorsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      sizeColumn = new System.Windows.Forms.ColumnHeader();
      menuStrip.SuspendLayout();
      statusStrip.SuspendLayout();
      tabControl.SuspendLayout();
      setupTab.SuspendLayout();
      advancedTab.SuspendLayout();
      filterTab.SuspendLayout();
      mimeTypesTab.SuspendLayout();
      progressTab.SuspendLayout();
      progressSplitter.Panel1.SuspendLayout();
      progressSplitter.Panel2.SuspendLayout();
      progressSplitter.SuspendLayout();
      errorsMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            this.crawlerMenu});
      menuStrip.Location = new System.Drawing.Point(0, 0);
      menuStrip.Name = "menuStrip";
      menuStrip.Size = new System.Drawing.Size(517, 24);
      menuStrip.TabIndex = 0;
      // 
      // fileMenu
      // 
      fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            newProjectMenuItem,
            openProjectMenuItem,
            saveProjectMenuItem,
            saveProjectAsMenuItem,
            menuSep1,
            this.exitMenuItem});
      fileMenu.Name = "fileMenu";
      fileMenu.Size = new System.Drawing.Size(35, 20);
      fileMenu.Text = "&File";
      // 
      // newProjectMenuItem
      // 
      newProjectMenuItem.Name = "newProjectMenuItem";
      newProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
      newProjectMenuItem.Size = new System.Drawing.Size(230, 22);
      newProjectMenuItem.Text = "&New Project";
      newProjectMenuItem.Click += new System.EventHandler(this.newProjectMenuItem_Click);
      // 
      // openProjectMenuItem
      // 
      openProjectMenuItem.Name = "openProjectMenuItem";
      openProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      openProjectMenuItem.Size = new System.Drawing.Size(230, 22);
      openProjectMenuItem.Text = "&Open Project...";
      openProjectMenuItem.Click += new System.EventHandler(this.openProjectMenuItem_Click);
      // 
      // saveProjectMenuItem
      // 
      saveProjectMenuItem.Name = "saveProjectMenuItem";
      saveProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      saveProjectMenuItem.Size = new System.Drawing.Size(230, 22);
      saveProjectMenuItem.Text = "&Save Project";
      saveProjectMenuItem.Click += new System.EventHandler(this.saveProjectMenuItem_Click);
      // 
      // saveProjectAsMenuItem
      // 
      saveProjectAsMenuItem.Name = "saveProjectAsMenuItem";
      saveProjectAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
      saveProjectAsMenuItem.Size = new System.Drawing.Size(230, 22);
      saveProjectAsMenuItem.Text = "Save Project &As...";
      saveProjectAsMenuItem.Click += new System.EventHandler(this.saveProjectAsMenuItem_Click);
      // 
      // menuSep1
      // 
      menuSep1.Name = "menuSep1";
      menuSep1.Size = new System.Drawing.Size(227, 6);
      // 
      // exitMenuItem
      // 
      this.exitMenuItem.Name = "exitMenuItem";
      this.exitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
      this.exitMenuItem.Size = new System.Drawing.Size(230, 22);
      this.exitMenuItem.Text = "E&xit";
      this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
      // 
      // crawlerMenu
      // 
      this.crawlerMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startCrawlingMenuItem,
            this.pauseCrawlingMenuItem,
            this.abortCrawlingMenuItem});
      this.crawlerMenu.Name = "crawlerMenu";
      this.crawlerMenu.Size = new System.Drawing.Size(56, 20);
      this.crawlerMenu.Text = "&Crawler";
      this.crawlerMenu.DropDownOpening += new System.EventHandler(this.crawlerMenu_DropDownOpening);
      // 
      // startCrawlingMenuItem
      // 
      this.startCrawlingMenuItem.Name = "startCrawlingMenuItem";
      this.startCrawlingMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
      this.startCrawlingMenuItem.Size = new System.Drawing.Size(161, 22);
      this.startCrawlingMenuItem.Text = "&Start Crawling";
      this.startCrawlingMenuItem.Click += new System.EventHandler(this.startCrawlingMenuItem_Click);
      // 
      // pauseCrawlingMenuItem
      // 
      this.pauseCrawlingMenuItem.Name = "pauseCrawlingMenuItem";
      this.pauseCrawlingMenuItem.Size = new System.Drawing.Size(161, 22);
      this.pauseCrawlingMenuItem.Text = "&Pause Crawling";
      this.pauseCrawlingMenuItem.Click += new System.EventHandler(this.pauseCrawlingMenuItem_Click);
      // 
      // abortCrawlingMenuItem
      // 
      this.abortCrawlingMenuItem.Name = "abortCrawlingMenuItem";
      this.abortCrawlingMenuItem.Size = new System.Drawing.Size(161, 22);
      this.abortCrawlingMenuItem.Text = "&Abort Crawling";
      this.abortCrawlingMenuItem.Click += new System.EventHandler(this.abortCrawlingMenuItem_Click);
      // 
      // statusStrip
      // 
      statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status,
            this.speedLabel});
      statusStrip.Location = new System.Drawing.Point(0, 321);
      statusStrip.Name = "statusStrip";
      statusStrip.Size = new System.Drawing.Size(517, 22);
      statusStrip.TabIndex = 1;
      // 
      // status
      // 
      this.status.Name = "status";
      this.status.Size = new System.Drawing.Size(451, 17);
      this.status.Spring = true;
      this.status.Text = "Welcome.";
      this.status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // speedLabel
      // 
      this.speedLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
      this.speedLabel.Name = "speedLabel";
      this.speedLabel.Size = new System.Drawing.Size(51, 17);
      this.speedLabel.Text = "Stopped";
      // 
      // tabControl
      // 
      tabControl.Controls.Add(setupTab);
      tabControl.Controls.Add(advancedTab);
      tabControl.Controls.Add(filterTab);
      tabControl.Controls.Add(mimeTypesTab);
      tabControl.Controls.Add(progressTab);
      tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      tabControl.Location = new System.Drawing.Point(0, 24);
      tabControl.Name = "tabControl";
      tabControl.SelectedIndex = 0;
      tabControl.Size = new System.Drawing.Size(517, 297);
      tabControl.TabIndex = 2;
      // 
      // setupTab
      // 
      setupTab.Controls.Add(this.txtBaseUrls);
      setupTab.Controls.Add(lblBaseUrls);
      setupTab.Controls.Add(this.chkGenerateErrorFiles);
      setupTab.Controls.Add(this.chkCookies);
      setupTab.Controls.Add(this.chkLinkRewrite);
      setupTab.Controls.Add(this.txtConnsPerServer);
      setupTab.Controls.Add(lblConnsPerServer);
      setupTab.Controls.Add(this.txtMaxConnections);
      setupTab.Controls.Add(lblConns);
      setupTab.Controls.Add(this.lstDownload);
      setupTab.Controls.Add(lblDownload);
      setupTab.Controls.Add(this.cmbDomainNav);
      setupTab.Controls.Add(lblDomainNav);
      setupTab.Controls.Add(this.cmbDirNav);
      setupTab.Controls.Add(lblDirNav);
      setupTab.Controls.Add(this.btnRevert);
      setupTab.Controls.Add(this.btnApply);
      setupTab.Controls.Add(browseOutDir);
      setupTab.Controls.Add(this.txtOutDir);
      setupTab.Controls.Add(lblOutDir);
      setupTab.Location = new System.Drawing.Point(4, 22);
      setupTab.Name = "setupTab";
      setupTab.Padding = new System.Windows.Forms.Padding(3);
      setupTab.Size = new System.Drawing.Size(509, 271);
      setupTab.TabIndex = 0;
      setupTab.Text = "Setup";
      setupTab.UseVisualStyleBackColor = true;
      // 
      // txtBaseUrls
      // 
      this.txtBaseUrls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtBaseUrls.Location = new System.Drawing.Point(109, 6);
      this.txtBaseUrls.Multiline = true;
      this.txtBaseUrls.Name = "txtBaseUrls";
      this.txtBaseUrls.Size = new System.Drawing.Size(391, 58);
      this.txtBaseUrls.TabIndex = 1;
      this.txtBaseUrls.WordWrap = false;
      this.txtBaseUrls.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtBaseUrls.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblBaseUrls
      // 
      lblBaseUrls.AutoSize = true;
      lblBaseUrls.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      lblBaseUrls.Location = new System.Drawing.Point(3, 6);
      lblBaseUrls.Name = "lblBaseUrls";
      lblBaseUrls.Size = new System.Drawing.Size(82, 26);
      lblBaseUrls.TabIndex = 0;
      lblBaseUrls.Text = "Base &Urls\n(one per line)";
      lblBaseUrls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // chkGenerateErrorFiles
      // 
      this.chkGenerateErrorFiles.AutoSize = true;
      this.chkGenerateErrorFiles.Location = new System.Drawing.Point(261, 194);
      this.chkGenerateErrorFiles.Name = "chkGenerateErrorFiles";
      this.chkGenerateErrorFiles.Size = new System.Drawing.Size(119, 17);
      this.chkGenerateErrorFiles.TabIndex = 17;
      this.chkGenerateErrorFiles.Text = "Generate &Error Files";
      this.chkGenerateErrorFiles.UseVisualStyleBackColor = true;
      this.chkGenerateErrorFiles.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkCookies
      // 
      this.chkCookies.AutoSize = true;
      this.chkCookies.Location = new System.Drawing.Point(261, 171);
      this.chkCookies.Name = "chkCookies";
      this.chkCookies.Size = new System.Drawing.Size(64, 17);
      this.chkCookies.TabIndex = 16;
      this.chkCookies.Text = "&Cookies";
      this.chkCookies.UseVisualStyleBackColor = true;
      this.chkCookies.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkLinkRewrite
      // 
      this.chkLinkRewrite.AutoSize = true;
      this.chkLinkRewrite.Location = new System.Drawing.Point(261, 148);
      this.chkLinkRewrite.Name = "chkLinkRewrite";
      this.chkLinkRewrite.Size = new System.Drawing.Size(93, 17);
      this.chkLinkRewrite.TabIndex = 15;
      this.chkLinkRewrite.Text = "Link &Rewriting";
      this.chkLinkRewrite.UseVisualStyleBackColor = true;
      this.chkLinkRewrite.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // txtConnsPerServer
      // 
      this.txtConnsPerServer.Location = new System.Drawing.Point(293, 121);
      this.txtConnsPerServer.Name = "txtConnsPerServer";
      this.txtConnsPerServer.Size = new System.Drawing.Size(39, 20);
      this.txtConnsPerServer.TabIndex = 12;
      this.txtConnsPerServer.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtConnsPerServer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblConnsPerServer
      // 
      lblConnsPerServer.AutoSize = true;
      lblConnsPerServer.Location = new System.Drawing.Point(172, 125);
      lblConnsPerServer.Name = "lblConnsPerServer";
      lblConnsPerServer.Size = new System.Drawing.Size(119, 13);
      lblConnsPerServer.TabIndex = 11;
      lblConnsPerServer.Text = "Connections Per Server";
      lblConnsPerServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtMaxConnections
      // 
      this.txtMaxConnections.Location = new System.Drawing.Point(109, 121);
      this.txtMaxConnections.Name = "txtMaxConnections";
      this.txtMaxConnections.Size = new System.Drawing.Size(39, 20);
      this.txtMaxConnections.TabIndex = 10;
      this.txtMaxConnections.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtMaxConnections.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblConns
      // 
      lblConns.AutoSize = true;
      lblConns.Location = new System.Drawing.Point(3, 125);
      lblConns.Name = "lblConns";
      lblConns.Size = new System.Drawing.Size(89, 13);
      lblConns.TabIndex = 9;
      lblConns.Text = "Max Connections";
      lblConns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lstDownload
      // 
      this.lstDownload.CheckOnClick = true;
      this.lstDownload.FormattingEnabled = true;
      this.lstDownload.Location = new System.Drawing.Point(109, 147);
      this.lstDownload.Name = "lstDownload";
      this.lstDownload.Size = new System.Drawing.Size(146, 79);
      this.lstDownload.TabIndex = 14;
      this.lstDownload.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.download_ItemCheck);
      // 
      // lblDownload
      // 
      lblDownload.AutoSize = true;
      lblDownload.Location = new System.Drawing.Point(3, 150);
      lblDownload.Name = "lblDownload";
      lblDownload.Size = new System.Drawing.Size(55, 13);
      lblDownload.TabIndex = 13;
      lblDownload.Text = "&Download";
      lblDownload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cmbDomainNav
      // 
      this.cmbDomainNav.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbDomainNav.FormattingEnabled = true;
      this.cmbDomainNav.Location = new System.Drawing.Point(293, 95);
      this.cmbDomainNav.Name = "cmbDomainNav";
      this.cmbDomainNav.Size = new System.Drawing.Size(121, 21);
      this.cmbDomainNav.TabIndex = 8;
      this.cmbDomainNav.SelectedIndexChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // lblDomainNav
      // 
      lblDomainNav.AutoSize = true;
      lblDomainNav.Location = new System.Drawing.Point(194, 99);
      lblDomainNav.Name = "lblDomainNav";
      lblDomainNav.Size = new System.Drawing.Size(97, 13);
      lblDomainNav.TabIndex = 7;
      lblDomainNav.Text = "Domain Navigation";
      lblDomainNav.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // cmbDirNav
      // 
      this.cmbDirNav.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbDirNav.FormattingEnabled = true;
      this.cmbDirNav.Location = new System.Drawing.Point(109, 95);
      this.cmbDirNav.Name = "cmbDirNav";
      this.cmbDirNav.Size = new System.Drawing.Size(83, 21);
      this.cmbDirNav.TabIndex = 6;
      this.cmbDirNav.SelectedIndexChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // lblDirNav
      // 
      lblDirNav.AutoSize = true;
      lblDirNav.Location = new System.Drawing.Point(3, 99);
      lblDirNav.Name = "lblDirNav";
      lblDirNav.Size = new System.Drawing.Size(103, 13);
      lblDirNav.TabIndex = 5;
      lblDirNav.Text = "Directory Navigation";
      lblDirNav.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnRevert
      // 
      this.btnRevert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert.Location = new System.Drawing.Point(428, 242);
      this.btnRevert.Name = "btnRevert";
      this.btnRevert.Size = new System.Drawing.Size(75, 23);
      this.btnRevert.TabIndex = 19;
      this.btnRevert.Text = "Revert";
      this.btnRevert.UseVisualStyleBackColor = true;
      this.btnRevert.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply
      // 
      this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply.Location = new System.Drawing.Point(347, 242);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(75, 23);
      this.btnApply.TabIndex = 18;
      this.btnApply.Text = "&Apply";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // browseOutDir
      // 
      browseOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      browseOutDir.Location = new System.Drawing.Point(425, 70);
      browseOutDir.Name = "browseOutDir";
      browseOutDir.Size = new System.Drawing.Size(75, 20);
      browseOutDir.TabIndex = 4;
      browseOutDir.Text = "&Browse...";
      browseOutDir.UseVisualStyleBackColor = true;
      browseOutDir.Click += new System.EventHandler(this.browseOutDir_Click);
      // 
      // txtOutDir
      // 
      this.txtOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOutDir.Location = new System.Drawing.Point(109, 70);
      this.txtOutDir.Name = "txtOutDir";
      this.txtOutDir.Size = new System.Drawing.Size(310, 20);
      this.txtOutDir.TabIndex = 3;
      this.txtOutDir.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtOutDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblOutDir
      // 
      lblOutDir.AutoSize = true;
      lblOutDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      lblOutDir.Location = new System.Drawing.Point(3, 74);
      lblOutDir.Name = "lblOutDir";
      lblOutDir.Size = new System.Drawing.Size(100, 13);
      lblOutDir.TabIndex = 2;
      lblOutDir.Text = "&Output Directory";
      lblOutDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // advancedTab
      // 
      advancedTab.Controls.Add(this.txtMaxQueue);
      advancedTab.Controls.Add(lblMaxQueue);
      advancedTab.Controls.Add(this.chkEnqueueBaseUrls);
      advancedTab.Controls.Add(this.txtAdditionalUrls);
      advancedTab.Controls.Add(lblAdditionalUrls);
      advancedTab.Controls.Add(this.chkClear);
      advancedTab.Controls.Add(this.cmbLanguage);
      advancedTab.Controls.Add(lblLanguage);
      advancedTab.Controls.Add(this.txtUserAgent);
      advancedTab.Controls.Add(lblUserAgent);
      advancedTab.Controls.Add(this.txtReferrer);
      advancedTab.Controls.Add(lblReferrer);
      advancedTab.Controls.Add(this.chkPassiveFtp);
      advancedTab.Controls.Add(this.chkNormalizeHosts);
      advancedTab.Controls.Add(this.chkNormalizeQueries);
      advancedTab.Controls.Add(this.btnRevert2);
      advancedTab.Controls.Add(this.btnApply2);
      advancedTab.Controls.Add(this.txtTransferTimeout);
      advancedTab.Controls.Add(lblTransfer);
      advancedTab.Controls.Add(this.txtReadTimeout);
      advancedTab.Controls.Add(lblRead);
      advancedTab.Controls.Add(this.txtIdleTimeout);
      advancedTab.Controls.Add(lblIdle);
      advancedTab.Controls.Add(this.txtFileSize);
      advancedTab.Controls.Add(this.txtRetries);
      advancedTab.Controls.Add(lblRetries);
      advancedTab.Controls.Add(this.txtQueryStrings);
      advancedTab.Controls.Add(lblQueryStrings);
      advancedTab.Controls.Add(lblFileSize);
      advancedTab.Controls.Add(this.txtMaxDepth);
      advancedTab.Controls.Add(lblDepth);
      advancedTab.Location = new System.Drawing.Point(4, 22);
      advancedTab.Name = "advancedTab";
      advancedTab.Padding = new System.Windows.Forms.Padding(3);
      advancedTab.Size = new System.Drawing.Size(423, 271);
      advancedTab.TabIndex = 3;
      advancedTab.Text = "Advanced Setup";
      advancedTab.UseVisualStyleBackColor = true;
      // 
      // txtMaxQueue
      // 
      this.txtMaxQueue.Location = new System.Drawing.Point(204, 107);
      this.txtMaxQueue.Name = "txtMaxQueue";
      this.txtMaxQueue.Size = new System.Drawing.Size(54, 20);
      this.txtMaxQueue.TabIndex = 23;
      // 
      // lblMaxQueue
      // 
      lblMaxQueue.AutoSize = true;
      lblMaxQueue.Location = new System.Drawing.Point(114, 111);
      lblMaxQueue.Name = "lblMaxQueue";
      lblMaxQueue.Size = new System.Drawing.Size(85, 13);
      lblMaxQueue.TabIndex = 22;
      lblMaxQueue.Text = "Max Queue Size";
      lblMaxQueue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // chkEnqueueBaseUrls
      // 
      this.chkEnqueueBaseUrls.AutoSize = true;
      this.chkEnqueueBaseUrls.Location = new System.Drawing.Point(160, 205);
      this.chkEnqueueBaseUrls.Name = "chkEnqueueBaseUrls";
      this.chkEnqueueBaseUrls.Size = new System.Drawing.Size(117, 17);
      this.chkEnqueueBaseUrls.TabIndex = 41;
      this.chkEnqueueBaseUrls.Text = "Enqueue Base Urls";
      this.chkEnqueueBaseUrls.UseVisualStyleBackColor = true;
      this.chkEnqueueBaseUrls.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // txtAdditionalUrls
      // 
      this.txtAdditionalUrls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtAdditionalUrls.Location = new System.Drawing.Point(86, 6);
      this.txtAdditionalUrls.Multiline = true;
      this.txtAdditionalUrls.Name = "txtAdditionalUrls";
      this.txtAdditionalUrls.Size = new System.Drawing.Size(328, 43);
      this.txtAdditionalUrls.TabIndex = 1;
      this.txtAdditionalUrls.WordWrap = false;
      this.txtAdditionalUrls.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtAdditionalUrls.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblAdditionalUrls
      // 
      lblAdditionalUrls.AutoSize = true;
      lblAdditionalUrls.Location = new System.Drawing.Point(6, 9);
      lblAdditionalUrls.Name = "lblAdditionalUrls";
      lblAdditionalUrls.Size = new System.Drawing.Size(74, 26);
      lblAdditionalUrls.TabIndex = 0;
      lblAdditionalUrls.Text = "Additional &Urls\n(one per line)";
      lblAdditionalUrls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // chkClear
      // 
      this.chkClear.AutoSize = true;
      this.chkClear.Location = new System.Drawing.Point(6, 251);
      this.chkClear.Name = "chkClear";
      this.chkClear.Size = new System.Drawing.Size(209, 17);
      this.chkClear.TabIndex = 38;
      this.chkClear.Text = "Clear Download Directory Before Crawl";
      this.chkClear.UseVisualStyleBackColor = true;
      this.chkClear.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // cmbLanguage
      // 
      this.cmbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbLanguage.FormattingEnabled = true;
      this.cmbLanguage.Location = new System.Drawing.Point(261, 180);
      this.cmbLanguage.Name = "cmbLanguage";
      this.cmbLanguage.Size = new System.Drawing.Size(153, 21);
      this.cmbLanguage.TabIndex = 40;
      this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // lblLanguage
      // 
      lblLanguage.AutoSize = true;
      lblLanguage.Location = new System.Drawing.Point(157, 184);
      lblLanguage.Name = "lblLanguage";
      lblLanguage.Size = new System.Drawing.Size(101, 13);
      lblLanguage.TabIndex = 39;
      lblLanguage.Text = "Preferred Language";
      lblLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtUserAgent
      // 
      this.txtUserAgent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtUserAgent.Location = new System.Drawing.Point(89, 156);
      this.txtUserAgent.Name = "txtUserAgent";
      this.txtUserAgent.Size = new System.Drawing.Size(325, 20);
      this.txtUserAgent.TabIndex = 33;
      this.txtUserAgent.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtUserAgent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblUserAgent
      // 
      lblUserAgent.AutoSize = true;
      lblUserAgent.Location = new System.Drawing.Point(3, 160);
      lblUserAgent.Name = "lblUserAgent";
      lblUserAgent.Size = new System.Drawing.Size(60, 13);
      lblUserAgent.TabIndex = 32;
      lblUserAgent.Text = "User Agent";
      lblUserAgent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtReferrer
      // 
      this.txtReferrer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReferrer.Location = new System.Drawing.Point(89, 131);
      this.txtReferrer.Name = "txtReferrer";
      this.txtReferrer.Size = new System.Drawing.Size(325, 20);
      this.txtReferrer.TabIndex = 31;
      this.txtReferrer.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtReferrer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblReferrer
      // 
      lblReferrer.AutoSize = true;
      lblReferrer.Location = new System.Drawing.Point(3, 135);
      lblReferrer.Name = "lblReferrer";
      lblReferrer.Size = new System.Drawing.Size(82, 13);
      lblReferrer.TabIndex = 30;
      lblReferrer.Text = "Default Referrer";
      lblReferrer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // chkPassiveFtp
      // 
      this.chkPassiveFtp.AutoSize = true;
      this.chkPassiveFtp.Location = new System.Drawing.Point(6, 228);
      this.chkPassiveFtp.Name = "chkPassiveFtp";
      this.chkPassiveFtp.Size = new System.Drawing.Size(86, 17);
      this.chkPassiveFtp.TabIndex = 37;
      this.chkPassiveFtp.Text = "Passive FTP";
      this.chkPassiveFtp.UseVisualStyleBackColor = true;
      this.chkPassiveFtp.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkNormalizeHosts
      // 
      this.chkNormalizeHosts.AutoSize = true;
      this.chkNormalizeHosts.Location = new System.Drawing.Point(6, 205);
      this.chkNormalizeHosts.Name = "chkNormalizeHosts";
      this.chkNormalizeHosts.Size = new System.Drawing.Size(133, 17);
      this.chkNormalizeHosts.TabIndex = 36;
      this.chkNormalizeHosts.Text = "Normalize Host Names";
      this.chkNormalizeHosts.UseVisualStyleBackColor = true;
      this.chkNormalizeHosts.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkNormalizeQueries
      // 
      this.chkNormalizeQueries.AutoSize = true;
      this.chkNormalizeQueries.Location = new System.Drawing.Point(6, 182);
      this.chkNormalizeQueries.Name = "chkNormalizeQueries";
      this.chkNormalizeQueries.Size = new System.Drawing.Size(138, 17);
      this.chkNormalizeQueries.TabIndex = 35;
      this.chkNormalizeQueries.Text = "Normalize Query Strings";
      this.chkNormalizeQueries.UseVisualStyleBackColor = true;
      this.chkNormalizeQueries.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // btnRevert2
      // 
      this.btnRevert2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert2.Location = new System.Drawing.Point(342, 242);
      this.btnRevert2.Name = "btnRevert2";
      this.btnRevert2.Size = new System.Drawing.Size(75, 23);
      this.btnRevert2.TabIndex = 60;
      this.btnRevert2.Text = "Revert";
      this.btnRevert2.UseVisualStyleBackColor = true;
      this.btnRevert2.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply2
      // 
      this.btnApply2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply2.Location = new System.Drawing.Point(261, 242);
      this.btnApply2.Name = "btnApply2";
      this.btnApply2.Size = new System.Drawing.Size(75, 23);
      this.btnApply2.TabIndex = 50;
      this.btnApply2.Text = "&Apply";
      this.btnApply2.UseVisualStyleBackColor = true;
      this.btnApply2.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // txtTransferTimeout
      // 
      this.txtTransferTimeout.Location = new System.Drawing.Point(338, 81);
      this.txtTransferTimeout.Name = "txtTransferTimeout";
      this.txtTransferTimeout.Size = new System.Drawing.Size(35, 20);
      this.txtTransferTimeout.TabIndex = 15;
      this.txtTransferTimeout.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtTransferTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblTransfer
      // 
      lblTransfer.AutoSize = true;
      lblTransfer.Location = new System.Drawing.Point(247, 85);
      lblTransfer.Name = "lblTransfer";
      lblTransfer.Size = new System.Drawing.Size(87, 13);
      lblTransfer.TabIndex = 14;
      lblTransfer.Text = "Transfer Timeout";
      lblTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtReadTimeout
      // 
      this.txtReadTimeout.Location = new System.Drawing.Point(204, 81);
      this.txtReadTimeout.Name = "txtReadTimeout";
      this.txtReadTimeout.Size = new System.Drawing.Size(35, 20);
      this.txtReadTimeout.TabIndex = 13;
      this.txtReadTimeout.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtReadTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblRead
      // 
      lblRead.AutoSize = true;
      lblRead.Location = new System.Drawing.Point(114, 85);
      lblRead.Name = "lblRead";
      lblRead.Size = new System.Drawing.Size(74, 13);
      lblRead.TabIndex = 12;
      lblRead.Text = "Read Timeout";
      lblRead.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtIdleTimeout
      // 
      this.txtIdleTimeout.Location = new System.Drawing.Point(73, 81);
      this.txtIdleTimeout.Name = "txtIdleTimeout";
      this.txtIdleTimeout.Size = new System.Drawing.Size(35, 20);
      this.txtIdleTimeout.TabIndex = 11;
      this.txtIdleTimeout.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtIdleTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblIdle
      // 
      lblIdle.AutoSize = true;
      lblIdle.Location = new System.Drawing.Point(3, 85);
      lblIdle.Name = "lblIdle";
      lblIdle.Size = new System.Drawing.Size(65, 13);
      lblIdle.TabIndex = 10;
      lblIdle.Text = "Idle Timeout";
      lblIdle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtFileSize
      // 
      this.txtFileSize.Location = new System.Drawing.Point(73, 55);
      this.txtFileSize.Name = "txtFileSize";
      this.txtFileSize.Size = new System.Drawing.Size(35, 20);
      this.txtFileSize.TabIndex = 3;
      this.txtFileSize.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtFileSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // txtRetries
      // 
      this.txtRetries.Location = new System.Drawing.Point(73, 106);
      this.txtRetries.Name = "txtRetries";
      this.txtRetries.Size = new System.Drawing.Size(35, 20);
      this.txtRetries.TabIndex = 21;
      this.txtRetries.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtRetries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblRetries
      // 
      lblRetries.AutoSize = true;
      lblRetries.Location = new System.Drawing.Point(3, 110);
      lblRetries.Name = "lblRetries";
      lblRetries.Size = new System.Drawing.Size(63, 13);
      lblRetries.TabIndex = 20;
      lblRetries.Text = "Max Retries";
      lblRetries.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtQueryStrings
      // 
      this.txtQueryStrings.Location = new System.Drawing.Point(204, 55);
      this.txtQueryStrings.Name = "txtQueryStrings";
      this.txtQueryStrings.Size = new System.Drawing.Size(35, 20);
      this.txtQueryStrings.TabIndex = 5;
      this.txtQueryStrings.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtQueryStrings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblQueryStrings
      // 
      lblQueryStrings.AutoSize = true;
      lblQueryStrings.Location = new System.Drawing.Point(114, 59);
      lblQueryStrings.Name = "lblQueryStrings";
      lblQueryStrings.Size = new System.Drawing.Size(89, 13);
      lblQueryStrings.TabIndex = 4;
      lblQueryStrings.Text = "Query String Limit";
      lblQueryStrings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblFileSize
      // 
      lblFileSize.AutoSize = true;
      lblFileSize.Location = new System.Drawing.Point(3, 59);
      lblFileSize.Name = "lblFileSize";
      lblFileSize.Size = new System.Drawing.Size(69, 13);
      lblFileSize.TabIndex = 2;
      lblFileSize.Text = "Max File Size";
      lblFileSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtMaxDepth
      // 
      this.txtMaxDepth.Location = new System.Drawing.Point(338, 56);
      this.txtMaxDepth.Name = "txtMaxDepth";
      this.txtMaxDepth.Size = new System.Drawing.Size(35, 20);
      this.txtMaxDepth.TabIndex = 7;
      this.txtMaxDepth.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.txtMaxDepth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblDepth
      // 
      lblDepth.AutoSize = true;
      lblDepth.Location = new System.Drawing.Point(247, 59);
      lblDepth.Name = "lblDepth";
      lblDepth.Size = new System.Drawing.Size(82, 13);
      lblDepth.TabIndex = 6;
      lblDepth.Text = "Max Link Depth";
      lblDepth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // filterTab
      // 
      filterTab.Controls.Add(this.btnDeleteFilter);
      filterTab.Controls.Add(this.btnAddFilter);
      filterTab.Controls.Add(this.cmbFilterType);
      filterTab.Controls.Add(lblType);
      filterTab.Controls.Add(this.txtReplacement);
      filterTab.Controls.Add(lblReplacement);
      filterTab.Controls.Add(this.txtRegex);
      filterTab.Controls.Add(lblRegex);
      filterTab.Controls.Add(this.filters);
      filterTab.Controls.Add(this.btnRevert3);
      filterTab.Controls.Add(this.btnApply3);
      filterTab.Location = new System.Drawing.Point(4, 22);
      filterTab.Name = "filterTab";
      filterTab.Padding = new System.Windows.Forms.Padding(3);
      filterTab.Size = new System.Drawing.Size(423, 271);
      filterTab.TabIndex = 4;
      filterTab.Text = "Filters";
      filterTab.UseVisualStyleBackColor = true;
      // 
      // btnDeleteFilter
      // 
      this.btnDeleteFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnDeleteFilter.Enabled = false;
      this.btnDeleteFilter.Location = new System.Drawing.Point(69, 228);
      this.btnDeleteFilter.Name = "btnDeleteFilter";
      this.btnDeleteFilter.Size = new System.Drawing.Size(59, 23);
      this.btnDeleteFilter.TabIndex = 8;
      this.btnDeleteFilter.Text = "&Delete";
      this.btnDeleteFilter.UseVisualStyleBackColor = true;
      this.btnDeleteFilter.Click += new System.EventHandler(this.btnDeleteFilter_Click);
      // 
      // btnAddFilter
      // 
      this.btnAddFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAddFilter.Location = new System.Drawing.Point(3, 228);
      this.btnAddFilter.Name = "btnAddFilter";
      this.btnAddFilter.Size = new System.Drawing.Size(59, 23);
      this.btnAddFilter.TabIndex = 7;
      this.btnAddFilter.Text = "Add &New";
      this.btnAddFilter.UseVisualStyleBackColor = true;
      this.btnAddFilter.Click += new System.EventHandler(this.btnAddFilter_Click);
      // 
      // cmbFilterType
      // 
      this.cmbFilterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.cmbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbFilterType.FormattingEnabled = true;
      this.cmbFilterType.Items.AddRange(new object[] {
            "Must Match",
            "Must Not Match",
            "Change",
            "Content"});
      this.cmbFilterType.Location = new System.Drawing.Point(77, 201);
      this.cmbFilterType.Name = "cmbFilterType";
      this.cmbFilterType.Size = new System.Drawing.Size(104, 21);
      this.cmbFilterType.TabIndex = 6;
      this.cmbFilterType.SelectedIndexChanged += new System.EventHandler(this.filterType_SelectedIndexChanged);
      // 
      // lblType
      // 
      lblType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblType.AutoSize = true;
      lblType.Location = new System.Drawing.Point(3, 205);
      lblType.Name = "lblType";
      lblType.Size = new System.Drawing.Size(31, 13);
      lblType.TabIndex = 5;
      lblType.Text = "&Type";
      lblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtReplacement
      // 
      this.txtReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReplacement.Enabled = false;
      this.txtReplacement.Location = new System.Drawing.Point(77, 177);
      this.txtReplacement.Name = "txtReplacement";
      this.txtReplacement.Size = new System.Drawing.Size(342, 20);
      this.txtReplacement.TabIndex = 4;
      this.txtReplacement.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblReplacement
      // 
      lblReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblReplacement.AutoSize = true;
      lblReplacement.Location = new System.Drawing.Point(1, 181);
      lblReplacement.Name = "lblReplacement";
      lblReplacement.Size = new System.Drawing.Size(70, 13);
      lblReplacement.TabIndex = 3;
      lblReplacement.Text = "Re&placement";
      lblReplacement.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtRegex
      // 
      this.txtRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtRegex.Location = new System.Drawing.Point(77, 153);
      this.txtRegex.Name = "txtRegex";
      this.txtRegex.Size = new System.Drawing.Size(342, 20);
      this.txtRegex.TabIndex = 2;
      this.txtRegex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblRegex
      // 
      lblRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblRegex.AutoSize = true;
      lblRegex.Location = new System.Drawing.Point(1, 157);
      lblRegex.Name = "lblRegex";
      lblRegex.Size = new System.Drawing.Size(38, 13);
      lblRegex.TabIndex = 1;
      lblRegex.Text = "&Regex";
      lblRegex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // filters
      // 
      this.filters.AllowColumnReorder = true;
      this.filters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.filters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            filterColumn,
            typeColumn});
      this.filters.FullRowSelect = true;
      this.filters.HideSelection = false;
      this.filters.Location = new System.Drawing.Point(4, 4);
      this.filters.MultiSelect = false;
      this.filters.Name = "filters";
      this.filters.Size = new System.Drawing.Size(415, 144);
      this.filters.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.filters.TabIndex = 0;
      this.filters.UseCompatibleStateImageBehavior = false;
      this.filters.View = System.Windows.Forms.View.Details;
      this.filters.SelectedIndexChanged += new System.EventHandler(this.filters_SelectedIndexChanged);
      this.filters.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
      // 
      // filterColumn
      // 
      filterColumn.Text = "Filter";
      filterColumn.Width = 305;
      // 
      // typeColumn
      // 
      typeColumn.Text = "Type";
      typeColumn.Width = 90;
      // 
      // btnRevert3
      // 
      this.btnRevert3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert3.Location = new System.Drawing.Point(342, 242);
      this.btnRevert3.Name = "btnRevert3";
      this.btnRevert3.Size = new System.Drawing.Size(75, 23);
      this.btnRevert3.TabIndex = 10;
      this.btnRevert3.Text = "Revert";
      this.btnRevert3.UseVisualStyleBackColor = true;
      this.btnRevert3.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply3
      // 
      this.btnApply3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply3.Location = new System.Drawing.Point(261, 242);
      this.btnApply3.Name = "btnApply3";
      this.btnApply3.Size = new System.Drawing.Size(75, 23);
      this.btnApply3.TabIndex = 9;
      this.btnApply3.Text = "&Apply";
      this.btnApply3.UseVisualStyleBackColor = true;
      this.btnApply3.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // mimeTypesTab
      // 
      mimeTypesTab.Controls.Add(this.chkPreferred);
      mimeTypesTab.Controls.Add(this.btnDeleteMime);
      mimeTypesTab.Controls.Add(this.btnAddMime);
      mimeTypesTab.Controls.Add(this.txtMimeType);
      mimeTypesTab.Controls.Add(this.txtExtension);
      mimeTypesTab.Controls.Add(lblMime);
      mimeTypesTab.Controls.Add(lblExtension);
      mimeTypesTab.Controls.Add(this.mimeTypes);
      mimeTypesTab.Controls.Add(this.btnRevert4);
      mimeTypesTab.Controls.Add(this.btnApply4);
      mimeTypesTab.Location = new System.Drawing.Point(4, 22);
      mimeTypesTab.Name = "mimeTypesTab";
      mimeTypesTab.Padding = new System.Windows.Forms.Padding(3);
      mimeTypesTab.Size = new System.Drawing.Size(423, 271);
      mimeTypesTab.TabIndex = 1;
      mimeTypesTab.Text = "MIME Types";
      mimeTypesTab.UseVisualStyleBackColor = true;
      // 
      // chkPreferred
      // 
      this.chkPreferred.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.chkPreferred.AutoSize = true;
      this.chkPreferred.Location = new System.Drawing.Point(263, 54);
      this.chkPreferred.Name = "chkPreferred";
      this.chkPreferred.Size = new System.Drawing.Size(129, 17);
      this.chkPreferred.TabIndex = 5;
      this.chkPreferred.Text = "Is Preferred Extension";
      this.chkPreferred.UseVisualStyleBackColor = true;
      // 
      // btnDeleteMime
      // 
      this.btnDeleteMime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDeleteMime.Enabled = false;
      this.btnDeleteMime.Location = new System.Drawing.Point(327, 75);
      this.btnDeleteMime.Name = "btnDeleteMime";
      this.btnDeleteMime.Size = new System.Drawing.Size(59, 23);
      this.btnDeleteMime.TabIndex = 7;
      this.btnDeleteMime.Text = "&Delete";
      this.btnDeleteMime.UseVisualStyleBackColor = true;
      this.btnDeleteMime.Click += new System.EventHandler(this.btnDeleteMime_Click);
      // 
      // btnAddMime
      // 
      this.btnAddMime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAddMime.Enabled = false;
      this.btnAddMime.Location = new System.Drawing.Point(261, 75);
      this.btnAddMime.Name = "btnAddMime";
      this.btnAddMime.Size = new System.Drawing.Size(59, 23);
      this.btnAddMime.TabIndex = 6;
      this.btnAddMime.Text = "Add &New";
      this.btnAddMime.UseVisualStyleBackColor = true;
      this.btnAddMime.Click += new System.EventHandler(this.btnAddMime_Click);
      // 
      // txtMimeType
      // 
      this.txtMimeType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMimeType.Location = new System.Drawing.Point(320, 28);
      this.txtMimeType.Name = "txtMimeType";
      this.txtMimeType.Size = new System.Drawing.Size(97, 20);
      this.txtMimeType.TabIndex = 4;
      this.txtMimeType.TextChanged += new System.EventHandler(this.txtMimeType_TextChanged);
      this.txtMimeType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // txtExtension
      // 
      this.txtExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtExtension.Location = new System.Drawing.Point(320, 4);
      this.txtExtension.Name = "txtExtension";
      this.txtExtension.Size = new System.Drawing.Size(97, 20);
      this.txtExtension.TabIndex = 2;
      this.txtExtension.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblMime
      // 
      lblMime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      lblMime.AutoSize = true;
      lblMime.Location = new System.Drawing.Point(261, 32);
      lblMime.Name = "lblMime";
      lblMime.Size = new System.Drawing.Size(31, 13);
      lblMime.TabIndex = 3;
      lblMime.Text = "&Type";
      lblMime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblExtension
      // 
      lblExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      lblExtension.AutoSize = true;
      lblExtension.Location = new System.Drawing.Point(261, 8);
      lblExtension.Name = "lblExtension";
      lblExtension.Size = new System.Drawing.Size(53, 13);
      lblExtension.TabIndex = 1;
      lblExtension.Text = "&Extension";
      lblExtension.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // mimeTypes
      // 
      this.mimeTypes.AllowColumnReorder = true;
      this.mimeTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mimeTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            extensionColumn,
            mimeColumn,
            preferredColumn});
      this.mimeTypes.FullRowSelect = true;
      this.mimeTypes.HideSelection = false;
      this.mimeTypes.Location = new System.Drawing.Point(4, 4);
      this.mimeTypes.MultiSelect = false;
      this.mimeTypes.Name = "mimeTypes";
      this.mimeTypes.Size = new System.Drawing.Size(251, 261);
      this.mimeTypes.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.mimeTypes.TabIndex = 0;
      this.mimeTypes.UseCompatibleStateImageBehavior = false;
      this.mimeTypes.View = System.Windows.Forms.View.Details;
      this.mimeTypes.SelectedIndexChanged += new System.EventHandler(this.mimeTypes_SelectedIndexChanged);
      this.mimeTypes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
      // 
      // extensionColumn
      // 
      extensionColumn.Text = "Extension";
      // 
      // mimeColumn
      // 
      mimeColumn.Text = "MIME Type";
      mimeColumn.Width = 115;
      // 
      // preferredColumn
      // 
      preferredColumn.Text = "Preferred";
      preferredColumn.Width = 55;
      // 
      // btnRevert4
      // 
      this.btnRevert4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert4.Location = new System.Drawing.Point(342, 242);
      this.btnRevert4.Name = "btnRevert4";
      this.btnRevert4.Size = new System.Drawing.Size(75, 23);
      this.btnRevert4.TabIndex = 9;
      this.btnRevert4.Text = "Revert";
      this.btnRevert4.UseVisualStyleBackColor = true;
      this.btnRevert4.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply4
      // 
      this.btnApply4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply4.Location = new System.Drawing.Point(261, 242);
      this.btnApply4.Name = "btnApply4";
      this.btnApply4.Size = new System.Drawing.Size(75, 23);
      this.btnApply4.TabIndex = 8;
      this.btnApply4.Text = "&Apply";
      this.btnApply4.UseVisualStyleBackColor = true;
      this.btnApply4.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // progressTab
      // 
      progressTab.Controls.Add(progressSplitter);
      progressTab.Location = new System.Drawing.Point(4, 22);
      progressTab.Name = "progressTab";
      progressTab.Size = new System.Drawing.Size(509, 271);
      progressTab.TabIndex = 2;
      progressTab.Text = "Progress";
      progressTab.UseVisualStyleBackColor = true;
      // 
      // progressSplitter
      // 
      progressSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
      progressSplitter.Location = new System.Drawing.Point(0, 0);
      progressSplitter.Name = "progressSplitter";
      progressSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // progressSplitter.Panel1
      // 
      progressSplitter.Panel1.Controls.Add(lblCurrentDownloads);
      progressSplitter.Panel1.Controls.Add(this.downloads);
      progressSplitter.Panel1MinSize = 100;
      // 
      // progressSplitter.Panel2
      // 
      progressSplitter.Panel2.Controls.Add(this.bytesDownloaded);
      progressSplitter.Panel2.Controls.Add(lblBytesDownloaded);
      progressSplitter.Panel2.Controls.Add(this.downloadedResources);
      progressSplitter.Panel2.Controls.Add(lblDownloadedResources);
      progressSplitter.Panel2.Controls.Add(this.speed);
      progressSplitter.Panel2.Controls.Add(lblSpeed);
      progressSplitter.Panel2.Controls.Add(this.queued);
      progressSplitter.Panel2.Controls.Add(lblQueued);
      progressSplitter.Panel2.Controls.Add(this.connections);
      progressSplitter.Panel2.Controls.Add(lblConnections);
      progressSplitter.Panel2.Controls.Add(lblErrors);
      progressSplitter.Panel2.Controls.Add(this.recentErrors);
      progressSplitter.Panel2MinSize = 100;
      progressSplitter.Size = new System.Drawing.Size(509, 271);
      progressSplitter.SplitterDistance = 132;
      progressSplitter.TabIndex = 11;
      // 
      // lblCurrentDownloads
      // 
      lblCurrentDownloads.AutoSize = true;
      lblCurrentDownloads.Location = new System.Drawing.Point(0, 2);
      lblCurrentDownloads.Name = "lblCurrentDownloads";
      lblCurrentDownloads.Size = new System.Drawing.Size(97, 13);
      lblCurrentDownloads.TabIndex = 0;
      lblCurrentDownloads.Text = "Current Downloads";
      // 
      // downloads
      // 
      this.downloads.AllowColumnReorder = true;
      this.downloads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.downloads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            pathColumn,
            speedColumn,
            dlSizeColumn,
            sizeColumn,
            mimeTypeColumn,
            hostColumn});
      this.downloads.FullRowSelect = true;
      this.downloads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.downloads.Location = new System.Drawing.Point(0, 17);
      this.downloads.MultiSelect = false;
      this.downloads.Name = "downloads";
      this.downloads.Size = new System.Drawing.Size(509, 112);
      this.downloads.TabIndex = 1;
      this.downloads.UseCompatibleStateImageBehavior = false;
      this.downloads.View = System.Windows.Forms.View.Details;
      // 
      // pathColumn
      // 
      pathColumn.Text = "Path";
      pathColumn.Width = 195;
      // 
      // speedColumn
      // 
      speedColumn.Text = "Speed";
      speedColumn.Width = 62;
      // 
      // dlSizeColumn
      // 
      dlSizeColumn.Text = "Done";
      dlSizeColumn.Width = 62;
      // 
      // mimeTypeColumn
      // 
      mimeTypeColumn.Text = "Type";
      mimeTypeColumn.Width = 50;
      // 
      // hostColumn
      // 
      hostColumn.Text = "Host";
      hostColumn.Width = 35;
      // 
      // bytesDownloaded
      // 
      this.bytesDownloaded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.bytesDownloaded.Location = new System.Drawing.Point(273, 120);
      this.bytesDownloaded.Name = "bytesDownloaded";
      this.bytesDownloaded.Size = new System.Drawing.Size(78, 13);
      this.bytesDownloaded.TabIndex = 28;
      this.bytesDownloaded.Text = "0";
      this.bytesDownloaded.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblBytesDownloaded
      // 
      lblBytesDownloaded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblBytesDownloaded.AutoSize = true;
      lblBytesDownloaded.Location = new System.Drawing.Point(175, 120);
      lblBytesDownloaded.Name = "lblBytesDownloaded";
      lblBytesDownloaded.Size = new System.Drawing.Size(99, 13);
      lblBytesDownloaded.TabIndex = 27;
      lblBytesDownloaded.Text = "Bytes Downloaded:";
      lblBytesDownloaded.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // downloadedResources
      // 
      this.downloadedResources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.downloadedResources.Location = new System.Drawing.Point(120, 120);
      this.downloadedResources.Name = "downloadedResources";
      this.downloadedResources.Size = new System.Drawing.Size(55, 13);
      this.downloadedResources.TabIndex = 26;
      this.downloadedResources.Text = "0";
      this.downloadedResources.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblDownloadedResources
      // 
      lblDownloadedResources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblDownloadedResources.AutoSize = true;
      lblDownloadedResources.Location = new System.Drawing.Point(-1, 120);
      lblDownloadedResources.Name = "lblDownloadedResources";
      lblDownloadedResources.Size = new System.Drawing.Size(124, 13);
      lblDownloadedResources.TabIndex = 25;
      lblDownloadedResources.Text = "Resources Downloaded:";
      lblDownloadedResources.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // speed
      // 
      this.speed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.speed.Location = new System.Drawing.Point(317, 104);
      this.speed.Name = "speed";
      this.speed.Size = new System.Drawing.Size(98, 13);
      this.speed.TabIndex = 24;
      this.speed.Text = "0 kbps";
      this.speed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblSpeed
      // 
      lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblSpeed.AutoSize = true;
      lblSpeed.Location = new System.Drawing.Point(242, 104);
      lblSpeed.Name = "lblSpeed";
      lblSpeed.Size = new System.Drawing.Size(78, 13);
      lblSpeed.TabIndex = 23;
      lblSpeed.Text = "Current Speed:";
      lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // queued
      // 
      this.queued.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.queued.Location = new System.Drawing.Point(190, 104);
      this.queued.Name = "queued";
      this.queued.Size = new System.Drawing.Size(53, 13);
      this.queued.TabIndex = 22;
      this.queued.Text = "0";
      this.queued.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblQueued
      // 
      lblQueued.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblQueued.AutoSize = true;
      lblQueued.Location = new System.Drawing.Point(116, 104);
      lblQueued.Name = "lblQueued";
      lblQueued.Size = new System.Drawing.Size(76, 13);
      lblQueued.TabIndex = 21;
      lblQueued.Text = "Queued Links:";
      lblQueued.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // connections
      // 
      this.connections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.connections.Location = new System.Drawing.Point(66, 104);
      this.connections.Name = "connections";
      this.connections.Size = new System.Drawing.Size(44, 13);
      this.connections.TabIndex = 20;
      this.connections.Text = "0";
      this.connections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblConnections
      // 
      lblConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblConnections.AutoSize = true;
      lblConnections.Location = new System.Drawing.Point(-1, 104);
      lblConnections.Name = "lblConnections";
      lblConnections.Size = new System.Drawing.Size(69, 13);
      lblConnections.TabIndex = 19;
      lblConnections.Text = "Connections:";
      lblConnections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblErrors
      // 
      lblErrors.AutoSize = true;
      lblErrors.Location = new System.Drawing.Point(0, 0);
      lblErrors.Name = "lblErrors";
      lblErrors.Size = new System.Drawing.Size(72, 13);
      lblErrors.TabIndex = 2;
      lblErrors.Text = "Recent Errors";
      // 
      // recentErrors
      // 
      this.recentErrors.AllowColumnReorder = true;
      this.recentErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.recentErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            urlColumn2,
            messageColumn,
            referrerColumn,
            fatalColumn});
      this.recentErrors.ContextMenuStrip = errorsMenu;
      this.recentErrors.FullRowSelect = true;
      this.recentErrors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.recentErrors.Location = new System.Drawing.Point(0, 16);
      this.recentErrors.MultiSelect = false;
      this.recentErrors.Name = "recentErrors";
      this.recentErrors.Size = new System.Drawing.Size(509, 85);
      this.recentErrors.TabIndex = 3;
      this.recentErrors.UseCompatibleStateImageBehavior = false;
      this.recentErrors.View = System.Windows.Forms.View.Details;
      // 
      // urlColumn2
      // 
      urlColumn2.Text = "Url";
      urlColumn2.Width = 100;
      // 
      // messageColumn
      // 
      messageColumn.Text = "Message";
      messageColumn.Width = 165;
      // 
      // referrerColumn
      // 
      referrerColumn.Text = "Referrer";
      referrerColumn.Width = 100;
      // 
      // fatalColumn
      // 
      fatalColumn.Text = "Fatal";
      fatalColumn.Width = 38;
      // 
      // errorsMenu
      // 
      errorsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            clearErrorsMenuItem});
      errorsMenu.Name = "errorsMenu";
      errorsMenu.Size = new System.Drawing.Size(100, 26);
      // 
      // clearErrorsMenuItem
      // 
      clearErrorsMenuItem.Name = "clearErrorsMenuItem";
      clearErrorsMenuItem.Size = new System.Drawing.Size(99, 22);
      clearErrorsMenuItem.Text = "&Clear";
      clearErrorsMenuItem.Click += new System.EventHandler(this.clearErrorsMenuItem_Click);
      // 
      // sizeColumn
      // 
      sizeColumn.Text = "Size";
      sizeColumn.Width = 62;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(517, 343);
      this.Controls.Add(tabControl);
      this.Controls.Add(statusStrip);
      this.Controls.Add(menuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = menuStrip;
      this.MinimumSize = new System.Drawing.Size(439, 370);
      this.Name = "MainForm";
      this.Text = "WebCrawl 0.35 by Adam Milazzo";
      menuStrip.ResumeLayout(false);
      menuStrip.PerformLayout();
      statusStrip.ResumeLayout(false);
      statusStrip.PerformLayout();
      tabControl.ResumeLayout(false);
      setupTab.ResumeLayout(false);
      setupTab.PerformLayout();
      advancedTab.ResumeLayout(false);
      advancedTab.PerformLayout();
      filterTab.ResumeLayout(false);
      filterTab.PerformLayout();
      mimeTypesTab.ResumeLayout(false);
      mimeTypesTab.PerformLayout();
      progressTab.ResumeLayout(false);
      progressSplitter.Panel1.ResumeLayout(false);
      progressSplitter.Panel1.PerformLayout();
      progressSplitter.Panel2.ResumeLayout(false);
      progressSplitter.Panel2.PerformLayout();
      progressSplitter.ResumeLayout(false);
      errorsMenu.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
    private System.Windows.Forms.ToolStripMenuItem crawlerMenu;
    private System.Windows.Forms.ToolStripMenuItem startCrawlingMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pauseCrawlingMenuItem;
    private System.Windows.Forms.ToolStripStatusLabel status;
    private System.Windows.Forms.TextBox txtOutDir;
    private System.Windows.Forms.ComboBox cmbDomainNav;
    private System.Windows.Forms.ComboBox cmbDirNav;
    private System.Windows.Forms.Button btnRevert;
    private System.Windows.Forms.Button btnApply;
    private System.Windows.Forms.TextBox txtConnsPerServer;
    private System.Windows.Forms.TextBox txtMaxConnections;
    private System.Windows.Forms.CheckedListBox lstDownload;
    private System.Windows.Forms.CheckBox chkCookies;
    private System.Windows.Forms.CheckBox chkLinkRewrite;
    private System.Windows.Forms.TextBox txtFileSize;
    private System.Windows.Forms.TextBox txtRetries;
    private System.Windows.Forms.TextBox txtQueryStrings;
    private System.Windows.Forms.TextBox txtMaxDepth;
    private System.Windows.Forms.TextBox txtIdleTimeout;
    private System.Windows.Forms.ToolStripStatusLabel speedLabel;
    private System.Windows.Forms.Button btnRevert2;
    private System.Windows.Forms.Button btnApply2;
    private System.Windows.Forms.TextBox txtTransferTimeout;
    private System.Windows.Forms.TextBox txtReadTimeout;
    private System.Windows.Forms.Button btnRevert4;
    private System.Windows.Forms.Button btnApply4;
    private System.Windows.Forms.CheckBox chkPassiveFtp;
    private System.Windows.Forms.CheckBox chkNormalizeHosts;
    private System.Windows.Forms.CheckBox chkNormalizeQueries;
    private System.Windows.Forms.CheckBox chkGenerateErrorFiles;
    private System.Windows.Forms.TextBox txtUserAgent;
    private System.Windows.Forms.TextBox txtReferrer;
    private System.Windows.Forms.ComboBox cmbLanguage;
    private System.Windows.Forms.ListView mimeTypes;
    private System.Windows.Forms.Button btnDeleteMime;
    private System.Windows.Forms.Button btnAddMime;
    private System.Windows.Forms.TextBox txtMimeType;
    private System.Windows.Forms.TextBox txtExtension;
    private System.Windows.Forms.TextBox txtBaseUrls;
    private System.Windows.Forms.TextBox txtAdditionalUrls;
    private System.Windows.Forms.Button btnRevert3;
    private System.Windows.Forms.Button btnApply3;
    private System.Windows.Forms.ListView filters;
    private System.Windows.Forms.TextBox txtRegex;
    private System.Windows.Forms.TextBox txtReplacement;
    private System.Windows.Forms.ComboBox cmbFilterType;
    private System.Windows.Forms.Button btnDeleteFilter;
    private System.Windows.Forms.Button btnAddFilter;
    private System.Windows.Forms.CheckBox chkClear;
    private System.Windows.Forms.ToolStripMenuItem abortCrawlingMenuItem;
    private ListViewNF downloads;
    private ListViewNF recentErrors;
    private System.Windows.Forms.Label speed;
    private System.Windows.Forms.Label queued;
    private System.Windows.Forms.Label connections;
    private System.Windows.Forms.CheckBox chkEnqueueBaseUrls;
    private System.Windows.Forms.CheckBox chkPreferred;
    private System.Windows.Forms.TextBox txtMaxQueue;
    private System.Windows.Forms.Label bytesDownloaded;
    private System.Windows.Forms.Label downloadedResources;

  }
}