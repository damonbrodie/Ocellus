using System;
using System.Windows.Forms;

namespace WebVars
{
    public partial class EditWebVars : Form
    {
        public EditWebVars()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int counter = 1;
            if (txtJson1.Text != string.Empty)
            {
                PluginRegistry.setStringValue("webVar" + counter.ToString(), txtJson1.Text);
                counter++;
            }
            if (txtJson2.Text != string.Empty)
            {
                PluginRegistry.setStringValue("webVar" + counter.ToString(), txtJson2.Text);
                counter++;
            }
            if (txtJson3.Text != string.Empty)
            {
                PluginRegistry.setStringValue("webVar" + counter.ToString(), txtJson3.Text);
                counter++;
            }
            if (txtJson4.Text != string.Empty)
            {
                PluginRegistry.setStringValue("webVar" + counter.ToString(), txtJson4.Text);
                counter++;
            }
            if (txtJson5.Text != string.Empty)
            {
                PluginRegistry.setStringValue("webVar" + counter.ToString(), txtJson5.Text);
                counter++;
            }
            while (counter <= 5)
            {
                PluginRegistry.deleteKey("WebVar" + counter.ToString());
                counter++;
            }
            this.Close();
        }

        private void btnTest1_Click(object sender, EventArgs e)
        {

        }

        private void EditWebVars_Load(object sender, EventArgs e)
        {
            Boolean moreVarsInRegistry = true;
            int counter = 1;
            while (moreVarsInRegistry == true)
            {
                string webVar = PluginRegistry.getStringValue("webVar" + counter.ToString());
                if (webVar != null)
                {
                    switch (counter)
                    {
                        case 1:
                            txtJson1.Text = webVar;
                            btnTest1.Enabled = true;
                            break;
                        case 2:
                            txtJson2.Text = webVar;
                            btnTest2.Enabled = true;
                            break;
                        case 3:
                            txtJson3.Text = webVar;
                            btnTest3.Enabled = true;
                            break;
                        case 4:
                            txtJson4.Text = webVar;
                            btnTest4.Enabled = true;
                            break;
                        case 5:
                            txtJson5.Text = webVar;
                            btnTest5.Enabled = true;
                            break;
                    }
                    if (counter >= 5)
                    {
                        moreVarsInRegistry = false;
                    }
                    counter++;
                }
                else
                {
                    moreVarsInRegistry = false;
                }
            }
        }
    }
}
