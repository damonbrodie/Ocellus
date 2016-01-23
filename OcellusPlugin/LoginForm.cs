﻿using System;
using System.Windows.Forms;

namespace Credentials
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txt_email.Text = PluginRegistry.getStringValue("email");
            txt_password.Text = PluginRegistry.getStringValue("password");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean error = false;
            try
            {
                var testEmail = new System.Net.Mail.MailAddress(txt_email.Text);
                lbl_validation_email.Visible = false;
            }
            catch
            {
                lbl_validation_email.Visible = true;
                error = true;
                
            }

            if (txt_password.Text.Length == 0)
            {
                lbl_validation_password.Visible = true;
                error = true;
            }
            else {
                lbl_validation_password.Visible = false;
            }

            if (!error)
            {
                PluginRegistry.setStringValue("email", txt_email.Text);
                PluginRegistry.setStringValue("password", txt_password.Text);

              //  CookieContainer cookieContainer = new CookieContainer();

              //  Tuple<CookieContainer, string> tAuthentication = Companion.loginToAPI();

              //  cookieContainer = tAuthentication.Item1;
             //   string loginResponse = tAuthentication.Item2;
             //   Utilities.writeDebug("loginResponse:  " + loginResponse);

             //   if (loginResponse == "verification" || loginResponse == "ok")

                    this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}