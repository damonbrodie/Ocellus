using System;
using System.Net;
using System.Windows.Forms;
using System.Speech.Synthesis;


namespace ConfigureForm
{
    public partial class Configuration : Form
    {

        public CookieContainer Cookie { get; set; }

        public string LoginResponse { get; set; }

        public string currentState { get; set; }

        public Configuration(string _state)
        {
            InitializeComponent();
            currentState = _state;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            switch (currentState)
            {
                case "ok":
                    txtCurrentStatus.Text = "Authenticated";
                    break;
                default:
                    txtCurrentStatus.Text = "Not Authenticated";
                    break;
            }

            string startupVoice = PluginRegistry.getStringValue("startupVoice");
            string eddnVoice = PluginRegistry.getStringValue("eddnVoice");
            string updateVoice = PluginRegistry.getStringValue("updateVoice");
            string engineVoice = PluginRegistry.getStringValue("engineVoice");
            string errorVoice = PluginRegistry.getStringValue("errorVoice");

            int counter = 0;
            bool foundStartup = false;
            bool foundEddn = false;
            bool foundUpdate = false;
            bool foundEngine = false;
            bool foundError = false;
            SpeechSynthesizer reader = new SpeechSynthesizer();
            foreach (InstalledVoice voice in reader.GetInstalledVoices())
            {
                try
                {
                    reader.SelectVoice(voice.VoiceInfo.Name);
                    cmbStartupVoice.Items.Add(voice.VoiceInfo.Name);
                    cmbEddnVoice.Items.Add(voice.VoiceInfo.Name);
                    cmbUpdateVoice.Items.Add(voice.VoiceInfo.Name);
                    cmbEngineVoice.Items.Add(voice.VoiceInfo.Name);
                    cmbErrorVoice.Items.Add(voice.VoiceInfo.Name);
                    if (startupVoice == voice.VoiceInfo.Name)
                    {
                        cmbStartupVoice.SelectedIndex = counter;
                        foundStartup = true;
                    }
                    if (eddnVoice == voice.VoiceInfo.Name)
                    {
                        cmbEddnVoice.SelectedIndex = counter;
                        foundEddn = true;
                    }
                    if (updateVoice == voice.VoiceInfo.Name)
                    {
                        cmbUpdateVoice.SelectedIndex = counter;
                        foundUpdate = true;
                    }
                    if (engineVoice == voice.VoiceInfo.Name)
                    {
                        cmbEngineVoice.SelectedIndex = counter;
                        foundEngine = true;
                    }
                    if (errorVoice == voice.VoiceInfo.Name)
                    {
                        cmbErrorVoice.SelectedIndex = counter;
                        foundError = true;
                    }
                        counter++;
                }
                catch
                {
                    Debug.Write("Error:  Problem with voice, skipping: " + voice.VoiceInfo.Name);
                }
            }

            if (!foundStartup)
            {
                cmbStartupVoice.SelectedIndex = 0;
            }
            if (!foundEddn)
            {
                cmbEddnVoice.SelectedIndex = 0;
            }
            if (!foundUpdate)
            {
                cmbUpdateVoice.SelectedIndex = 0;
            }
            if (!foundEngine)
            {
                cmbEngineVoice.SelectedIndex = 0;
            }
            if (!foundError)
            {
                cmbErrorVoice.SelectedIndex = 0;
            }

            txt_email.Text = PluginRegistry.getStringValue("email");
            txt_password.Text = PluginRegistry.getStringValue("password");

            if (PluginRegistry.getStringValue("startupNotification") == "tts")
            {
                rdoStartupTTS.Checked = true;
                txtStartupTTS.Text = PluginRegistry.getStringValue("startupText");
            }
            else if (PluginRegistry.getStringValue("startupNotification") == "sound")
            {
                rdoStartupSound.Checked = true;
                txtStartupSoundFile.Text = PluginRegistry.getStringValue("startupSound");
            }
            else //None
            {
                rdoStartupNoNotification.Checked = true;
            }


            if (PluginRegistry.getStringValue("eddnNotification") == "tts")
            {
                rdoEddnTTS.Checked = true;
                txtEddnTTS.Text = PluginRegistry.getStringValue("eddnText");
            }
            else if (PluginRegistry.getStringValue("eddnNotification") == "sound")
            {
                rdoEddnSound.Checked = true;
                txtEddnSoundFile.Text = PluginRegistry.getStringValue("eddnSound");
            }
            else //None
            {
                rdoEddnNoNotification.Checked = true;
            }


            if (PluginRegistry.getStringValue("updateNotification") == "tts")
            {
                rdoUpdateTTS.Checked = true;
                txtUpdateTTS.Text = PluginRegistry.getStringValue("updateText");
            }
            else if (PluginRegistry.getStringValue("updateNotification") == "sound")
            {
                rdoUpdateSound.Checked = true;
                txtUpdateSoundFile.Text = PluginRegistry.getStringValue("updateSound");
            }
            else //None
            {
                rdoUpdateNoNotification.Checked = true;
            }


            if (PluginRegistry.getStringValue("engineNotification") == "tts")
            {
                rdoEngineTTS.Checked = true;
                txtEngineTTS.Text = PluginRegistry.getStringValue("engineText");
            }
            else if (PluginRegistry.getStringValue("engineNotification") == "sound")
            {
                rdoEngineSound.Checked = true;
                txtEngineSoundFile.Text = PluginRegistry.getStringValue("engineSound");
            }
            else //None
            {
                rdoUpdateNoNotification.Checked = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            txtCurrentStatus.Text = "";
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
            else
            {
                lbl_validation_password.Visible = false;
            }

            if (!error)
            {
                PluginRegistry.setStringValue("email", txt_email.Text);
                PluginRegistry.setStringValue("password", txt_password.Text);

                CookieContainer cookieContainer = new CookieContainer();

                Tuple<CookieContainer, string> tAuthentication = Companion.loginToAPI(cookieContainer);

                this.Cookie = tAuthentication.Item1;
                string loginResponse = tAuthentication.Item2;

                Debug.Write("Login Response:  " + loginResponse);

                if (loginResponse == "verification" || loginResponse == "ok")
                {
                    this.LoginResponse = loginResponse;

                    // XXX Make this share the code with Save

                    PluginRegistry.setStringValue("email", txt_email.Text);
                    PluginRegistry.setStringValue("password", txt_password.Text);

                    if (rdoStartupNoNotification.Checked)
                    {
                        PluginRegistry.setStringValue("startupNotification", "none");
                    }
                    else if (rdoStartupTTS.Checked)
                    {
                        PluginRegistry.setStringValue("startupNotification", "tts");
                    }
                    else if (rdoStartupSound.Checked)
                    {
                        PluginRegistry.setStringValue("startupNotification", "sound");
                    }
                    PluginRegistry.setStringValue("startupSound", txtStartupSoundFile.Text);
                    PluginRegistry.setStringValue("startupText", txtStartupTTS.Text);
                    PluginRegistry.setStringValue("startupVoice", cmbStartupVoice.SelectedItem.ToString());


                    if (rdoEddnNoNotification.Checked)
                    {
                        PluginRegistry.setStringValue("eddnNotification", "none");
                    }
                    else if (rdoEddnTTS.Checked)
                    {
                        PluginRegistry.setStringValue("eddnNotification", "tts");
                    }
                    else if (rdoEddnSound.Checked)
                    {
                        PluginRegistry.setStringValue("eddnNotification", "sound");
                    }

                    PluginRegistry.setStringValue("eddnSound", txtEddnSoundFile.Text);
                    PluginRegistry.setStringValue("eddnText", txtEddnTTS.Text);
                    PluginRegistry.setStringValue("eddnVoice", cmbEddnVoice.SelectedItem.ToString());


                    if (rdoUpdateNoNotification.Checked)
                    {
                        PluginRegistry.setStringValue("updateNotification", "none");
                    }
                    else if (rdoUpdateTTS.Checked)
                    {
                        PluginRegistry.setStringValue("updateNotification", "tts");
                    }
                    else if (rdoUpdateSound.Checked)
                    {
                        PluginRegistry.setStringValue("updateNotification", "sound");
                    }
                    PluginRegistry.setStringValue("updateText", txtUpdateTTS.Text);
                    PluginRegistry.setStringValue("updateSound", txtUpdateSoundFile.Text);
                    PluginRegistry.setStringValue("updateVoice", cmbUpdateVoice.SelectedItem.ToString());

                    if (rdoEngineNoNotification.Checked)
                    {
                        PluginRegistry.setStringValue("engineNotification", "none");
                    }
                    else if (rdoUpdateTTS.Checked)
                    {
                        PluginRegistry.setStringValue("engineNotification", "tts");
                    }
                    else if (rdoUpdateSound.Checked)
                    {
                        PluginRegistry.setStringValue("engineNotification", "sound");
                    }
                    PluginRegistry.setStringValue("engineText", txtEngineTTS.Text);
                    PluginRegistry.setStringValue("engineSound", txtEngineSoundFile.Text);
                    PluginRegistry.setStringValue("engineVoice", cmbEngineVoice.SelectedItem.ToString());

                    PluginRegistry.setStringValue("errorVoice", cmbErrorVoice.SelectedItem.ToString());

                    this.Close();
                }
            }
            else
            {
                txtCurrentStatus.Text = "Not Authenticated";
                MessageBox.Show("Invalid Credentials, please re-enter them and retry");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfigurationSave_Click(object sender, EventArgs e)
        {
            PluginRegistry.setStringValue("email", txt_email.Text);
            PluginRegistry.setStringValue("password", txt_password.Text);

            if (rdoStartupNoNotification.Checked)
            {
                PluginRegistry.setStringValue("startupNotification", "none");
            }
            else if (rdoStartupTTS.Checked)
            {
                PluginRegistry.setStringValue("startupNotification", "tts");
            }
            else if (rdoStartupSound.Checked)
            {
                PluginRegistry.setStringValue("startupNotification", "sound");
            }
            PluginRegistry.setStringValue("startupSound", txtStartupSoundFile.Text);
            PluginRegistry.setStringValue("startupText", txtStartupTTS.Text);
            PluginRegistry.setStringValue("startupVoice", cmbStartupVoice.SelectedItem.ToString());


            if (rdoEddnNoNotification.Checked)
            {
                PluginRegistry.setStringValue("eddnNotification", "none");
            }
            else if (rdoEddnTTS.Checked)
            {
                PluginRegistry.setStringValue("eddnNotification", "tts");
            }
            else if (rdoEddnSound.Checked)
            {
                PluginRegistry.setStringValue("eddnNotification", "sound");
            }

            PluginRegistry.setStringValue("eddnSound", txtEddnSoundFile.Text);
            PluginRegistry.setStringValue("eddnText", txtEddnTTS.Text);
            PluginRegistry.setStringValue("eddnVoice", cmbEddnVoice.SelectedItem.ToString());


            if (rdoUpdateNoNotification.Checked)
            {
                PluginRegistry.setStringValue("updateNotification", "none");
            }
            else if (rdoUpdateTTS.Checked)
            {
                PluginRegistry.setStringValue("updateNotification", "tts");
            }
            else if (rdoUpdateSound.Checked)
            {
                PluginRegistry.setStringValue("updateNotification", "sound");
            }
            PluginRegistry.setStringValue("updateText", txtUpdateTTS.Text);
            PluginRegistry.setStringValue("updateSound", txtUpdateSoundFile.Text);
            PluginRegistry.setStringValue("updateVoice", cmbUpdateVoice.SelectedItem.ToString());


            if (rdoEngineNoNotification.Checked)
            {
                PluginRegistry.setStringValue("engineNotification", "none");
            }
            else if (rdoEngineTTS.Checked)
            {
                PluginRegistry.setStringValue("engineNotification", "tts");
            }
            else if (rdoEngineSound.Checked)
            {
                PluginRegistry.setStringValue("engineNotification", "sound");
            }
            PluginRegistry.setStringValue("engineText", txtEngineTTS.Text);
            PluginRegistry.setStringValue("engineSound", txtEngineSoundFile.Text);
            PluginRegistry.setStringValue("engineVoice", cmbEngineVoice.SelectedItem.ToString());

            PluginRegistry.setStringValue("errorVoice", cmbErrorVoice.SelectedItem.ToString());

            this.Close();
        }

        private void cmb_StartupVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            rdoStartupTTS.Checked = true;
        }

        private void txt_StartupTTS_TextChanged(object sender, EventArgs e)
        {
            rdoStartupTTS.Checked = true;
        }

        private void btnStartupSoundSelect_Click(object sender, EventArgs e)
        {
            rdoStartupSound.Checked = true;

            var result = dlgFile.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                txtStartupSoundFile.Text = dlgFile.FileName;
            }
        }

        private void btnStartupSoundPreview_Click(object sender, EventArgs e)
        {
            Announcements.playSound(txtStartupSoundFile.Text);
        }

        private void btnPreviewStartupTTS_Click(object sender, EventArgs e)
        {
            Announcements.read(txtStartupTTS.Text, cmbStartupVoice.SelectedItem.ToString(), true);
        }

        private void btnPreviewEddnTTS_Click(object sender, EventArgs e)
        {
            Announcements.read(txtEddnTTS.Text, cmbEddnVoice.SelectedItem.ToString(),true);
        }

        private void btnPreviewUpdateTTS_Click(object sender, EventArgs e)
        {
            Announcements.read(txtUpdateTTS.Text, cmbUpdateVoice.SelectedItem.ToString(), true);
        }

        private void btnUpdateSoundPreview_Click(object sender, EventArgs e)
        {
            Announcements.playSound(txtUpdateSoundFile.Text);
        }

        private void btnEddnSoundPreview_Click(object sender, EventArgs e)
        {
            Announcements.playSound(txtEddnSoundFile.Text);
        }

        private void btnEddnSoundSelect_Click(object sender, EventArgs e)
        {
            rdoEddnSound.Checked = true;

            var result = dlgFile.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                txtEddnSoundFile.Text = dlgFile.FileName;
            }
        }

