using System;
using System.Net;
using System.Windows.Forms;

namespace VerificationCode
{
    public partial class Validate : Form
    {
        public CookieContainer Cookie
        {
            get; set;
        }

        public string VerifyResponse
        {
            get; set;
        }

        public Validate()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtVerification.Text.Length == 0)
            {
                lblVerificationMessage.Visible = true;
            }
            else
            {
                CookieContainer verifyCookies = this.Cookie;
                Tuple<CookieContainer, string> tVerify = Companion.verifyWithAPI(verifyCookies, txtVerification.Text);
                verifyCookies = tVerify.Item1;
                string verifyResponse = tVerify.Item2;

                if (verifyResponse == "ok")
                {
                    Web.WriteCookiesToDisk(Config.CookiePath(), verifyCookies);
                    this.VerifyResponse = verifyResponse;
                    this.Cookie = verifyCookies;
                    this.Close();
                }
                txtVerification.Text = "";
                lblVerificationMessage.Visible = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
