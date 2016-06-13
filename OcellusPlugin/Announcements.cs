using System;
using System.IO;
using System.Speech.Synthesis;
using System.Threading;
using System.Media;


// *******************************************
// *  Functions for handling Text to Speech  *
// *******************************************
class Announcements
{
    public static void read(string text, string useVoice)
    {
        if (text == null || text == "")
        {
            return;
        }
        SpeechSynthesizer reader = new SpeechSynthesizer();
        if (useVoice != null && useVoice != "")
        {
            try
            {
                reader.SelectVoice(useVoice);
            }
            catch
            {
                Debug.Write("Error:  Unable to set voice: " + useVoice);
            }
        }

        reader.Speak(text);
        reader.Dispose();
    }

    public static void playSound(string file)
    {
        if (File.Exists(file))
        {
            SoundPlayer player = new SoundPlayer(file);
            player.Play();
        }
    }

    public static void updateAnnouncement()
    {
        string announcementType = PluginRegistry.getStringValue("updateNotification");
        if (announcementType == "tts")
        {
            string voice = PluginRegistry.getStringValue("updateVoice");
            read(PluginRegistry.getStringValue("updateText"), voice);
        }
        else if (announcementType == "sound")
        {
            string file = PluginRegistry.getStringValue("updateSound");
            playSound(file);
        }

    }

    public static void eddnAnnouncement()
    {
        string announcementType = PluginRegistry.getStringValue("eddnNotification");
        if (announcementType == "tts")
        {
            string voice = PluginRegistry.getStringValue("eddnVoice");
            read(PluginRegistry.getStringValue("eddnText"), voice);
        }
        else if (announcementType == "sound")
        {
            string file = PluginRegistry.getStringValue("eddnSound");
            playSound(file);
        }
    }

    public static void startupNotifications(int registryCheck)
    {
        // This returns null if the registry key is missing - that's ok
        

        if (registryCheck == 1)
        {
            read("Welcome to the Ocellus Assistant.  Say configure plug in to begin.", null);
        }
        else if (registryCheck == 2)
        {
            read("New Configurations available.  Say Configure Plug in to make modifications.", null);
        }
        
        else
        {
            string announcementType = PluginRegistry.getStringValue("startupNotification");
            if (announcementType == "tts")
            {
                string voice = PluginRegistry.getStringValue("startupVoice");
                read(PluginRegistry.getStringValue("startupText"), voice);
            }
            else if (announcementType == "sound")
            {
                string file = PluginRegistry.getStringValue("startupSound");
                playSound(file);
            }
        }
        Thread.Sleep(3000);
        if (Upgrade.needUpgrade())
        {
            updateAnnouncement();
        }
    }
}
