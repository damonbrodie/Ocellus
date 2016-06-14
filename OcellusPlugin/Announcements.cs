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
    public static void read(string text, string voice, bool useAsync = false)
    {
        SpeechSynthesizer reader = new SpeechSynthesizer();
        if (voice != null && voice != "")
        {
            try
            {
                reader.SelectVoice(voice);
            }
            catch
            {
                Debug.Write("Error:  Unable to set voice: " + voice);
            }
        }
        reader.SetOutputToDefaultAudioDevice();
        if (useAsync)
        {
            reader.SpeakAsync(text);
        }
        else
        {
            reader.Speak(text);
        }
    }

    public static void speak(Elite.MessageBus messageBus)
    {
        SpeechSynthesizer reader = new SpeechSynthesizer();

        while (true)
        {
            if (messageBus.spokenAnnouncements.Count > 0)
            {
                try
                {
                    Elite.Speech message = messageBus.spokenAnnouncements[0];
                    messageBus.spokenAnnouncements.RemoveAt(0);
                    if (message.voice != null && message.voice != "")
                    {
                        try
                        {
                            reader.SelectVoice(message.voice);
                        }
                        catch
                        {
                            Debug.Write("Error:  Unable to set voice: " + message.voice);
                        }
                    }
                    reader.SetOutputToDefaultAudioDevice();
                    reader.Speak(message.text);
                    Thread.Sleep(1500);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }
            else
            {
                Thread.Sleep(250);
            }
        }
    }

    public static void addSpeech(Elite.MessageBus messageBus, string text, string voice)
    {
        Elite.Speech newMessage = new Elite.Speech();
        newMessage.text = text;
        newMessage.voice = voice;
        messageBus.spokenAnnouncements.Add(newMessage);
    }

    public static void playSound(string file)
    {
        if (File.Exists(file))
        {
            SoundPlayer player = new SoundPlayer(file);
            player.Play();
        }
    }

    public static void errorAnnouncement(Elite.MessageBus messageBus, string error)
    {
        string voice = PluginRegistry.getStringValue("errorVoice");
        addSpeech(messageBus, error, voice);
    }

    public static void updateAnnouncement(Elite.MessageBus messageBus)
    {
        string announcementType = PluginRegistry.getStringValue("updateNotification");
        if (announcementType == "tts")
        {
            string voice = PluginRegistry.getStringValue("updateVoice");
            addSpeech(messageBus, PluginRegistry.getStringValue("updateText"), voice);
        }
        else if (announcementType == "sound")
        {
            string file = PluginRegistry.getStringValue("updateSound");
            playSound(file);
        }

    }

    public static void eddnAnnouncement(Elite.MessageBus messageBus)
    {
        string announcementType = PluginRegistry.getStringValue("eddnNotification");
        if (announcementType == "tts")
        {
            string voice = PluginRegistry.getStringValue("eddnVoice");
            addSpeech(messageBus, PluginRegistry.getStringValue("eddnText"), voice);
        }
        else if (announcementType == "sound")
        {
            string file = PluginRegistry.getStringValue("eddnSound");
            playSound(file);
        }
    }

    public static void startupNotifications(Elite.MessageBus messageBus, int registryCheck)
    {
        // This returns null if the registry key is missing - that's ok
        

        if (registryCheck == 1)
        {
            addSpeech(messageBus, "Welcome to the Ocellus Assistant.  Say configure plug in to begin.", null);
        }
        else if (registryCheck == 2 || registryCheck == 3)
        {
            addSpeech(messageBus, "New Configurations available.  Say Configure Plug in to make modifications.", null);
        }
        else
        {
            string announcementType = PluginRegistry.getStringValue("startupNotification");
            if (announcementType == "tts")
            {
                string voice = PluginRegistry.getStringValue("startupVoice");
                addSpeech(messageBus, PluginRegistry.getStringValue("startupText"), voice);
            }
            else if (announcementType == "sound")
            {
                string file = PluginRegistry.getStringValue("startupSound");
                playSound(file);
            }
        }
        if (Upgrade.needUpgrade())
        {
            updateAnnouncement(messageBus);
        }
    }
}
