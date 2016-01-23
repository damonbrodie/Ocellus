using System;
using System.Windows.Forms;

namespace Validate
{
    public partial class Validate : Form
    {
        public Validate()
        {
            InitializeComponent();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submit_Click(object sender, EventArgs e)
        {
            if (txt_verification.Text.Length == 0)
            {
                lbl_validation_verification.Visible = true;
            }
            else
            {
                lbl_validation_verification.Visible = false;
                this.Close();
            }
        }
    }
}
