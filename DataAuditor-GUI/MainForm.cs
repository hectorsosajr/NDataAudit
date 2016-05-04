//*********************************************************************
// File:				MainForm.cs
// Author:				Hector Sosa, Jr
// Date:				2/8/2005
// Class Summary:
//*********************************************************************
//Change Log
//*****************************************************************************
// USER					DATE		COMMENTS
// Hector Sosa, Jr		2/8/2005	Created
// Hector Sosa, Jr		3/7/2005	Added cursor changing to XpLinkRun_Click()
// Hector Sosa, Jr		3/21/2005	Added event handlers for running a
//									single audit, instead of all of them.
// Hector Sosa, Jr      8/22/2012   Renamed to DataAuditor.
//*****************************************************************************

using System;
using System.Windows.Forms;
using NAudit.Framework;

namespace DataAuditor.UI
{
    public class MainForm : Form
    {

    #region  Windows Form Designer generated code 

        public MainForm() : base()
        {

            //This call is required by the Windows Form Designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call

        }

        //Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        internal TD.SandBar.SandBarManager SandBarManager1;
        internal TD.SandBar.ToolBarContainer leftSandBarDock;
        internal TD.SandBar.ToolBarContainer rightSandBarDock;
        internal TD.SandBar.ToolBarContainer bottomSandBarDock;
        internal TD.SandBar.ToolBarContainer topSandBarDock;
        internal TD.SandBar.MenuBar MenuBar1;
        internal TD.SandBar.MenuBarItem MenuBarItem1;
        internal TD.SandBar.MenuBarItem MenuBarItem2;
        internal TD.SandBar.MenuBarItem MenuBarItem3;
        internal TD.SandBar.MenuBarItem MenuBarItem4;
        internal TD.SandBar.MenuBarItem MenuBarItem5;
        internal System.Windows.Forms.Splitter Splitter1;
        internal TD.SandBar.MenuButtonItem mnuExit;
        internal TD.SandBar.MenuButtonItem mnuLoad;
        internal TD.SandBar.MenuButtonItem mnuSave;
        internal TD.SandBar.MenuButtonItem mnuNewGroup;
        internal NETXP.Controls.TaskPane.XPLink XpLinkNew;
        internal NETXP.Controls.TaskPane.XPLink XpLinkLoad;
        internal System.Windows.Forms.OpenFileDialog OpenAudits;
        internal System.Windows.Forms.ImageList imgAudits;
        internal NETXP.Controls.TaskPane.XPTaskPaneGroup xpgAudits;
        internal NETXP.Controls.TaskPane.XPTaskPaneGroup xpgGroups;
        internal NETXP.Controls.TaskPane.XPLink XpLinkRun;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.PropertyGrid ppgAudits;
        internal System.Windows.Forms.Label lblAuditGroup;
        internal System.Windows.Forms.ListView lsvAudits;
        internal System.Windows.Forms.ColumnHeader clhIcon;
        internal System.Windows.Forms.ColumnHeader clhDesc;
        internal System.Windows.Forms.ColumnHeader clhStatus;
        internal NETXP.Controls.TaskPane.XPTaskPane XpTaskPane;
        internal NETXP.Controls.TaskPane.XPLink xpLinkAdd;
        internal NETXP.Controls.TaskPane.XPLink xpLinkDelete;
        internal NETXP.Controls.TaskPane.XPLink xpLinkUp;
        internal NETXP.Controls.TaskPane.XPLink xpLinkDown;
        internal NETXP.Controls.TaskPane.XPLink xpLinkAuditRun;
        internal TD.SandBar.ToolBar tbGroup;
        private TD.SandBar.ButtonItem btnNewGroup;
        private TD.SandBar.ButtonItem btnLoadGroup;
        private TD.SandBar.ButtonItem btnRunGroup;
        private TD.SandBar.ToolBar tbAudit;
        private TD.SandBar.ButtonItem btnAddAudit;
        private TD.SandBar.ButtonItem btnDeleteAudit;
        private TD.SandBar.ButtonItem btnRunAudit;
        internal System.Windows.Forms.Splitter Splitter2;
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            NETXP.Library.DynamicColorTable dynamicColorTable1 = new NETXP.Library.DynamicColorTable();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SandBarManager1 = new TD.SandBar.SandBarManager();
            this.leftSandBarDock = new TD.SandBar.ToolBarContainer();
            this.rightSandBarDock = new TD.SandBar.ToolBarContainer();
            this.bottomSandBarDock = new TD.SandBar.ToolBarContainer();
            this.topSandBarDock = new TD.SandBar.ToolBarContainer();
            this.MenuBar1 = new TD.SandBar.MenuBar();
            this.MenuBarItem1 = new TD.SandBar.MenuBarItem();
            this.mnuNewGroup = new TD.SandBar.MenuButtonItem();
            this.mnuLoad = new TD.SandBar.MenuButtonItem();
            this.mnuSave = new TD.SandBar.MenuButtonItem();
            this.mnuExit = new TD.SandBar.MenuButtonItem();
            this.MenuBarItem2 = new TD.SandBar.MenuBarItem();
            this.MenuBarItem3 = new TD.SandBar.MenuBarItem();
            this.MenuBarItem4 = new TD.SandBar.MenuBarItem();
            this.MenuBarItem5 = new TD.SandBar.MenuBarItem();
            this.tbGroup = new TD.SandBar.ToolBar();
            this.btnNewGroup = new TD.SandBar.ButtonItem();
            this.btnLoadGroup = new TD.SandBar.ButtonItem();
            this.btnRunGroup = new TD.SandBar.ButtonItem();
            this.tbAudit = new TD.SandBar.ToolBar();
            this.btnAddAudit = new TD.SandBar.ButtonItem();
            this.btnDeleteAudit = new TD.SandBar.ButtonItem();
            this.btnRunAudit = new TD.SandBar.ButtonItem();
            this.XpTaskPane = new NETXP.Controls.TaskPane.XPTaskPane();
            this.xpgAudits = new NETXP.Controls.TaskPane.XPTaskPaneGroup();
            this.xpLinkAuditRun = new NETXP.Controls.TaskPane.XPLink();
            this.xpLinkDown = new NETXP.Controls.TaskPane.XPLink();
            this.xpLinkUp = new NETXP.Controls.TaskPane.XPLink();
            this.xpLinkDelete = new NETXP.Controls.TaskPane.XPLink();
            this.xpLinkAdd = new NETXP.Controls.TaskPane.XPLink();
            this.xpgGroups = new NETXP.Controls.TaskPane.XPTaskPaneGroup();
            this.XpLinkRun = new NETXP.Controls.TaskPane.XPLink();
            this.XpLinkLoad = new NETXP.Controls.TaskPane.XPLink();
            this.XpLinkNew = new NETXP.Controls.TaskPane.XPLink();
            this.Splitter1 = new System.Windows.Forms.Splitter();
            this.OpenAudits = new System.Windows.Forms.OpenFileDialog();
            this.imgAudits = new System.Windows.Forms.ImageList(this.components);
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Splitter2 = new System.Windows.Forms.Splitter();
            this.ppgAudits = new System.Windows.Forms.PropertyGrid();
            this.lblAuditGroup = new System.Windows.Forms.Label();
            this.lsvAudits = new System.Windows.Forms.ListView();
            this.clhIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.topSandBarDock.SuspendLayout();
            this.XpTaskPane.SuspendLayout();
            this.xpgAudits.SuspendLayout();
            this.xpgGroups.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SandBarManager1
            // 
            this.SandBarManager1.OwnerForm = this;
            // 
            // leftSandBarDock
            // 
            this.leftSandBarDock.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftSandBarDock.Guid = new System.Guid("0005de62-e1a4-4aed-86e8-8982f974f423");
            this.leftSandBarDock.Location = new System.Drawing.Point(0, 64);
            this.leftSandBarDock.Manager = this.SandBarManager1;
            this.leftSandBarDock.Name = "leftSandBarDock";
            this.leftSandBarDock.Size = new System.Drawing.Size(0, 422);
            this.leftSandBarDock.TabIndex = 1;
            // 
            // rightSandBarDock
            // 
            this.rightSandBarDock.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightSandBarDock.Guid = new System.Guid("d16b2d2d-d34e-4bc5-8a74-d6961a94a91b");
            this.rightSandBarDock.Location = new System.Drawing.Point(624, 64);
            this.rightSandBarDock.Manager = this.SandBarManager1;
            this.rightSandBarDock.Name = "rightSandBarDock";
            this.rightSandBarDock.Size = new System.Drawing.Size(0, 422);
            this.rightSandBarDock.TabIndex = 2;
            // 
            // bottomSandBarDock
            // 
            this.bottomSandBarDock.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomSandBarDock.Guid = new System.Guid("453d2a06-d705-4729-8665-4c03d7a4fc7d");
            this.bottomSandBarDock.Location = new System.Drawing.Point(0, 486);
            this.bottomSandBarDock.Manager = this.SandBarManager1;
            this.bottomSandBarDock.Name = "bottomSandBarDock";
            this.bottomSandBarDock.Size = new System.Drawing.Size(624, 0);
            this.bottomSandBarDock.TabIndex = 3;
            // 
            // topSandBarDock
            // 
            this.topSandBarDock.Controls.Add(this.MenuBar1);
            this.topSandBarDock.Controls.Add(this.tbGroup);
            this.topSandBarDock.Controls.Add(this.tbAudit);
            this.topSandBarDock.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSandBarDock.Guid = new System.Guid("f84b0b89-33c0-426f-9560-62b247d238f2");
            this.topSandBarDock.Location = new System.Drawing.Point(0, 0);
            this.topSandBarDock.Manager = this.SandBarManager1;
            this.topSandBarDock.Name = "topSandBarDock";
            this.topSandBarDock.Size = new System.Drawing.Size(624, 64);
            this.topSandBarDock.TabIndex = 4;
            // 
            // MenuBar1
            // 
            this.MenuBar1.Guid = new System.Guid("0196a532-5051-4a9b-b066-d94b76c60c8c");
            this.MenuBar1.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
            this.MenuBarItem1,
            this.MenuBarItem2,
            this.MenuBarItem3,
            this.MenuBarItem4,
            this.MenuBarItem5});
            this.MenuBar1.Location = new System.Drawing.Point(2, 0);
            this.MenuBar1.Name = "MenuBar1";
            this.MenuBar1.OwnerForm = this;
            this.MenuBar1.Size = new System.Drawing.Size(622, 24);
            this.MenuBar1.TabIndex = 0;
            this.MenuBar1.Text = "MenuBar1";
            // 
            // MenuBarItem1
            // 
            this.MenuBarItem1.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
            this.mnuNewGroup,
            this.mnuLoad,
            this.mnuSave,
            this.mnuExit});
            this.MenuBarItem1.Text = "&File";
            // 
            // mnuNewGroup
            // 
            this.mnuNewGroup.Text = "New Audit Group";
            // 
            // mnuLoad
            // 
            this.mnuLoad.Text = "Load Audit Group";
            // 
            // mnuSave
            // 
            this.mnuSave.Text = "Save Audit Group";
            // 
            // mnuExit
            // 
            this.mnuExit.BeginGroup = true;
            this.mnuExit.Text = "Exit";
            this.mnuExit.Activate += new System.EventHandler(this.mnuExit_Activate);
            // 
            // MenuBarItem2
            // 
            this.MenuBarItem2.Text = "&Edit";
            // 
            // MenuBarItem3
            // 
            this.MenuBarItem3.Text = "&View";
            // 
            // MenuBarItem4
            // 
            this.MenuBarItem4.MdiWindowList = true;
            this.MenuBarItem4.Text = "&Window";
            // 
            // MenuBarItem5
            // 
            this.MenuBarItem5.Text = "&Help";
            // 
            // tbGroup
            // 
            this.tbGroup.DockLine = 1;
            this.tbGroup.Guid = new System.Guid("1068a9f8-d8ea-4221-a4a9-f004e2dcc607");
            this.tbGroup.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
            this.btnNewGroup,
            this.btnLoadGroup,
            this.btnRunGroup});
            this.tbGroup.Location = new System.Drawing.Point(2, 24);
            this.tbGroup.Name = "tbGroup";
            this.tbGroup.Size = new System.Drawing.Size(237, 40);
            this.tbGroup.TabIndex = 1;
            this.tbGroup.Text = "AuditGroup";
            this.tbGroup.TextAlign = TD.SandBar.ToolBarTextAlign.Underneath;
            // 
            // btnNewGroup
            // 
            this.btnNewGroup.BeginGroup = true;
            this.btnNewGroup.Icon = ((System.Drawing.Icon)(resources.GetObject("btnNewGroup.Icon")));
            this.btnNewGroup.Text = "New Group";
            // 
            // btnLoadGroup
            // 
            this.btnLoadGroup.Icon = ((System.Drawing.Icon)(resources.GetObject("btnLoadGroup.Icon")));
            this.btnLoadGroup.Text = "Load Group";
            this.btnLoadGroup.Activate += new System.EventHandler(this.btnLoadGroup_Activate);
            // 
            // btnRunGroup
            // 
            this.btnRunGroup.Icon = ((System.Drawing.Icon)(resources.GetObject("btnRunGroup.Icon")));
            this.btnRunGroup.Text = "Run Audits";
            // 
            // tbAudit
            // 
            this.tbAudit.DockLine = 1;
            this.tbAudit.DockOffset = 12;
            this.tbAudit.Guid = new System.Guid("34672742-ab26-40ec-88cb-eaca2e35a249");
            this.tbAudit.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
            this.btnAddAudit,
            this.btnDeleteAudit,
            this.btnRunAudit});
            this.tbAudit.Location = new System.Drawing.Point(241, 24);
            this.tbAudit.Name = "tbAudit";
            this.tbAudit.Size = new System.Drawing.Size(243, 40);
            this.tbAudit.TabIndex = 2;
            this.tbAudit.Text = "Audits";
            this.tbAudit.TextAlign = TD.SandBar.ToolBarTextAlign.Underneath;
            // 
            // btnAddAudit
            // 
            this.btnAddAudit.Icon = ((System.Drawing.Icon)(resources.GetObject("btnAddAudit.Icon")));
            this.btnAddAudit.Text = "Add Audit";
            // 
            // btnDeleteAudit
            // 
            this.btnDeleteAudit.Icon = ((System.Drawing.Icon)(resources.GetObject("btnDeleteAudit.Icon")));
            this.btnDeleteAudit.Text = "Delete Audit";
            // 
            // btnRunAudit
            // 
            this.btnRunAudit.Icon = ((System.Drawing.Icon)(resources.GetObject("btnRunAudit.Icon")));
            this.btnRunAudit.Text = "Run Audit";
            this.btnRunAudit.Activate += new System.EventHandler(this.btnRunAudit_Activate);
            // 
            // XpTaskPane
            // 
            this.XpTaskPane.AutoScroll = true;
            this.XpTaskPane.ColorTable = dynamicColorTable1;
            this.XpTaskPane.Controls.Add(this.xpgAudits);
            this.XpTaskPane.Controls.Add(this.xpgGroups);
            this.XpTaskPane.Dock = System.Windows.Forms.DockStyle.Left;
            this.XpTaskPane.Location = new System.Drawing.Point(0, 64);
            this.XpTaskPane.Name = "XpTaskPane";
            this.XpTaskPane.Size = new System.Drawing.Size(168, 422);
            this.XpTaskPane.TabIndex = 5;
            this.XpTaskPane.Text = "XpTaskPane1";
            // 
            // xpgAudits
            // 
            this.xpgAudits.ColorTable = dynamicColorTable1;
            this.xpgAudits.Controls.Add(this.xpLinkAuditRun);
            this.xpgAudits.Controls.Add(this.xpLinkDown);
            this.xpgAudits.Controls.Add(this.xpLinkUp);
            this.xpgAudits.Controls.Add(this.xpLinkDelete);
            this.xpgAudits.Controls.Add(this.xpLinkAdd);
            this.xpgAudits.Location = new System.Drawing.Point(8, 152);
            this.xpgAudits.Name = "xpgAudits";
            this.xpgAudits.Size = new System.Drawing.Size(144, 152);
            this.xpgAudits.TabIndex = 1;
            this.xpgAudits.Text = "Audit Actions";
            // 
            // xpLinkAuditRun
            // 
            this.xpLinkAuditRun.BackColor = System.Drawing.Color.Transparent;
            this.xpLinkAuditRun.ColorTable = dynamicColorTable1;
            this.xpLinkAuditRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.xpLinkAuditRun.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.xpLinkAuditRun.Image = ((System.Drawing.Image)(resources.GetObject("xpLinkAuditRun.Image")));
            this.xpLinkAuditRun.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xpLinkAuditRun.Location = new System.Drawing.Point(8, 80);
            this.xpLinkAuditRun.Name = "xpLinkAuditRun";
            this.xpLinkAuditRun.Size = new System.Drawing.Size(128, 16);
            this.xpLinkAuditRun.TabIndex = 5;
            this.xpLinkAuditRun.Text = "Run Audit";
            this.xpLinkAuditRun.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xpLinkAuditRun.Click += new System.EventHandler(this.xpLinkAuditRun_Click);
            // 
            // xpLinkDown
            // 
            this.xpLinkDown.BackColor = System.Drawing.Color.Transparent;
            this.xpLinkDown.ColorTable = dynamicColorTable1;
            this.xpLinkDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.xpLinkDown.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.xpLinkDown.Image = ((System.Drawing.Image)(resources.GetObject("xpLinkDown.Image")));
            this.xpLinkDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xpLinkDown.Location = new System.Drawing.Point(8, 128);
            this.xpLinkDown.Name = "xpLinkDown";
            this.xpLinkDown.Size = new System.Drawing.Size(128, 16);
            this.xpLinkDown.TabIndex = 4;
            this.xpLinkDown.Text = "Move Down";
            this.xpLinkDown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // xpLinkUp
            // 
            this.xpLinkUp.BackColor = System.Drawing.Color.Transparent;
            this.xpLinkUp.ColorTable = dynamicColorTable1;
            this.xpLinkUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.xpLinkUp.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.xpLinkUp.Image = ((System.Drawing.Image)(resources.GetObject("xpLinkUp.Image")));
            this.xpLinkUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xpLinkUp.Location = new System.Drawing.Point(8, 104);
            this.xpLinkUp.Name = "xpLinkUp";
            this.xpLinkUp.Size = new System.Drawing.Size(128, 16);
            this.xpLinkUp.TabIndex = 3;
            this.xpLinkUp.Text = "Move Up";
            this.xpLinkUp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // xpLinkDelete
            // 
            this.xpLinkDelete.BackColor = System.Drawing.Color.Transparent;
            this.xpLinkDelete.ColorTable = dynamicColorTable1;
            this.xpLinkDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.xpLinkDelete.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.xpLinkDelete.Image = ((System.Drawing.Image)(resources.GetObject("xpLinkDelete.Image")));
            this.xpLinkDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xpLinkDelete.Location = new System.Drawing.Point(8, 56);
            this.xpLinkDelete.Name = "xpLinkDelete";
            this.xpLinkDelete.Size = new System.Drawing.Size(128, 16);
            this.xpLinkDelete.TabIndex = 2;
            this.xpLinkDelete.Text = "Delete Audit";
            this.xpLinkDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // xpLinkAdd
            // 
            this.xpLinkAdd.BackColor = System.Drawing.Color.Transparent;
            this.xpLinkAdd.ColorTable = dynamicColorTable1;
            this.xpLinkAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.xpLinkAdd.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.xpLinkAdd.Image = ((System.Drawing.Image)(resources.GetObject("xpLinkAdd.Image")));
            this.xpLinkAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xpLinkAdd.Location = new System.Drawing.Point(8, 32);
            this.xpLinkAdd.Name = "xpLinkAdd";
            this.xpLinkAdd.Size = new System.Drawing.Size(128, 16);
            this.xpLinkAdd.TabIndex = 1;
            this.xpLinkAdd.Text = "Add Audit";
            this.xpLinkAdd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // xpgGroups
            // 
            this.xpgGroups.ColorTable = dynamicColorTable1;
            this.xpgGroups.Controls.Add(this.XpLinkRun);
            this.xpgGroups.Controls.Add(this.XpLinkLoad);
            this.xpgGroups.Controls.Add(this.XpLinkNew);
            this.xpgGroups.Location = new System.Drawing.Point(8, 24);
            this.xpgGroups.Name = "xpgGroups";
            this.xpgGroups.Size = new System.Drawing.Size(144, 112);
            this.xpgGroups.TabIndex = 0;
            this.xpgGroups.Text = "Group Actions";
            // 
            // XpLinkRun
            // 
            this.XpLinkRun.BackColor = System.Drawing.Color.Transparent;
            this.XpLinkRun.ColorTable = dynamicColorTable1;
            this.XpLinkRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.XpLinkRun.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.XpLinkRun.Image = ((System.Drawing.Image)(resources.GetObject("XpLinkRun.Image")));
            this.XpLinkRun.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.XpLinkRun.Location = new System.Drawing.Point(8, 80);
            this.XpLinkRun.Name = "XpLinkRun";
            this.XpLinkRun.Size = new System.Drawing.Size(128, 16);
            this.XpLinkRun.TabIndex = 2;
            this.XpLinkRun.Text = "Run Audits";
            this.XpLinkRun.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.XpLinkRun.Click += new System.EventHandler(this.XpLinkRun_Click);
            // 
            // XpLinkLoad
            // 
            this.XpLinkLoad.BackColor = System.Drawing.Color.Transparent;
            this.XpLinkLoad.ColorTable = dynamicColorTable1;
            this.XpLinkLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.XpLinkLoad.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.XpLinkLoad.Image = ((System.Drawing.Image)(resources.GetObject("XpLinkLoad.Image")));
            this.XpLinkLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.XpLinkLoad.Location = new System.Drawing.Point(8, 56);
            this.XpLinkLoad.Name = "XpLinkLoad";
            this.XpLinkLoad.Size = new System.Drawing.Size(128, 16);
            this.XpLinkLoad.TabIndex = 1;
            this.XpLinkLoad.Text = "Load Audit Group";
            this.XpLinkLoad.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.XpLinkLoad.Click += new System.EventHandler(this.XpLinkLoad_Click);
            // 
            // XpLinkNew
            // 
            this.XpLinkNew.BackColor = System.Drawing.Color.Transparent;
            this.XpLinkNew.ColorTable = dynamicColorTable1;
            this.XpLinkNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.XpLinkNew.HotForeColor = System.Drawing.SystemColors.HotTrack;
            this.XpLinkNew.Image = ((System.Drawing.Image)(resources.GetObject("XpLinkNew.Image")));
            this.XpLinkNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.XpLinkNew.Location = new System.Drawing.Point(8, 32);
            this.XpLinkNew.Name = "XpLinkNew";
            this.XpLinkNew.Size = new System.Drawing.Size(128, 16);
            this.XpLinkNew.TabIndex = 0;
            this.XpLinkNew.Text = "New Audit Group";
            this.XpLinkNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Splitter1
            // 
            this.Splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Splitter1.Location = new System.Drawing.Point(168, 64);
            this.Splitter1.Name = "Splitter1";
            this.Splitter1.Size = new System.Drawing.Size(8, 422);
            this.Splitter1.TabIndex = 6;
            this.Splitter1.TabStop = false;
            // 
            // OpenAudits
            // 
            this.OpenAudits.AddExtension = false;
            this.OpenAudits.DefaultExt = "xml";
            this.OpenAudits.Filter = "XML Files|*.xml|Audit Files|*.audit|All Files|*.*";
            this.OpenAudits.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenAudits_FileOk);
            // 
            // imgAudits
            // 
            this.imgAudits.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgAudits.ImageStream")));
            this.imgAudits.TransparentColor = System.Drawing.Color.Transparent;
            this.imgAudits.Images.SetKeyName(0, "");
            this.imgAudits.Images.SetKeyName(1, "");
            this.imgAudits.Images.SetKeyName(2, "");
            this.imgAudits.Images.SetKeyName(3, "");
            this.imgAudits.Images.SetKeyName(4, "");
            this.imgAudits.Images.SetKeyName(5, "");
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.Splitter2);
            this.Panel1.Controls.Add(this.ppgAudits);
            this.Panel1.Controls.Add(this.lblAuditGroup);
            this.Panel1.Controls.Add(this.lsvAudits);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel1.Location = new System.Drawing.Point(176, 64);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(448, 422);
            this.Panel1.TabIndex = 7;
            // 
            // Splitter2
            // 
            this.Splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Splitter2.Location = new System.Drawing.Point(0, 174);
            this.Splitter2.Name = "Splitter2";
            this.Splitter2.Size = new System.Drawing.Size(448, 8);
            this.Splitter2.TabIndex = 16;
            this.Splitter2.TabStop = false;
            // 
            // ppgAudits
            // 
            this.ppgAudits.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ppgAudits.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.ppgAudits.Location = new System.Drawing.Point(0, 182);
            this.ppgAudits.Name = "ppgAudits";
            this.ppgAudits.Size = new System.Drawing.Size(448, 240);
            this.ppgAudits.TabIndex = 15;
            // 
            // lblAuditGroup
            // 
            this.lblAuditGroup.BackColor = System.Drawing.Color.Blue;
            this.lblAuditGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAuditGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuditGroup.ForeColor = System.Drawing.Color.White;
            this.lblAuditGroup.Location = new System.Drawing.Point(0, 0);
            this.lblAuditGroup.Name = "lblAuditGroup";
            this.lblAuditGroup.Size = new System.Drawing.Size(448, 23);
            this.lblAuditGroup.TabIndex = 14;
            this.lblAuditGroup.Text = "Label1";
            this.lblAuditGroup.Click += new System.EventHandler(this.lblAuditGroup_Click);
            // 
            // lsvAudits
            // 
            this.lsvAudits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvAudits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhIcon,
            this.clhDesc,
            this.clhStatus});
            this.lsvAudits.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lsvAudits.FullRowSelect = true;
            this.lsvAudits.HideSelection = false;
            this.lsvAudits.Location = new System.Drawing.Point(0, 24);
            this.lsvAudits.Name = "lsvAudits";
            this.lsvAudits.Size = new System.Drawing.Size(448, 146);
            this.lsvAudits.SmallImageList = this.imgAudits;
            this.lsvAudits.TabIndex = 13;
            this.lsvAudits.UseCompatibleStateImageBehavior = false;
            this.lsvAudits.View = System.Windows.Forms.View.Details;
            this.lsvAudits.SelectedIndexChanged += new System.EventHandler(this.lsvAudits_SelectedIndexChanged);
            // 
            // clhIcon
            // 
            this.clhIcon.Text = "Audit Name";
            this.clhIcon.Width = 145;
            // 
            // clhDesc
            // 
            this.clhDesc.Text = "Description";
            this.clhDesc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.clhDesc.Width = 222;
            // 
            // clhStatus
            // 
            this.clhStatus.Text = "Status";
            this.clhStatus.Width = 76;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(624, 486);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Splitter1);
            this.Controls.Add(this.XpTaskPane);
            this.Controls.Add(this.leftSandBarDock);
            this.Controls.Add(this.rightSandBarDock);
            this.Controls.Add(this.bottomSandBarDock);
            this.Controls.Add(this.topSandBarDock);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DataAuditor - No Audits Loaded";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.topSandBarDock.ResumeLayout(false);
            this.XpTaskPane.ResumeLayout(false);
            this.xpgAudits.ResumeLayout(false);
            this.xpgGroups.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

    #endregion

    #region Declarations

        private string _auditName;
        private AuditController _audits;
        private AuditTesting _auditTesting;
        private int _selectedIndex = 0;

    #endregion

    #region  Control Events

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            xpgAudits.GroupState = NETXP.Controls.TaskPane.GroupState.Collapsed;
            XpLinkRun.Enabled = false;
            lblAuditGroup.Text = "No Audits Loaded";
        } 

        private void mnuExit_Activate(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void XpLinkLoad_Click(object sender, System.EventArgs e)
        {
            LoadFileDialog();
        }

        private void XpLinkRun_Click(object sender, System.EventArgs e)
        {
            _auditTesting = new AuditTesting(_audits.AuditGroup);
            
            // Wire up the events
            _auditTesting.AuditTestingStarting += _auditTesting_AuditTestingStarting;
            _auditTesting.CurrentAuditRunning +=_auditTesting_CurrentAuditRunning;
            _auditTesting.CurrentAuditDone +=_auditTesting_CurrentAuditDone;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _auditTesting.RunAudits();
            }
            catch (NoAuditsLoadedException ex)
            {
                MessageBox.Show("There are no audits to run!", "No Audits", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (System.Net.Mail.SmtpException mailSmtpException)
            {
                this.Refresh();

                // Change the icon to the "paused" icon
                foreach (ListViewItem objItem in lsvAudits.Items)
                {
                    objItem.ImageIndex = 4;
                    objItem.SubItems[2].Text = "Email Failure";
                    this.Refresh();
                }

                MessageBox.Show(mailSmtpException.Message, "NAudit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                this.Refresh();

                // Change the icon to the "paused" icon
                foreach (ListViewItem objItem in lsvAudits.Items)
                {
                    objItem.ImageIndex = 4;
                    objItem.SubItems[2].Text = "Failure";
                    this.Refresh();
                }

                MessageBox.Show(ex.Message, "NAudit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }		
        }

        private void lsvAudits_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ListView currView;
            ListViewItem currItem;

            if (lsvAudits.SelectedItems.Count == 0)
            {
                if (ppgAudits.SelectedObject != null)
                {
                    ppgAudits.SelectedObject = null;
                }
            }
            else
            {
                ppgAudits.SelectedObject = null;

                currView = ((ListView)(sender));

                try
                {
                    currItem = currView.SelectedItems[0];
                    ppgAudits.SelectedObject = currItem.Tag;
                    _selectedIndex = currItem.Index;
                }
                catch (Exception ex)
                {
                }

                if (xpgGroups.GroupState == NETXP.Controls.TaskPane.GroupState.Expanded) 
                {				
                    xpgGroups.GroupState = NETXP.Controls.TaskPane.GroupState.Collapsed;
                    xpgAudits.GroupState = NETXP.Controls.TaskPane.GroupState.Expanded;
                }
            } // end if
        }

        private void btnLoadGroup_Activate(object sender, EventArgs e)
        {
            LoadFileDialog();
        }

        private void OpenAudits_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string auditName = OpenAudits.FileName;

            this.Text = string.Empty;
            this.Text = "DataAuditor";
            this.Text = this.Text + " - [" + auditName + "]";

            LoadAuditGroup(auditName);

            XpLinkRun.Enabled = true;		
        }

        private void lblAuditGroup_Click(object sender, System.EventArgs e)
        {
            ppgAudits.SelectedObject = null;

            xpgGroups.GroupState = NETXP.Controls.TaskPane.GroupState.Expanded;
            xpgAudits.GroupState = NETXP.Controls.TaskPane.GroupState.Collapsed;		
        }

        private void _auditTesting_AuditTestingStarting()
        {
            this.Refresh();

            // Change the icon to the "paused" icon
            foreach (ListViewItem objItem in lsvAudits.Items)
            {
                objItem.ImageIndex = 2;
                objItem.SubItems[2].Text = "Waiting...";

                this.Refresh();
            }
        }

        private void _auditTesting_CurrentAuditRunning(int AuditNumber, string AuditName)
        {
            lsvAudits.Items[AuditNumber].ImageIndex = 1;
            lsvAudits.Items[AuditNumber].SubItems[2].Text = "Processing";

            this.Refresh();
        }

        private void _auditTesting_CurrentAuditDone(int AuditNumber, string AuditName)
        {
            Audit currAudit = _audits.AuditGroup[AuditNumber];
            bool result = currAudit.Result;

            if (result)
            {
                lsvAudits.Items[AuditNumber].ImageIndex = 3;
                lsvAudits.Items[AuditNumber].SubItems[2].Text = "Completed";
            }
            else
            {
                lsvAudits.Items[AuditNumber].ImageIndex = 4;
                lsvAudits.Items[AuditNumber].SubItems[2].Text = "Failure";
            }

            this.Refresh();
        }

        private void xpLinkAuditRun_Click(object sender, System.EventArgs e)
        {
            RunSingleAudit();
        }

        private void btnRunAudit_Activate(object sender, EventArgs e)
        {
            RunSingleAudit();
        }

        private void _auditTesting_CurrentSingleAuditRunning(Audit CurrentAudit)
        {
            
            lsvAudits.Items[_selectedIndex].ImageIndex = 1;
            lsvAudits.Items[_selectedIndex].SubItems[2].Text = "Processing";

            this.Refresh();
        }

        private void _auditTesting_CurrentSingleAuditDone(Audit CurrentAudit)
        {
            bool result = CurrentAudit.Result;

            if (result)
            {
                lsvAudits.Items[_selectedIndex].ImageIndex = 3;
                lsvAudits.Items[_selectedIndex].SubItems[2].Text = "Completed";
            }
            else
            {
                lsvAudits.Items[_selectedIndex].ImageIndex = 4;
                lsvAudits.Items[_selectedIndex].SubItems[2].Text = "Failure";
            }

            this.Refresh();
        }

    #endregion

    #region  Private Members 

        private void LoadFileDialog()
        {
            OpenAudits.ShowDialog();
        }

        private void LoadAuditGroup(string xmlGroup)
        {
            ListViewItem currViewItem;
            Audit currAudit;
            int NodeCount;

            lsvAudits.Items.Clear();

            _audits = new AuditController(xmlGroup);

            lblAuditGroup.Text = _audits.AuditGroupName;

            foreach (Audit currItem in _audits.AuditGroup)
            {
                currAudit = currItem;

                currViewItem = lsvAudits.Items.Add(currAudit.Name, 0);

                currViewItem.Tag = currAudit;

                currViewItem.SubItems[0].Text = currAudit.Name;
                currViewItem.SubItems.Add("");
                currViewItem.SubItems.Add("");
            }
        }

        private void RunSingleAudit()
        {
            _auditTesting = new AuditTesting();
            bool result = false;

            // Wire events
            _auditTesting.CurrentSingleAuditRunning += _auditTesting_CurrentSingleAuditRunning;
            _auditTesting.CurrentSingleAuditDone += _auditTesting_CurrentSingleAuditDone;

            var currentAudit = (Audit)lsvAudits.Items[_selectedIndex].Tag;

            this.Refresh();

            _auditTesting.RunAudit(ref currentAudit);
        }

    #endregion

        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }

    } // end of class

} //end of root namespace