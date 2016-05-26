using System;
using System.Collections.Generic;
using System.Windows.Forms;


// *************************************
// *  Functions for Web Variable Form  *
// *************************************

namespace WebVars
{
    
    public partial class EditWebVars : Form
    {
        public EditWebVars()
        {
            InitializeComponent();
        }

        public void testJson(string url)
        {
            Tuple<bool, string, Dictionary<string, string>, Dictionary<string, int>, Dictionary<string, bool>, string> tResponse = GetWebVars.requestWebVars(url);

            while (dataGridTest.Rows.Count > 0)
            {
                dataGridTest.Rows.RemoveAt(0);
            }

            if (tResponse.Item1 == true)
            {
                txtServerStatus.Text = tResponse.Item2;
                txtNumVariables.Text = "";
                txtWarnings.Text = "";
                txtJson.Text = "";
                return;
            }

            txtServerStatus.Text = "Ok";
            txtWarnings.Text = tResponse.Item2;
            txtJson.Text = tResponse.Item6;
            int counter = 0;
            foreach (string key in tResponse.Item3.Keys)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridTest.RowTemplate.Clone();
                row.CreateCells(dataGridTest, key, tResponse.Item3[key]);
                dataGridTest.Rows.Add(row);
                counter++;

            }
            foreach (string key in tResponse.Item4.Keys)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridTest.RowTemplate.Clone();
                row.CreateCells(dataGridTest, key, tResponse.Item4[key]);
                dataGridTest.Rows.Add(row);
                counter++;
            }
            foreach (string key in tResponse.Item5.Keys)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridTest.RowTemplate.Clone();
                row.CreateCells(dataGridTest, key, tResponse.Item5[key]);
                dataGridTest.Rows.Add(row);
                counter++;
            }
            txtNumVariables.Text = counter.ToString();

        }

        private void btnTest1_Click(object sender, EventArgs e)
        {
            testJson(txtJson1.Text);
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            testJson(txtJson2.Text);
        }

        private void btnTest3_Click(object sender, EventArgs e)
        {
            testJson(txtJson3.Text);
        }

        private void btnTest4_Click(object sender, EventArgs e)
        {
            testJson(txtJson4.Text);
        }

        private void btnTest5_Click(object sender, EventArgs e)
        {
            testJson(txtJson5.Text);
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

            if (counter == 1)
            {
                PluginRegistry.setStringValue("webVar", "none");
            }

            while (counter <= 5)
            {
                PluginRegistry.deleteKey("WebVar" + counter.ToString());
                counter++;
            }

            this.Close();
        }

        private void EditWebVars_Load(object sender, EventArgs e)
        {
            bool moreVarsInRegistry = true;
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
                    string haveWebVar = PluginRegistry.getStringValue("webVar");
                    if (haveWebVar == null)
                    {
                        txtJson1.Text = "http://ocellus.io/webvars_test";
                    }
                    btnTest1.Enabled = true;
                }
            }
        }

        private void txtJson1_TextChanged(object sender, EventArgs e)
        {
            if (txtJson1.Text.Length > 0)
            {
                btnTest1.Enabled = true;
            }
            else
            {
                btnTest1.Enabled = false;
            }
        }

        private void txtJson2_TextChanged(object sender, EventArgs e)
        {
            if (txtJson2.Text.Length > 0)
            {
                btnTest2.Enabled = true;
            }
            else
            {
                btnTest2.Enabled = false;
            }
        }

        private void txtJson3_TextChanged(object sender, EventArgs e)
        {
            if (txtJson3.Text.Length > 0)
            {
                btnTest3.Enabled = true;
            }
            else
            {
                btnTest3.Enabled = false;
            }
        }

        private void txtJson4_TextChanged(object sender, EventArgs e)
        {
            if (txtJson4.Text.Length > 0)
            {
                btnTest4.Enabled = true;
            }
            else
            {
                btnTest4.Enabled = false;
            }
        }

        private void txtJson5_TextChanged(object sender, EventArgs e)
        {
            if (txtJson5.Text.Length > 0)
            {
                btnTest5.Enabled = true;
            }
            else
            {
                btnTest5.Enabled = false;
            }
        }
    }
}
