using System;
using System.Speech.Recognition;
using System.Collections.Generic;


// ******************************************************************
// *  Functions creating and using speech engine for Elite Systems  *
// ******************************************************************
class EliteGrammar
{
    private static List<string> alternatePhonetics(string system)
    {
        List<string> alternates = new List<string>();
        alternates.Add(system);  // Add the plain system name by default
        if (system == "Chun Hsi")
        {
            alternates.Add("Chun Si");
        }
        return alternates;
    }

    public static void loadGrammar(Elite.MessageBus messageBus)
    {
        SpeechRecognitionEngine recognitionEngine = messageBus.recognitionEngine;
        recognitionEngine.SetInputToDefaultAudioDevice();
        recognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
        recognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1.5);
        recognitionEngine.BabbleTimeout = TimeSpan.FromSeconds(5);

        DateTime startTime = DateTime.Now;
        Debug.Write("Begin loading star system grammar");
        bool grammarLoaded = true;
        try
        {
            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = messageBus.recognitionEngineLang;
            Choices systemChoice = new Choices();
            foreach (string system in messageBus.systemIndex["systems"].Keys)
            {
                List<string> alternates = alternatePhonetics(system);
                foreach (string alternate in alternates)
                {
                    GrammarBuilder systemBuilder = new GrammarBuilder(alternate);
                    systemBuilder.Culture = messageBus.recognitionEngineLang;
                    SemanticResultValue systemSemantic = new SemanticResultValue(systemBuilder, system);
                    systemChoice.Add(new GrammarBuilder(systemSemantic));
                }
            }
            gb.Append(systemChoice);
            Grammar grammar = new Grammar(gb);
            grammar.Name = "populated";
            recognitionEngine.LoadGrammar(grammar);
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
            grammarLoaded = false;
            Debug.Write("Error:  Unable to load grammar");
            Announcements.errorAnnouncement(messageBus, "Unable to load star system recognition engine");
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

            Announcements.engineAnnouncement(messageBus);
        }
        messageBus.grammarLoaded = grammarLoaded;
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
                Debug.Write("Semantic " + phrase.Semantics.ToString());

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

