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
      System.Windows.Forms.TabPage progressTab;
      System.Windows.Forms.SplitContainer progressSplitter;
      System.Windows.Forms.Label lblCurrentDownloads;
      System.Windows.Forms.ColumnHeader pathColumn;
      System.Windows.Forms.ColumnHeader speedColumn;
      System.Windows.Forms.ColumnHeader sizeColumn;
      System.Windows.Forms.ColumnHeader hostColumn;
      System.Windows.Forms.Label lblSpeed;
      System.Windows.Forms.Label lblQueued;
      System.Windows.Forms.Label lblConnections;
      System.Windows.Forms.Label lblErrors;
      System.Windows.Forms.ColumnHeader urlColumn2;
      System.Windows.Forms.ColumnHeader messageColumn;
      System.Windows.Forms.ColumnHeader referrerColumn;
      System.Windows.Forms.TabPage consoleTab;
      System.Windows.Forms.TextBox txtInput;
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
      this.connsPerServer = new System.Windows.Forms.TextBox();
      this.maxConnections = new System.Windows.Forms.TextBox();
      this.download = new System.Windows.Forms.CheckedListBox();
      this.domainNav = new System.Windows.Forms.ComboBox();
      this.dirNav = new System.Windows.Forms.ComboBox();
      this.btnRevert = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.txtOutDir = new System.Windows.Forms.TextBox();
      this.additionalUrls = new System.Windows.Forms.TextBox();
      this.chkClear = new System.Windows.Forms.CheckBox();
      this.language = new System.Windows.Forms.ComboBox();
      this.userAgent = new System.Windows.Forms.TextBox();
      this.referrer = new System.Windows.Forms.TextBox();
      this.chkPassiveFtp = new System.Windows.Forms.CheckBox();
      this.chkNormalizeHosts = new System.Windows.Forms.CheckBox();
      this.chkNormalizeQueries = new System.Windows.Forms.CheckBox();
      this.btnRevert2 = new System.Windows.Forms.Button();
      this.btnApply2 = new System.Windows.Forms.Button();
      this.transferTimeout = new System.Windows.Forms.TextBox();
      this.readTimeout = new System.Windows.Forms.TextBox();
      this.idleTimeout = new System.Windows.Forms.TextBox();
      this.fileSize = new System.Windows.Forms.TextBox();
      this.retries = new System.Windows.Forms.TextBox();
      this.queryStrings = new System.Windows.Forms.TextBox();
      this.maxDepth = new System.Windows.Forms.TextBox();
      this.btnDeleteFilter = new System.Windows.Forms.Button();
      this.btnAddFilter = new System.Windows.Forms.Button();
      this.filterType = new System.Windows.Forms.ComboBox();
      this.txtReplacement = new System.Windows.Forms.TextBox();
      this.txtRegex = new System.Windows.Forms.TextBox();
      this.filters = new System.Windows.Forms.ListView();
      this.btnRevert3 = new System.Windows.Forms.Button();
      this.btnApply3 = new System.Windows.Forms.Button();
      this.btnDeleteMime = new System.Windows.Forms.Button();
      this.btnAddMime = new System.Windows.Forms.Button();
      this.txtMimeType = new System.Windows.Forms.TextBox();
      this.txtExtension = new System.Windows.Forms.TextBox();
      this.mimeTypes = new System.Windows.Forms.ListView();
      this.btnRevert4 = new System.Windows.Forms.Button();
      this.btnApply4 = new System.Windows.Forms.Button();
      this.downloads = new System.Windows.Forms.ListView();
      this.speed = new System.Windows.Forms.Label();
      this.queued = new System.Windows.Forms.Label();
      this.connections = new System.Windows.Forms.Label();
      this.recentErrors = new System.Windows.Forms.ListView();
      this.txtConsole = new System.Windows.Forms.TextBox();
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
      progressTab = new System.Windows.Forms.TabPage();
      progressSplitter = new System.Windows.Forms.SplitContainer();
      lblCurrentDownloads = new System.Windows.Forms.Label();
      pathColumn = new System.Windows.Forms.ColumnHeader();
      speedColumn = new System.Windows.Forms.ColumnHeader();
      sizeColumn = new System.Windows.Forms.ColumnHeader();
      hostColumn = new System.Windows.Forms.ColumnHeader();
      lblSpeed = new System.Windows.Forms.Label();
      lblQueued = new System.Windows.Forms.Label();
      lblConnections = new System.Windows.Forms.Label();
      lblErrors = new System.Windows.Forms.Label();
      urlColumn2 = new System.Windows.Forms.ColumnHeader();
      messageColumn = new System.Windows.Forms.ColumnHeader();
      referrerColumn = new System.Windows.Forms.ColumnHeader();
      consoleTab = new System.Windows.Forms.TabPage();
      txtInput = new System.Windows.Forms.TextBox();
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
      consoleTab.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            this.crawlerMenu});
      menuStrip.Location = new System.Drawing.Point(0, 0);
      menuStrip.Name = "menuStrip";
      menuStrip.Size = new System.Drawing.Size(431, 24);
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
      statusStrip.Location = new System.Drawing.Point(0, 297);
      statusStrip.Name = "statusStrip";
      statusStrip.Size = new System.Drawing.Size(431, 22);
      statusStrip.TabIndex = 1;
      // 
      // status
      // 
      this.status.Name = "status";
      this.status.Size = new System.Drawing.Size(365, 17);
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
      tabControl.Controls.Add(consoleTab);
      tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      tabControl.Location = new System.Drawing.Point(0, 24);
      tabControl.Name = "tabControl";
      tabControl.SelectedIndex = 0;
      tabControl.Size = new System.Drawing.Size(431, 273);
      tabControl.TabIndex = 2;
      // 
      // setupTab
      // 
      setupTab.Controls.Add(this.txtBaseUrls);
      setupTab.Controls.Add(lblBaseUrls);
      setupTab.Controls.Add(this.chkGenerateErrorFiles);
      setupTab.Controls.Add(this.chkCookies);
      setupTab.Controls.Add(this.chkLinkRewrite);
      setupTab.Controls.Add(this.connsPerServer);
      setupTab.Controls.Add(lblConnsPerServer);
      setupTab.Controls.Add(this.maxConnections);
      setupTab.Controls.Add(lblConns);
      setupTab.Controls.Add(this.download);
      setupTab.Controls.Add(lblDownload);
      setupTab.Controls.Add(this.domainNav);
      setupTab.Controls.Add(lblDomainNav);
      setupTab.Controls.Add(this.dirNav);
      setupTab.Controls.Add(lblDirNav);
      setupTab.Controls.Add(this.btnRevert);
      setupTab.Controls.Add(this.btnApply);
      setupTab.Controls.Add(browseOutDir);
      setupTab.Controls.Add(this.txtOutDir);
      setupTab.Controls.Add(lblOutDir);
      setupTab.Location = new System.Drawing.Point(4, 22);
      setupTab.Name = "setupTab";
      setupTab.Padding = new System.Windows.Forms.Padding(3);
      setupTab.Size = new System.Drawing.Size(423, 247);
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
      this.txtBaseUrls.Size = new System.Drawing.Size(305, 58);
      this.txtBaseUrls.TabIndex = 19;
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
      lblBaseUrls.TabIndex = 18;
      lblBaseUrls.Text = "Base Urls\n(one per line)";
      lblBaseUrls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // chkGenerateErrorFiles
      // 
      this.chkGenerateErrorFiles.AutoSize = true;
      this.chkGenerateErrorFiles.Location = new System.Drawing.Point(261, 194);
      this.chkGenerateErrorFiles.Name = "chkGenerateErrorFiles";
      this.chkGenerateErrorFiles.Size = new System.Drawing.Size(119, 17);
      this.chkGenerateErrorFiles.TabIndex = 15;
      this.chkGenerateErrorFiles.Text = "Generate Error Files";
      this.chkGenerateErrorFiles.UseVisualStyleBackColor = true;
      this.chkGenerateErrorFiles.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkCookies
      // 
      this.chkCookies.AutoSize = true;
      this.chkCookies.Location = new System.Drawing.Point(261, 171);
      this.chkCookies.Name = "chkCookies";
      this.chkCookies.Size = new System.Drawing.Size(64, 17);
      this.chkCookies.TabIndex = 14;
      this.chkCookies.Text = "Cookies";
      this.chkCookies.UseVisualStyleBackColor = true;
      this.chkCookies.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkLinkRewrite
      // 
      this.chkLinkRewrite.AutoSize = true;
      this.chkLinkRewrite.Location = new System.Drawing.Point(261, 148);
      this.chkLinkRewrite.Name = "chkLinkRewrite";
      this.chkLinkRewrite.Size = new System.Drawing.Size(93, 17);
      this.chkLinkRewrite.TabIndex = 13;
      this.chkLinkRewrite.Text = "Link Rewriting";
      this.chkLinkRewrite.UseVisualStyleBackColor = true;
      this.chkLinkRewrite.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // connsPerServer
      // 
      this.connsPerServer.Location = new System.Drawing.Point(293, 121);
      this.connsPerServer.Name = "connsPerServer";
      this.connsPerServer.Size = new System.Drawing.Size(39, 20);
      this.connsPerServer.TabIndex = 10;
      this.connsPerServer.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.connsPerServer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblConnsPerServer
      // 
      lblConnsPerServer.AutoSize = true;
      lblConnsPerServer.Location = new System.Drawing.Point(172, 125);
      lblConnsPerServer.Name = "lblConnsPerServer";
      lblConnsPerServer.Size = new System.Drawing.Size(119, 13);
      lblConnsPerServer.TabIndex = 9;
      lblConnsPerServer.Text = "Connections Per Server";
      lblConnsPerServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // maxConnections
      // 
      this.maxConnections.Location = new System.Drawing.Point(109, 121);
      this.maxConnections.Name = "maxConnections";
      this.maxConnections.Size = new System.Drawing.Size(39, 20);
      this.maxConnections.TabIndex = 8;
      this.maxConnections.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.maxConnections.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblConns
      // 
      lblConns.AutoSize = true;
      lblConns.Location = new System.Drawing.Point(3, 125);
      lblConns.Name = "lblConns";
      lblConns.Size = new System.Drawing.Size(89, 13);
      lblConns.TabIndex = 7;
      lblConns.Text = "Max Connections";
      lblConns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // download
      // 
      this.download.CheckOnClick = true;
      this.download.FormattingEnabled = true;
      this.download.Location = new System.Drawing.Point(109, 147);
      this.download.Name = "download";
      this.download.Size = new System.Drawing.Size(146, 79);
      this.download.TabIndex = 12;
      this.download.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.download_ItemCheck);
      // 
      // lblDownload
      // 
      lblDownload.AutoSize = true;
      lblDownload.Location = new System.Drawing.Point(3, 150);
      lblDownload.Name = "lblDownload";
      lblDownload.Size = new System.Drawing.Size(55, 13);
      lblDownload.TabIndex = 11;
      lblDownload.Text = "Download";
      lblDownload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // domainNav
      // 
      this.domainNav.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.domainNav.FormattingEnabled = true;
      this.domainNav.Location = new System.Drawing.Point(293, 95);
      this.domainNav.Name = "domainNav";
      this.domainNav.Size = new System.Drawing.Size(121, 21);
      this.domainNav.TabIndex = 6;
      this.domainNav.SelectedIndexChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // lblDomainNav
      // 
      lblDomainNav.AutoSize = true;
      lblDomainNav.Location = new System.Drawing.Point(194, 99);
      lblDomainNav.Name = "lblDomainNav";
      lblDomainNav.Size = new System.Drawing.Size(97, 13);
      lblDomainNav.TabIndex = 5;
      lblDomainNav.Text = "Domain Navigation";
      lblDomainNav.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // dirNav
      // 
      this.dirNav.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dirNav.FormattingEnabled = true;
      this.dirNav.Location = new System.Drawing.Point(109, 95);
      this.dirNav.Name = "dirNav";
      this.dirNav.Size = new System.Drawing.Size(83, 21);
      this.dirNav.TabIndex = 4;
      this.dirNav.SelectedIndexChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // lblDirNav
      // 
      lblDirNav.AutoSize = true;
      lblDirNav.Location = new System.Drawing.Point(3, 99);
      lblDirNav.Name = "lblDirNav";
      lblDirNav.Size = new System.Drawing.Size(103, 13);
      lblDirNav.TabIndex = 3;
      lblDirNav.Text = "Directory Navigation";
      lblDirNav.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnRevert
      // 
      this.btnRevert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert.Location = new System.Drawing.Point(342, 218);
      this.btnRevert.Name = "btnRevert";
      this.btnRevert.Size = new System.Drawing.Size(75, 23);
      this.btnRevert.TabIndex = 17;
      this.btnRevert.Text = "Revert";
      this.btnRevert.UseVisualStyleBackColor = true;
      this.btnRevert.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply
      // 
      this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply.Location = new System.Drawing.Point(261, 218);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(75, 23);
      this.btnApply.TabIndex = 16;
      this.btnApply.Text = "&Apply";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // browseOutDir
      // 
      browseOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      browseOutDir.Location = new System.Drawing.Point(339, 70);
      browseOutDir.Name = "browseOutDir";
      browseOutDir.Size = new System.Drawing.Size(75, 20);
      browseOutDir.TabIndex = 2;
      browseOutDir.Text = "Browse...";
      browseOutDir.UseVisualStyleBackColor = true;
      browseOutDir.Click += new System.EventHandler(this.browseOutDir_Click);
      // 
      // txtOutDir
      // 
      this.txtOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOutDir.Location = new System.Drawing.Point(109, 70);
      this.txtOutDir.Name = "txtOutDir";
      this.txtOutDir.Size = new System.Drawing.Size(224, 20);
      this.txtOutDir.TabIndex = 1;
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
      lblOutDir.TabIndex = 0;
      lblOutDir.Text = "Output Directory";
      lblOutDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // advancedTab
      // 
      advancedTab.Controls.Add(this.additionalUrls);
      advancedTab.Controls.Add(lblAdditionalUrls);
      advancedTab.Controls.Add(this.chkClear);
      advancedTab.Controls.Add(this.language);
      advancedTab.Controls.Add(lblLanguage);
      advancedTab.Controls.Add(this.userAgent);
      advancedTab.Controls.Add(lblUserAgent);
      advancedTab.Controls.Add(this.referrer);
      advancedTab.Controls.Add(lblReferrer);
      advancedTab.Controls.Add(this.chkPassiveFtp);
      advancedTab.Controls.Add(this.chkNormalizeHosts);
      advancedTab.Controls.Add(this.chkNormalizeQueries);
      advancedTab.Controls.Add(this.btnRevert2);
      advancedTab.Controls.Add(this.btnApply2);
      advancedTab.Controls.Add(this.transferTimeout);
      advancedTab.Controls.Add(lblTransfer);
      advancedTab.Controls.Add(this.readTimeout);
      advancedTab.Controls.Add(lblRead);
      advancedTab.Controls.Add(this.idleTimeout);
      advancedTab.Controls.Add(lblIdle);
      advancedTab.Controls.Add(this.fileSize);
      advancedTab.Controls.Add(this.retries);
      advancedTab.Controls.Add(lblRetries);
      advancedTab.Controls.Add(this.queryStrings);
      advancedTab.Controls.Add(lblQueryStrings);
      advancedTab.Controls.Add(lblFileSize);
      advancedTab.Controls.Add(this.maxDepth);
      advancedTab.Controls.Add(lblDepth);
      advancedTab.Location = new System.Drawing.Point(4, 22);
      advancedTab.Name = "advancedTab";
      advancedTab.Padding = new System.Windows.Forms.Padding(3);
      advancedTab.Size = new System.Drawing.Size(423, 247);
      advancedTab.TabIndex = 3;
      advancedTab.Text = "Advanced Setup";
      advancedTab.UseVisualStyleBackColor = true;
      // 
      // additionalUrls
      // 
      this.additionalUrls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.additionalUrls.Location = new System.Drawing.Point(86, 6);
      this.additionalUrls.Multiline = true;
      this.additionalUrls.Name = "additionalUrls";
      this.additionalUrls.Size = new System.Drawing.Size(328, 43);
      this.additionalUrls.TabIndex = 27;
      this.additionalUrls.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.additionalUrls.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblAdditionalUrls
      // 
      lblAdditionalUrls.AutoSize = true;
      lblAdditionalUrls.Location = new System.Drawing.Point(6, 9);
      lblAdditionalUrls.Name = "lblAdditionalUrls";
      lblAdditionalUrls.Size = new System.Drawing.Size(74, 26);
      lblAdditionalUrls.TabIndex = 26;
      lblAdditionalUrls.Text = "Additional Urls\n(one per line)";
      lblAdditionalUrls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // chkClear
      // 
      this.chkClear.AutoSize = true;
      this.chkClear.Location = new System.Drawing.Point(6, 227);
      this.chkClear.Name = "chkClear";
      this.chkClear.Size = new System.Drawing.Size(146, 17);
      this.chkClear.TabIndex = 25;
      this.chkClear.Text = "Clear Download Directory";
      this.chkClear.UseVisualStyleBackColor = true;
      this.chkClear.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // language
      // 
      this.language.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.language.FormattingEnabled = true;
      this.language.Location = new System.Drawing.Point(261, 157);
      this.language.Name = "language";
      this.language.Size = new System.Drawing.Size(153, 21);
      this.language.TabIndex = 22;
      this.language.SelectedIndexChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // lblLanguage
      // 
      lblLanguage.AutoSize = true;
      lblLanguage.Location = new System.Drawing.Point(157, 161);
      lblLanguage.Name = "lblLanguage";
      lblLanguage.Size = new System.Drawing.Size(101, 13);
      lblLanguage.TabIndex = 21;
      lblLanguage.Text = "Preferred Language";
      lblLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // userAgent
      // 
      this.userAgent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.userAgent.Location = new System.Drawing.Point(89, 132);
      this.userAgent.Name = "userAgent";
      this.userAgent.Size = new System.Drawing.Size(325, 20);
      this.userAgent.TabIndex = 17;
      this.userAgent.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.userAgent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblUserAgent
      // 
      lblUserAgent.AutoSize = true;
      lblUserAgent.Location = new System.Drawing.Point(3, 136);
      lblUserAgent.Name = "lblUserAgent";
      lblUserAgent.Size = new System.Drawing.Size(60, 13);
      lblUserAgent.TabIndex = 16;
      lblUserAgent.Text = "User Agent";
      lblUserAgent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // referrer
      // 
      this.referrer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.referrer.Location = new System.Drawing.Point(89, 107);
      this.referrer.Name = "referrer";
      this.referrer.Size = new System.Drawing.Size(325, 20);
      this.referrer.TabIndex = 15;
      this.referrer.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.referrer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblReferrer
      // 
      lblReferrer.AutoSize = true;
      lblReferrer.Location = new System.Drawing.Point(3, 111);
      lblReferrer.Name = "lblReferrer";
      lblReferrer.Size = new System.Drawing.Size(82, 13);
      lblReferrer.TabIndex = 14;
      lblReferrer.Text = "Default Referrer";
      lblReferrer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // chkPassiveFtp
      // 
      this.chkPassiveFtp.AutoSize = true;
      this.chkPassiveFtp.Location = new System.Drawing.Point(6, 204);
      this.chkPassiveFtp.Name = "chkPassiveFtp";
      this.chkPassiveFtp.Size = new System.Drawing.Size(86, 17);
      this.chkPassiveFtp.TabIndex = 20;
      this.chkPassiveFtp.Text = "Passive FTP";
      this.chkPassiveFtp.UseVisualStyleBackColor = true;
      this.chkPassiveFtp.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkNormalizeHosts
      // 
      this.chkNormalizeHosts.AutoSize = true;
      this.chkNormalizeHosts.Location = new System.Drawing.Point(6, 181);
      this.chkNormalizeHosts.Name = "chkNormalizeHosts";
      this.chkNormalizeHosts.Size = new System.Drawing.Size(133, 17);
      this.chkNormalizeHosts.TabIndex = 19;
      this.chkNormalizeHosts.Text = "Normalize Host Names";
      this.chkNormalizeHosts.UseVisualStyleBackColor = true;
      this.chkNormalizeHosts.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // chkNormalizeQueries
      // 
      this.chkNormalizeQueries.AutoSize = true;
      this.chkNormalizeQueries.Location = new System.Drawing.Point(6, 158);
      this.chkNormalizeQueries.Name = "chkNormalizeQueries";
      this.chkNormalizeQueries.Size = new System.Drawing.Size(138, 17);
      this.chkNormalizeQueries.TabIndex = 18;
      this.chkNormalizeQueries.Text = "Normalize Query Strings";
      this.chkNormalizeQueries.UseVisualStyleBackColor = true;
      this.chkNormalizeQueries.CheckedChanged += new System.EventHandler(this.OnFormChanged);
      // 
      // btnRevert2
      // 
      this.btnRevert2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert2.Location = new System.Drawing.Point(342, 218);
      this.btnRevert2.Name = "btnRevert2";
      this.btnRevert2.Size = new System.Drawing.Size(75, 23);
      this.btnRevert2.TabIndex = 24;
      this.btnRevert2.Text = "Revert";
      this.btnRevert2.UseVisualStyleBackColor = true;
      this.btnRevert2.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply2
      // 
      this.btnApply2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply2.Location = new System.Drawing.Point(261, 218);
      this.btnApply2.Name = "btnApply2";
      this.btnApply2.Size = new System.Drawing.Size(75, 23);
      this.btnApply2.TabIndex = 23;
      this.btnApply2.Text = "&Apply";
      this.btnApply2.UseVisualStyleBackColor = true;
      this.btnApply2.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // transferTimeout
      // 
      this.transferTimeout.Location = new System.Drawing.Point(338, 81);
      this.transferTimeout.Name = "transferTimeout";
      this.transferTimeout.Size = new System.Drawing.Size(35, 20);
      this.transferTimeout.TabIndex = 13;
      this.transferTimeout.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.transferTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblTransfer
      // 
      lblTransfer.AutoSize = true;
      lblTransfer.Location = new System.Drawing.Point(247, 85);
      lblTransfer.Name = "lblTransfer";
      lblTransfer.Size = new System.Drawing.Size(87, 13);
      lblTransfer.TabIndex = 12;
      lblTransfer.Text = "Transfer Timeout";
      lblTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // readTimeout
      // 
      this.readTimeout.Location = new System.Drawing.Point(204, 81);
      this.readTimeout.Name = "readTimeout";
      this.readTimeout.Size = new System.Drawing.Size(35, 20);
      this.readTimeout.TabIndex = 11;
      this.readTimeout.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.readTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblRead
      // 
      lblRead.AutoSize = true;
      lblRead.Location = new System.Drawing.Point(114, 85);
      lblRead.Name = "lblRead";
      lblRead.Size = new System.Drawing.Size(74, 13);
      lblRead.TabIndex = 10;
      lblRead.Text = "Read Timeout";
      lblRead.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // idleTimeout
      // 
      this.idleTimeout.Location = new System.Drawing.Point(73, 81);
      this.idleTimeout.Name = "idleTimeout";
      this.idleTimeout.Size = new System.Drawing.Size(35, 20);
      this.idleTimeout.TabIndex = 9;
      this.idleTimeout.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.idleTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblIdle
      // 
      lblIdle.AutoSize = true;
      lblIdle.Location = new System.Drawing.Point(3, 85);
      lblIdle.Name = "lblIdle";
      lblIdle.Size = new System.Drawing.Size(65, 13);
      lblIdle.TabIndex = 8;
      lblIdle.Text = "Idle Timeout";
      lblIdle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // fileSize
      // 
      this.fileSize.Location = new System.Drawing.Point(73, 55);
      this.fileSize.Name = "fileSize";
      this.fileSize.Size = new System.Drawing.Size(35, 20);
      this.fileSize.TabIndex = 1;
      this.fileSize.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.fileSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // retries
      // 
      this.retries.Location = new System.Drawing.Point(382, 55);
      this.retries.Name = "retries";
      this.retries.Size = new System.Drawing.Size(32, 20);
      this.retries.TabIndex = 7;
      this.retries.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.retries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblRetries
      // 
      lblRetries.AutoSize = true;
      lblRetries.Location = new System.Drawing.Point(343, 59);
      lblRetries.Name = "lblRetries";
      lblRetries.Size = new System.Drawing.Size(40, 13);
      lblRetries.TabIndex = 6;
      lblRetries.Text = "Retries";
      lblRetries.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // queryStrings
      // 
      this.queryStrings.Location = new System.Drawing.Point(204, 55);
      this.queryStrings.Name = "queryStrings";
      this.queryStrings.Size = new System.Drawing.Size(35, 20);
      this.queryStrings.TabIndex = 3;
      this.queryStrings.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.queryStrings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblQueryStrings
      // 
      lblQueryStrings.AutoSize = true;
      lblQueryStrings.Location = new System.Drawing.Point(114, 59);
      lblQueryStrings.Name = "lblQueryStrings";
      lblQueryStrings.Size = new System.Drawing.Size(89, 13);
      lblQueryStrings.TabIndex = 2;
      lblQueryStrings.Text = "Query String Limit";
      lblQueryStrings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lblFileSize
      // 
      lblFileSize.AutoSize = true;
      lblFileSize.Location = new System.Drawing.Point(3, 59);
      lblFileSize.Name = "lblFileSize";
      lblFileSize.Size = new System.Drawing.Size(69, 13);
      lblFileSize.TabIndex = 0;
      lblFileSize.Text = "Max File Size";
      lblFileSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // maxDepth
      // 
      this.maxDepth.Location = new System.Drawing.Point(307, 55);
      this.maxDepth.Name = "maxDepth";
      this.maxDepth.Size = new System.Drawing.Size(32, 20);
      this.maxDepth.TabIndex = 5;
      this.maxDepth.TextChanged += new System.EventHandler(this.OnFormChanged);
      this.maxDepth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblDepth
      // 
      lblDepth.AutoSize = true;
      lblDepth.Location = new System.Drawing.Point(247, 59);
      lblDepth.Name = "lblDepth";
      lblDepth.Size = new System.Drawing.Size(59, 13);
      lblDepth.TabIndex = 4;
      lblDepth.Text = "Max Depth";
      lblDepth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // filterTab
      // 
      filterTab.Controls.Add(this.btnDeleteFilter);
      filterTab.Controls.Add(this.btnAddFilter);
      filterTab.Controls.Add(this.filterType);
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
      filterTab.Size = new System.Drawing.Size(423, 247);
      filterTab.TabIndex = 4;
      filterTab.Text = "Url Filters";
      filterTab.UseVisualStyleBackColor = true;
      // 
      // btnDeleteFilter
      // 
      this.btnDeleteFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnDeleteFilter.Enabled = false;
      this.btnDeleteFilter.Location = new System.Drawing.Point(69, 204);
      this.btnDeleteFilter.Name = "btnDeleteFilter";
      this.btnDeleteFilter.Size = new System.Drawing.Size(59, 23);
      this.btnDeleteFilter.TabIndex = 19;
      this.btnDeleteFilter.Text = "&Delete";
      this.btnDeleteFilter.UseVisualStyleBackColor = true;
      this.btnDeleteFilter.Click += new System.EventHandler(this.btnDeleteFilter_Click);
      // 
      // btnAddFilter
      // 
      this.btnAddFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAddFilter.Location = new System.Drawing.Point(3, 204);
      this.btnAddFilter.Name = "btnAddFilter";
      this.btnAddFilter.Size = new System.Drawing.Size(59, 23);
      this.btnAddFilter.TabIndex = 18;
      this.btnAddFilter.Text = "Add &New";
      this.btnAddFilter.UseVisualStyleBackColor = true;
      this.btnAddFilter.Click += new System.EventHandler(this.btnAddFilter_Click);
      // 
      // filterType
      // 
      this.filterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.filterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.filterType.FormattingEnabled = true;
      this.filterType.Items.AddRange(new object[] {
            "Must Match",
            "Must Not Match",
            "Change"});
      this.filterType.Location = new System.Drawing.Point(77, 177);
      this.filterType.Name = "filterType";
      this.filterType.Size = new System.Drawing.Size(104, 21);
      this.filterType.TabIndex = 17;
      this.filterType.SelectedIndexChanged += new System.EventHandler(this.filterType_SelectedIndexChanged);
      // 
      // lblType
      // 
      lblType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblType.AutoSize = true;
      lblType.Location = new System.Drawing.Point(3, 181);
      lblType.Name = "lblType";
      lblType.Size = new System.Drawing.Size(31, 13);
      lblType.TabIndex = 16;
      lblType.Text = "&Type";
      lblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtReplacement
      // 
      this.txtReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReplacement.Enabled = false;
      this.txtReplacement.Location = new System.Drawing.Point(77, 153);
      this.txtReplacement.Name = "txtReplacement";
      this.txtReplacement.Size = new System.Drawing.Size(342, 20);
      this.txtReplacement.TabIndex = 15;
      this.txtReplacement.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblReplacement
      // 
      lblReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblReplacement.AutoSize = true;
      lblReplacement.Location = new System.Drawing.Point(1, 157);
      lblReplacement.Name = "lblReplacement";
      lblReplacement.Size = new System.Drawing.Size(70, 13);
      lblReplacement.TabIndex = 14;
      lblReplacement.Text = "Re&placement";
      lblReplacement.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtRegex
      // 
      this.txtRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtRegex.Location = new System.Drawing.Point(77, 129);
      this.txtRegex.Name = "txtRegex";
      this.txtRegex.Size = new System.Drawing.Size(342, 20);
      this.txtRegex.TabIndex = 13;
      this.txtRegex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // lblRegex
      // 
      lblRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblRegex.AutoSize = true;
      lblRegex.Location = new System.Drawing.Point(1, 133);
      lblRegex.Name = "lblRegex";
      lblRegex.Size = new System.Drawing.Size(38, 13);
      lblRegex.TabIndex = 12;
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
      this.filters.Size = new System.Drawing.Size(415, 120);
      this.filters.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.filters.TabIndex = 11;
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
      this.btnRevert3.Location = new System.Drawing.Point(342, 218);
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
      this.btnApply3.Location = new System.Drawing.Point(261, 218);
      this.btnApply3.Name = "btnApply3";
      this.btnApply3.Size = new System.Drawing.Size(75, 23);
      this.btnApply3.TabIndex = 9;
      this.btnApply3.Text = "&Apply";
      this.btnApply3.UseVisualStyleBackColor = true;
      this.btnApply3.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // mimeTypesTab
      // 
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
      mimeTypesTab.Size = new System.Drawing.Size(423, 247);
      mimeTypesTab.TabIndex = 1;
      mimeTypesTab.Text = "MIME Types";
      mimeTypesTab.UseVisualStyleBackColor = true;
      // 
      // btnDeleteMime
      // 
      this.btnDeleteMime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnDeleteMime.Enabled = false;
      this.btnDeleteMime.Location = new System.Drawing.Point(327, 54);
      this.btnDeleteMime.Name = "btnDeleteMime";
      this.btnDeleteMime.Size = new System.Drawing.Size(59, 23);
      this.btnDeleteMime.TabIndex = 6;
      this.btnDeleteMime.Text = "&Delete";
      this.btnDeleteMime.UseVisualStyleBackColor = true;
      this.btnDeleteMime.Click += new System.EventHandler(this.btnDeleteMime_Click);
      // 
      // btnAddMime
      // 
      this.btnAddMime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAddMime.Enabled = false;
      this.btnAddMime.Location = new System.Drawing.Point(261, 54);
      this.btnAddMime.Name = "btnAddMime";
      this.btnAddMime.Size = new System.Drawing.Size(59, 23);
      this.btnAddMime.TabIndex = 5;
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
            mimeColumn});
      this.mimeTypes.FullRowSelect = true;
      this.mimeTypes.HideSelection = false;
      this.mimeTypes.Location = new System.Drawing.Point(4, 4);
      this.mimeTypes.MultiSelect = false;
      this.mimeTypes.Name = "mimeTypes";
      this.mimeTypes.Size = new System.Drawing.Size(251, 237);
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
      mimeColumn.Width = 170;
      // 
      // btnRevert4
      // 
      this.btnRevert4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRevert4.Location = new System.Drawing.Point(342, 218);
      this.btnRevert4.Name = "btnRevert4";
      this.btnRevert4.Size = new System.Drawing.Size(75, 23);
      this.btnRevert4.TabIndex = 8;
      this.btnRevert4.Text = "Revert";
      this.btnRevert4.UseVisualStyleBackColor = true;
      this.btnRevert4.Click += new System.EventHandler(this.OnRevertClicked);
      // 
      // btnApply4
      // 
      this.btnApply4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply4.Location = new System.Drawing.Point(261, 218);
      this.btnApply4.Name = "btnApply4";
      this.btnApply4.Size = new System.Drawing.Size(75, 23);
      this.btnApply4.TabIndex = 7;
      this.btnApply4.Text = "&Apply";
      this.btnApply4.UseVisualStyleBackColor = true;
      this.btnApply4.Click += new System.EventHandler(this.OnApplyClicked);
      // 
      // progressTab
      // 
      progressTab.Controls.Add(progressSplitter);
      progressTab.Location = new System.Drawing.Point(4, 22);
      progressTab.Name = "progressTab";
      progressTab.Size = new System.Drawing.Size(423, 247);
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
      progressSplitter.Panel2.Controls.Add(this.speed);
      progressSplitter.Panel2.Controls.Add(lblSpeed);
      progressSplitter.Panel2.Controls.Add(this.queued);
      progressSplitter.Panel2.Controls.Add(lblQueued);
      progressSplitter.Panel2.Controls.Add(this.connections);
      progressSplitter.Panel2.Controls.Add(lblConnections);
      progressSplitter.Panel2.Controls.Add(lblErrors);
      progressSplitter.Panel2.Controls.Add(this.recentErrors);
      progressSplitter.Panel2MinSize = 100;
      progressSplitter.Size = new System.Drawing.Size(423, 247);
      progressSplitter.SplitterDistance = 121;
      progressSplitter.TabIndex = 11;
      // 
      // lblCurrentDownloads
      // 
      lblCurrentDownloads.AutoSize = true;
      lblCurrentDownloads.Location = new System.Drawing.Point(0, 2);
      lblCurrentDownloads.Name = "lblCurrentDownloads";
      lblCurrentDownloads.Size = new System.Drawing.Size(97, 13);
      lblCurrentDownloads.TabIndex = 2;
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
            sizeColumn,
            hostColumn});
      this.downloads.FullRowSelect = true;
      this.downloads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.downloads.Location = new System.Drawing.Point(0, 17);
      this.downloads.MultiSelect = false;
      this.downloads.Name = "downloads";
      this.downloads.Size = new System.Drawing.Size(423, 101);
      this.downloads.TabIndex = 3;
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
      // sizeColumn
      // 
      sizeColumn.Text = "Bytes";
      sizeColumn.Width = 62;
      // 
      // hostColumn
      // 
      hostColumn.Text = "Host";
      hostColumn.Width = 75;
      // 
      // speed
      // 
      this.speed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.speed.Location = new System.Drawing.Point(305, 105);
      this.speed.Name = "speed";
      this.speed.Size = new System.Drawing.Size(117, 13);
      this.speed.TabIndex = 24;
      this.speed.Text = "0 kbps";
      // 
      // lblSpeed
      // 
      lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblSpeed.AutoSize = true;
      lblSpeed.Location = new System.Drawing.Point(230, 105);
      lblSpeed.Name = "lblSpeed";
      lblSpeed.Size = new System.Drawing.Size(78, 13);
      lblSpeed.TabIndex = 23;
      lblSpeed.Text = "Current Speed:";
      // 
      // queued
      // 
      this.queued.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.queued.Location = new System.Drawing.Point(183, 105);
      this.queued.Name = "queued";
      this.queued.Size = new System.Drawing.Size(47, 13);
      this.queued.TabIndex = 22;
      this.queued.Text = "0";
      // 
      // lblQueued
      // 
      lblQueued.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblQueued.AutoSize = true;
      lblQueued.Location = new System.Drawing.Point(109, 105);
      lblQueued.Name = "lblQueued";
      lblQueued.Size = new System.Drawing.Size(76, 13);
      lblQueued.TabIndex = 21;
      lblQueued.Text = "Queued Links:";
      // 
      // connections
      // 
      this.connections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.connections.Location = new System.Drawing.Point(66, 105);
      this.connections.Name = "connections";
      this.connections.Size = new System.Drawing.Size(44, 13);
      this.connections.TabIndex = 20;
      this.connections.Text = "0";
      // 
      // lblConnections
      // 
      lblConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      lblConnections.AutoSize = true;
      lblConnections.Location = new System.Drawing.Point(-1, 105);
      lblConnections.Name = "lblConnections";
      lblConnections.Size = new System.Drawing.Size(69, 13);
      lblConnections.TabIndex = 19;
      lblConnections.Text = "Connections:";
      // 
      // lblErrors
      // 
      lblErrors.AutoSize = true;
      lblErrors.Location = new System.Drawing.Point(0, 0);
      lblErrors.Name = "lblErrors";
      lblErrors.Size = new System.Drawing.Size(72, 13);
      lblErrors.TabIndex = 11;
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
            referrerColumn});
      this.recentErrors.FullRowSelect = true;
      this.recentErrors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.recentErrors.Location = new System.Drawing.Point(0, 16);
      this.recentErrors.MultiSelect = false;
      this.recentErrors.Name = "recentErrors";
      this.recentErrors.Size = new System.Drawing.Size(423, 84);
      this.recentErrors.TabIndex = 12;
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
      messageColumn.Width = 195;
      // 
      // referrerColumn
      // 
      referrerColumn.Text = "Referrer";
      referrerColumn.Width = 100;
      // 
      // consoleTab
      // 
      consoleTab.Controls.Add(txtInput);
      consoleTab.Controls.Add(this.txtConsole);
      consoleTab.Location = new System.Drawing.Point(4, 22);
      consoleTab.Name = "consoleTab";
      consoleTab.Padding = new System.Windows.Forms.Padding(3);
      consoleTab.Size = new System.Drawing.Size(423, 247);
      consoleTab.TabIndex = 5;
      consoleTab.Text = "Console";
      consoleTab.UseVisualStyleBackColor = true;
      // 
      // txtInput
      // 
      txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      txtInput.Location = new System.Drawing.Point(4, 223);
      txtInput.Name = "txtInput";
      txtInput.Size = new System.Drawing.Size(413, 20);
      txtInput.TabIndex = 2;
      txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // txtConsole
      // 
      this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtConsole.BackColor = System.Drawing.SystemColors.Window;
      this.txtConsole.Location = new System.Drawing.Point(4, 4);
      this.txtConsole.Multiline = true;
      this.txtConsole.Name = "txtConsole";
      this.txtConsole.ReadOnly = true;
      this.txtConsole.Size = new System.Drawing.Size(413, 214);
      this.txtConsole.TabIndex = 0;
      this.txtConsole.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(431, 319);
      this.Controls.Add(tabControl);
      this.Controls.Add(statusStrip);
      this.Controls.Add(menuStrip);
      this.MainMenuStrip = menuStrip;
      this.MinimumSize = new System.Drawing.Size(439, 346);
      this.Name = "MainForm";
      this.Text = "WebCrawl 0.10 by Adam Milazzo";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
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
      consoleTab.ResumeLayout(false);
      consoleTab.PerformLayout();
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
    private System.Windows.Forms.ComboBox domainNav;
    private System.Windows.Forms.ComboBox dirNav;
    private System.Windows.Forms.Button btnRevert;
    private System.Windows.Forms.Button btnApply;
    private System.Windows.Forms.TextBox connsPerServer;
    private System.Windows.Forms.TextBox maxConnections;
    private System.Windows.Forms.CheckedListBox download;
    private System.Windows.Forms.CheckBox chkCookies;
    private System.Windows.Forms.CheckBox chkLinkRewrite;
    private System.Windows.Forms.TextBox fileSize;
    private System.Windows.Forms.TextBox retries;
    private System.Windows.Forms.TextBox queryStrings;
    private System.Windows.Forms.TextBox maxDepth;
    private System.Windows.Forms.TextBox idleTimeout;
    private System.Windows.Forms.ToolStripStatusLabel speedLabel;
    private System.Windows.Forms.Button btnRevert2;
    private System.Windows.Forms.Button btnApply2;
    private System.Windows.Forms.TextBox transferTimeout;
    private System.Windows.Forms.TextBox readTimeout;
    private System.Windows.Forms.Button btnRevert4;
    private System.Windows.Forms.Button btnApply4;
    private System.Windows.Forms.CheckBox chkPassiveFtp;
    private System.Windows.Forms.CheckBox chkNormalizeHosts;
    private System.Windows.Forms.CheckBox chkNormalizeQueries;
    private System.Windows.Forms.CheckBox chkGenerateErrorFiles;
    private System.Windows.Forms.TextBox userAgent;
    private System.Windows.Forms.TextBox referrer;
    private System.Windows.Forms.ComboBox language;
    private System.Windows.Forms.ListView mimeTypes;
    private System.Windows.Forms.Button btnDeleteMime;
    private System.Windows.Forms.Button btnAddMime;
    private System.Windows.Forms.TextBox txtMimeType;
    private System.Windows.Forms.TextBox txtExtension;
    private System.Windows.Forms.TextBox txtBaseUrls;
    private System.Windows.Forms.TextBox additionalUrls;
    private System.Windows.Forms.Button btnRevert3;
    private System.Windows.Forms.Button btnApply3;
    private System.Windows.Forms.ListView filters;
    private System.Windows.Forms.TextBox txtRegex;
    private System.Windows.Forms.TextBox txtReplacement;
    private System.Windows.Forms.ComboBox filterType;
    private System.Windows.Forms.Button btnDeleteFilter;
    private System.Windows.Forms.Button btnAddFilter;
    private System.Windows.Forms.TextBox txtConsole;
    private System.Windows.Forms.CheckBox chkClear;
    private System.Windows.Forms.ToolStripMenuItem abortCrawlingMenuItem;
    private System.Windows.Forms.ListView downloads;
    private System.Windows.Forms.ListView recentErrors;
    private System.Windows.Forms.Label speed;
    private System.Windows.Forms.Label queued;
    private System.Windows.Forms.Label connections;

  }
}