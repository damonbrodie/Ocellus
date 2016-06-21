using System;
using System.Speech.Recognition;
using System.Collections.Generic;
using System.Text.RegularExpressions;


// ******************************************************************
// *  Functions creating and using speech engine for Elite Systems  *
// ******************************************************************
class EliteGrammar
{
    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
    {
        public void Add(T1 item1, T2 item2, T3 item3)
        {
            Add(new Tuple<T1, T2, T3>(item1, item2, item3));
        }
    }

    private static List<string> alternatePhonetics(string system)
    {
        TupleList<string, string, bool> substitutions = new TupleList<string, string, bool>
        {
            { @"(^| )([A-Z])([A-Z][ +-])",   @"$1$2 $3",    false},
            { @" *\+ *",                     @" ",          true},
            { @" *\+ *",                     @" + ",        false},
            { @"([a-z])-",                   @"$1 ",        false},
            { @" *- *",                      @" ",          true},
            { @" *- *",                      @" - ",        false},

            { @"\. ",                        @" ",          false},

            { @"([A-Za-z])([0-9])",          @"$1 $2",      false},
            { @"([0-9])([A-Za-z])",          @"$1 $2",      false},

            { @"^A([a-z] )",                 @"A $1",       true},
            { @"\bD([jxz])",                 @"$1",         true},
            { @"\bHs",                       @"S",          true},
            { @"\bKv",                       @"Kev",        true},
            { @"\bMb",                       @"Emb",        true},
            { @"mii\b",                      @"mee",        true},
            { @"\bN([dgj])",                 @"En$1",       true},
            { @"\bNg",                       @"N",          true},
            { @"\bNj",                       @"Y",          true},
            { @"\bPsa",                      @"Sa",         true},
            { @"\bPt",                       @"Pet",        true},
            { @"\bT([cjpz])",                @"$1",         true},
            { @"\bTl",                       @"Tel",        true},
            { @"\bXpi",                      @"Spee",       true},
            { @"\bZm",                       @"Zem",        true},
        };
        List<string> alternates = new List<string>();
        foreach (Tuple<string, string, bool> rule in substitutions)
        {
            string search = rule.Item1;
            string replace = rule.Item2;
            bool addPhrase = rule.Item3;
            Regex rgx = new Regex(search);
            string result = rgx.Replace(system, replace);
            if (system != result)
            {
                if (addPhrase)
                {
                    alternates.Add(result);
                }
                else
                {
                    system = result;
                }
            }
        }
        alternates.Add(system);
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
        int counter = 0;
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
                    counter += 1;
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
            Debug.Write("Star system recognition engine rules loaded:  " + counter);
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