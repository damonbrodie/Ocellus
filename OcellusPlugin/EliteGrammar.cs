using System;
using System.IO;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Threading.Tasks;


class EliteGrammar
{
    private const string grammarURL = "http://ocellus.io/data/SystemsGrammar.xml";

    public static bool downloadGrammar()
    {
        string path = Config.Path();
        string grammarFile = Path.Combine(Config.Path(), "SystemsGrammar.xml");

        if (File.Exists(grammarFile))
        {
            // Download the grammar once a week
            DateTime fileTime = File.GetLastWriteTime(grammarFile);
            DateTime weekago = DateTime.Now.AddDays(-7);
            if (fileTime > weekago)
            {
                return true;
            }
        }


        if (Web.downloadFile(grammarURL, grammarFile))
        {
            Debug.Write("Downloaded Grammar from Ocellus.io");
            return true;
        }

        if (!File.Exists(grammarFile))
        {
            Debug.Write("ERROR:  Unable to download Grammar from Ocellus.io");
            return false;
        }
        return true;
    }

    private static bool loadGrammar()
    {
        SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();
        recognitionEngine.SetInputToDefaultAudioDevice();
        Grammar grammar = new Grammar(Path.Combine(Config.Path(), "SystemsGrammar.xml"));
        recognitionEngine.LoadGrammar(grammar);
        return true;
    }

    public async Task<bool> initialize()
    {
        return await Task.Run(() => loadGrammar());
    }

    public static string dictateSystem(SpeechRecognitionEngine recognitionEngine)
    {
        recognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
        recognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1);
        RecognitionResult result = recognitionEngine.Recognize(TimeSpan.FromSeconds(5));
        
        if (result != null)
        {
            return result.Semantics.Value.ToString();
        }
        return null;
    }
}

