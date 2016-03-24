namespace Toolbox
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabToolbox = this.Factory.CreateRibbonTab();
            this.groupTreemaps = this.Factory.CreateRibbonGroup();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.groupData = this.Factory.CreateRibbonGroup();
            this.buttonTreemap = this.Factory.CreateRibbonButton();
            this.buttonCircularTreemap = this.Factory.CreateRibbonButton();
            this.buttonParameters = this.Factory.CreateRibbonButton();
            this.buttonDataSet1 = this.Factory.CreateRibbonButton();
            this.buttonDataSet2 = this.Factory.CreateRibbonButton();
            this.buttonDataSet3 = this.Factory.CreateRibbonButton();
            this.tabToolbox.SuspendLayout();
            this.groupTreemaps.SuspendLayout();
            this.groupData.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabToolbox
            // 
            this.tabToolbox.Groups.Add(this.groupTreemaps);
            this.tabToolbox.Groups.Add(this.groupData);
            this.tabToolbox.Label = "Charting";
            this.tabToolbox.Name = "tabToolbox";
            // 
            // groupTreemaps
            // 
            this.groupTreemaps.Items.Add(this.buttonTreemap);
            this.groupTreemaps.Items.Add(this.buttonCircularTreemap);
            this.groupTreemaps.Items.Add(this.separator1);
            this.groupTreemaps.Items.Add(this.buttonParameters);
            this.groupTreemaps.Label = "Charts";
            this.groupTreemaps.Name = "groupTreemaps";
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // groupData
            // 
            this.groupData.Items.Add(this.buttonDataSet1);
            this.groupData.Items.Add(this.buttonDataSet2);
            this.groupData.Items.Add(this.buttonDataSet3);
            this.groupData.Label = "Test Data";
            this.groupData.Name = "groupData";
            // 
            // buttonTreemap
            // 
            this.buttonTreemap.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonTreemap.Image = global::Toolbox.Properties.Resources.treemap_icon;
            this.buttonTreemap.Label = "Treemap";
            this.buttonTreemap.Name = "buttonTreemap";
            this.buttonTreemap.ShowImage = true;
            this.buttonTreemap.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonTreemap_Click);
            // 
            // buttonCircularTreemap
            // 
            this.buttonCircularTreemap.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonCircularTreemap.Image = global::Toolbox.Properties.Resources.treemap_icon;
            this.buttonCircularTreemap.Label = "Circular Treemap";
            this.buttonCircularTreemap.Name = "buttonCircularTreemap";
            this.buttonCircularTreemap.ShowImage = true;
            this.buttonCircularTreemap.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonCircularTreemap_Click);
            // 
            // buttonParameters
            // 
            this.buttonParameters.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonParameters.Enabled = false;
            this.buttonParameters.Image = global::Toolbox.Properties.Resources.tools_icon;
            this.buttonParameters.Label = "Parameters";
            this.buttonParameters.Name = "buttonParameters";
            this.buttonParameters.ShowImage = true;
            this.buttonParameters.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonParameters_Click);
            // 
            // buttonDataSet1
            // 
            this.buttonDataSet1.Label = "Data Set 25 items";
            this.buttonDataSet1.Name = "buttonDataSet1";
            this.buttonDataSet1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonDataSet1_Click_1);
            // 
            // buttonDataSet2
            // 
            this.buttonDataSet2.Label = "Data Set 150 items";
            this.buttonDataSet2.Name = "buttonDataSet2";
            this.buttonDataSet2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonDataSet2_Click);
            // 
            // buttonDataSet3
            // 
            this.buttonDataSet3.Label = "Data Set 1000 items";
            this.buttonDataSet3.Name = "buttonDataSet3";
            this.buttonDataSet3.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonDataSet3_Click);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabToolbox);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tabToolbox.ResumeLayout(false);
            this.tabToolbox.PerformLayout();
            this.groupTreemaps.ResumeLayout(false);
            this.groupTreemaps.PerformLayout();
            this.groupData.ResumeLayout(false);
            this.groupData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabToolbox;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupTreemaps;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonTreemap;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonDataSet1;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonParameters;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupData;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonDataSet2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonDataSet3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonCircularTreemap;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
