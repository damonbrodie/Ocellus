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

    private static bool loadGrammar()
    {
        SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();
        recognitionEngine.SetInputToDefaultAudioDevice();
        Grammar grammar = new Grammar(Path.Combine(Config.Path(), "systems_grammar.xml"));
        recognitionEngine.LoadGrammar(grammar);
        return true;
    }

    public async Task<bool> initialize()
    {
        return await Task.Run(() => loadGrammar());
    }

    public static Tuple<String, String> dictateSystem(SpeechRecognitionEngine recognitionEngine, List<string> trackedSystems)
    {
        recognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
        recognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1.5);
        recognitionEngine.BabbleTimeout = TimeSpan.FromSeconds(5);
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

