using System;
using System.IO;
using System.IO.Compression;
using System.Speech.Recognition;
using System.Threading.Tasks;


class EliteGrammar
{
    private const string grammarURL = "http://ocellus.io/data/SystemsGrammar.xml";


    public static bool downloadGrammar()
    {
        string path = Config.Path();
        string zipFile = Path.Combine(path, "SystemsGrammar.zip");
        string grammarFile = Path.Combine(Config.Path(), "SystemsGrammar.xml");

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
            ZipFile.ExtractToDirectory(zipFile, path);
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

