using System;
using System.Speech.Synthesis;
using System.Threading;

// *******************************************
// *  Functions for handling Text to Speech  *
// *******************************************
class TextToSpeech
{
    public static void read(string text)
    {
        if (text == null || text == "")
        {
            return;
        }
        SpeechSynthesizer reader = new SpeechSynthesizer();
        string pluginVoice = PluginRegistry.getStringValue("pluginVoice");


        foreach (InstalledVoice voice in reader.GetInstalledVoices())
        {
            try
            {
                if (pluginVoice == voice.VoiceInfo.Name)
                {
                    reader.SelectVoice(voice.VoiceInfo.Name);
                }
            }
            catch
            {
                Debug.Write("Error:  Unable to set voice: " + voice.VoiceInfo.Name);
            }
        }
        reader.Speak(text);
        reader.Dispose();
    }
    public static void announcements()
    {
        try
        {
            string email = PluginRegistry.getStringValue("email");
            if (email == null || email == "")
            {
                read("Welcome to the Ocellus Assistant.  Say configure plug in to begin.");
            }
            else
            {
                read(PluginRegistry.getStringValue("startupText"));
            }
            Thread.Sleep(3000);
            if (Upgrade.needUpgrade())
            {
                read(PluginRegistry.getStringValue("updateAvailableText"));
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }
    }

}
