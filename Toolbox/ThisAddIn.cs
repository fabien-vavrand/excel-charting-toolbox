using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Toolbox.Charts;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Toolbox.View;
using Toolbox.ViewModel.Treemap;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using Microsoft.Office.Core;
using System.Drawing;

namespace Toolbox
{
    public partial class ThisAddIn
    {
        #region Properties
        public List<ChartBase> Charts { get; set; }
        public Microsoft.Office.Tools.CustomTaskPane TaskPane { get; set; }
        public System.Windows.Controls.UserControl TaskPaneControl { get; set; }
        #endregion

        #region Events
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Charts = new List<ChartBase>();
            Messenger.Default.Register<NotificationMessage<ChartBase>>(
                this,
                "ChartUnactivated",
                (m) =>
                {
                    TaskPane.Visible = false;
                    var toRemove = Charts.Where(c => !c.IsActive).ToList();
                    toRemove.ForEach(c => Charts.Remove(c));
                });
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }
        #endregion

        #region Methods
        public void SetTaskPaneViewModel(ViewModelBase vm)
        {
            if (TaskPane == null)
            {
                TaskPaneControl = new TreemapView((TreemapViewModel)vm);
                ElementHost host = new ElementHost { Child = TaskPaneControl };
                host.Dock = DockStyle.Fill;
                UserControl userControl = new UserControl();
                userControl.BackColor = Color.White;
                userControl.Controls.Add(host);
                TaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(userControl, "Treemap");
                TaskPane.VisibleChanged += (sender, e) =>
                {
                    //((TreemapViewModel)vm).Treemap.IsActive = false;
                };
            }
            else
            {
                TaskPaneControl.DataContext = vm;
            }

            TaskPane.Width = 400;
            TaskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
            TaskPane.Visible = true;
        }
        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
