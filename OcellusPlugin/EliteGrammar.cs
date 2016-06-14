using System;
using System.IO;
using System.IO.Compression;
using System.Speech.Recognition;
using System.Threading.Tasks;
using System.Collections.Generic;


// ******************************************************************
// *  Functions creating and using speech engine for Elite Systems  *
// ******************************************************************
class EliteGrammar
{
    private const string grammarURL = "http://ocellus.io/data/systems_grammar.zip";

    public static bool downloadGrammar()
    {
        string zipFile = Path.Combine(Config.Path(), "systems_grammar.zip");
        string grammarFile = Path.Combine(Config.Path(), "systems_grammar.xml");
        if (File.Exists(grammarFile))
        {
            // Download the index once a week
            DateTime fileTime = File.GetLastWriteTime(grammarFile);
            DateTime weekago = DateTime.Now.AddDays(-7);
            if (fileTime > weekago)
            {
                return true;
            }
        }
        if (Web.downloadFile(grammarURL, zipFile))
        {
            File.Delete(grammarFile);
            ZipFile.ExtractToDirectory(zipFile, Config.Path());
            File.Delete(zipFile);
            return true;
        }
        else
        {
            Debug.Write("ERROR:  Unable to download Systems Grammar from Ocellus.io");
        }

        if (!File.Exists(grammarFile))
        {
            return false;
        }
        return true;
    }

    public static void loadGrammar(Elite.MessageBus messageBus)
    {
        SpeechRecognitionEngine recognitionEngine = messageBus.recognitionEngine;
        recognitionEngine.SetInputToDefaultAudioDevice();
        recognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
        recognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1.5);
        recognitionEngine.BabbleTimeout = TimeSpan.FromSeconds(5);
        string grammarFile = Path.Combine(Config.Path(), "systems_grammar.xml");

        DateTime startTime = DateTime.Now;
        Debug.Write("Begin loading star system grammar");
        bool grammarLoaded = true;
        try
        {
            Grammar grammar = new Grammar(grammarFile);
            recognitionEngine.LoadGrammar(grammar);
        }
        catch
        {
            grammarLoaded = false;
            Debug.Write("Error:  Unable to load grammar");
            Announcements.errorAnnouncement(messageBus, "Unable to load star system recognition engine");
            if (File.Exists(grammarFile))
            {
                File.Delete(grammarFile);
            }
        }
        if (grammarLoaded)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan loadTime = endTime - startTime;
            Debug.Write("Finished loading star system grammar.  Load time: " + loadTime.Seconds.ToString() + " seconds");
            Debug.Write("Recognition Engine - Audio Level: " + recognitionEngine.AudioLevel.ToString());
            Debug.Write("Recognition Engine - Audio Format: " + recognitionEngine.AudioFormat.ToString());
            Debug.Write("Recognition Engine - Grammars Loaded: " + recognitionEngine.Grammars.Count.ToString());
            Debug.Write("Recognition Engine - Recognizer Information: " + recognitionEngine.RecognizerInfo.Name.ToString());
            messageBus.grammarLoaded = true;
            Announcements.playSound(@"c:\windows\media\Windows Balloon.wav");
        }
    }

    public static Tuple<String, String> dictateSystem(SpeechRecognitionEngine recognitionEngine, List<string> trackedSystems)
    {
        RecognitionResult result = recognitionEngine.Recognize();
        double topPickConfidence = 0.00;
        string topPickSystem = null;
        string topPickPhonetic = null;
        if (result == null)
        {
            Debug.Write("dictate system:  No system matched");
            return Tuple.Create<String, String>(null, null);
        }
        if (result.Alternates != null)
        {
            foreach (RecognizedPhrase phrase in result.Alternates)
            {
                double confidence = (double)phrase.Confidence;
                var grammar = phrase.Grammar.Name;
                var rule = phrase.Grammar.RuleName;
                var semantic = phrase.Semantics.Value;

                if (trackedSystems.Contains(phrase.Text))
                {
                    confidence = confidence + (double)0.05;
                    Debug.Write("Bumping up " + phrase.Text + "by 5% confidence");
                }
                if (confidence > topPickConfidence)
                {
                    topPickConfidence = confidence;
                    topPickSystem = semantic.ToString();
                    topPickPhonetic = phrase.Text.ToString();
                }
                Debug.Write("dictateSystem: Possible system " + semantic + " (" + phrase.Confidence.ToString() + ", " + phrase.Text + ")");
            }
            return Tuple.Create<String, String>(topPickSystem, topPickPhonetic);
        }
        return Tuple.Create<String, String>(null, null);
    }
}

