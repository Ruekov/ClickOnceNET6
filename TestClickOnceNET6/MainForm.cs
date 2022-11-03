using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClickOnceNET6
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = string.Format("Test ClickOnce NET6 ver.{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private async void bttnCheck_Click(object sender, EventArgs e)
        {
            //Check if there are clickonce updates
           if (await App.OnProcessAction() == false)
            {
                // up`date!
                this.Close();   
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