        private void btnUpdateSoundSelect_Click(object sender, EventArgs e)
        {
            rdoUpdateSound.Checked = true;

            var result = dlgFile.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                txtUpdateSoundFile.Text = dlgFile.FileName;
            }
        }

        private void cmbEddnVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            rdoEddnTTS.Checked = true;
        }

        private void txtEddnTTS_TextChanged(object sender, EventArgs e)
        {
            rdoEddnTTS.Checked = true;
        }

        private void cmbUpdateVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            rdoUpdateTTS.Checked = true;
        }

        private void txtUpdateTTS_TextChanged(object sender, EventArgs e)
        {
            rdoUpdateTTS.Checked = true;
        }

        private void cmbEngineVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            rdoEngineTTS.Checked = true;
        }

        private void btnPreviewEngineTTS_Click(object sender, EventArgs e)
        {
            Announcements.read(txtEngineTTS.Text, cmbEngineVoice.SelectedItem.ToString(), true);
        }

        private void txtEngineTTS_TextChanged(object sender, EventArgs e)
        {
            rdoEngineTTS.Checked = true;
        }

        private void btnEngineSoundSelect_Click(object sender, EventArgs e)
        {
            rdoEngineSound.Checked = true;

            var result = dlgFile.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                txtEngineSoundFile.Text = dlgFile.FileName;
            }
        }

        private void btnEngineSoundPreview_Click(object sender, EventArgs e)
        {
            Announcements.playSound(txtEngineSoundFile.Text);
        }
    }
}