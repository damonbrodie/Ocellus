using System;
using System.Net;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace ConfigureForm
{
    public partial class Login : Form
    {

        public CookieContainer Cookie { get; set; }

        public string LoginResponse { get; set; }
        public Login()
        {
            InitializeComponent();
        }
            
        private void Login_Load(object sender, EventArgs e)
        {
            txt_email.Text = PluginRegistry.getStringValue("email");
            txt_password.Text = PluginRegistry.getStringValue("password");
            txt_EDDN_Updates.Text = PluginRegistry.getStringValue("eddnUpdateText");
            txt_Startup.Text = PluginRegistry.getStringValue("startupText");
            txt_Updates.Text = PluginRegistry.getStringValue("updateAvailableText");
            string pluginVoice = PluginRegistry.getStringValue("pluginVoice");

            int counter = 0;
            bool found = false;
            SpeechSynthesizer reader = new SpeechSynthesizer();
            foreach (InstalledVoice voice in reader.GetInstalledVoices())
            {
                try
                {
                    reader.SelectVoice(voice.VoiceInfo.Name);
                    cmb_Voice.Items.Add(voice.VoiceInfo.Name);
                    if (pluginVoice == voice.VoiceInfo.Name)
                    {
                        cmb_Voice.SelectedIndex = counter;
                        found = true;
                    }
                    counter++;
                }
                catch
                {
                    Debug.Write("Error:  Problem with voice, skipping: " + voice.VoiceInfo.Name);
                }
            }
            if (!found)
            {
                cmb_Voice.SelectedIndex = 0;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            bool error = false;
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
                PluginRegistry.setStringValue("eddnUpdateText", txt_EDDN_Updates.Text);
                PluginRegistry.setStringValue("startupText", txt_Startup.Text);
                PluginRegistry.setStringValue("updateAvailableText", txt_Updates.Text);
                PluginRegistry.setStringValue("pluginVoice", cmb_Voice.SelectedItem.ToString());
                
                CookieContainer cookieContainer = new CookieContainer();

                Tuple<CookieContainer, string> tAuthentication = Companion.loginToAPI(cookieContainer);

                this.Cookie = tAuthentication.Item1;
                string loginResponse = tAuthentication.Item2;

                Debug.Write("loginResponse:  " + loginResponse);

                if (loginResponse == "verification" || loginResponse == "ok")
                {
                    this.LoginResponse = loginResponse;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Credentials, please re-enter them and retry");
                }
            }
            else
            {
                MessageBox.Show("Invalid Credentials, please re-enter them and retry");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}