using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.IO;


// ********************************
// *  Read the Elite Keybindings  *
// ********************************
namespace OcellusPlugin
{
    internal partial class EliteBinds
    {
        private readonly Dictionary<string, List<string>> _bindList;

        private static Boolean needsBindsReload(ref Dictionary<string, object> state, ref Dictionary<string, string> textValues, ref Dictionary<string, bool?> booleanValues)
        {
            if (!state.ContainsKey("VAEDbindsFile") || !state.ContainsKey("VAEDbindsPreset") || !state.ContainsKey("VAEDlastPresetTimestamp") || !state.ContainsKey("VAEDlastBindsTimestamp"))
            {
                return true;
            }

            DateTime lastPresetTimestamp = (DateTime)state["VAEDlastPresetTimestamp"];
            DateTime lastBindsTimestamp = (DateTime)state["VAEDlastBindsTimestamp"];
            string bindsPreset = (string)state["VAEDbindsPreset"];
            string bindsFile = (string)state["VAEDbindsFile"];

            DateTime currentBindsTimestamp = File.GetLastWriteTime(bindsFile);
            DateTime currentPresetTimestamp = File.GetLastWriteTime(bindsPreset);

            if (lastPresetTimestamp != currentPresetTimestamp || lastBindsTimestamp != currentBindsTimestamp)
            {
                return true;
            }
           return false;
        }

        public static EliteBinds getBinds(ref Dictionary<string, object> state, ref Dictionary<string, string> textValues, ref Dictionary<string, bool?> booleanValues, Elite.MessageBus messageBus)
        {
            EliteBinds eliteBinds = null;

            if (needsBindsReload(ref state, ref textValues, ref booleanValues) == true)
            {
                Tuple<string, string> tResponse = Elite.getBindsFilename();
                string bindsPreset = tResponse.Item1;
                string bindsFile = tResponse.Item2;

                if (bindsPreset != null && bindsFile != null)
                {
                    Debug.Write("Current Binds file: " + bindsFile);
                    var bindsTree = XElement.Load(bindsFile);
                    state["VAEDbindsFile"] = bindsFile;
                    state["VAEDbindsPreset"] = bindsPreset;
                    state["VAEDlastPresetTimestamp"] = File.GetLastWriteTime(bindsPreset);
                    state["VAEDlastBindsTimestamp"] = File.GetLastWriteTime(bindsFile);
                    XElement keyboardLayout = bindsTree.Element("KeyboardLayout");
                    string lang = keyboardLayout.Value.ToString();
                    Debug.Write("Elite key bindings language set to: " + lang);
                    state["VAEDbindsLanguage"] = lang;

                    eliteBinds = new EliteBinds(bindsTree, lang);
                    state["VAEDeliteBinds"] = eliteBinds;
                }
            }
            else
            {
                eliteBinds = (EliteBinds)state["VAEDeliteBinds"];
            }
            return eliteBinds;
        }

        public EliteBinds(XElement bindsTree, string lang)
        {

            _bindList = new Dictionary<string, List<string>>();

            foreach (var element in bindsTree.Elements())
            {
                var keys = ParseBindControlNode(element);
                if (keys == null) continue;
                _bindList.Add(element.Name.LocalName, keys);
            }
            _bindList.Add("HUD", new List < string >{ "LeftControl", "LeftAlt", "G" } );
            _bindList.Add("FrameRate", new List<string> { "LeftControl", "F" });
            _bindList.Add("ConnectionStatus", new List<string> { "LeftControl", "B" });
            _bindList.Add("Snapshot", new List<string> { "F10" });
            _bindList.Add("HighResSnapshot", new List<string> { "LeftAlt", "F10" });
            _bindList.Add("CloseQuickComms", new List<string> { "Esc" });
            // TODO: look at version in file and balk if unknown
        }

        public List<uint> GetCodes(string command, string keyboardLanguage)
        {
            var keys = _bindList.ContainsKey(command) ? _bindList[command] : null;
            // TODO: throw exceptions rather than returning null
            // TODO: die if key doesn't have key code
            string lookup = "en-US";
            if (langMap.ContainsKey(keyboardLanguage))
            {
                lookup = langMap[keyboardLanguage];
            }
            else
            {
                lookup = keyboardLanguage;
            }

            Debug.Write("Lookup Lang " + lookup);
            // Attempt to find the language specific keymap
            List<uint> codes = keys == null ? null : (from key in keys where keyMap.ContainsKey(lookup + ":" + key) select keyMap[lookup + ":" + key]).ToList();
            if (codes.Count != 0)
            {
                Debug.Write("Using the lookup lang");
                return codes;
            }
            Debug.Write("Falling back to en-US");
            // If not found then default to en-US
            return keys == null ? null : (from key in keys where keyMap.ContainsKey("en-US:" + key) select keyMap["en-US:" + key]).ToList();
        }

        private static List<string> ParseBindControlNode(XContainer bindControl)
        {
            return ParseBindNode(bindControl.Element("Primary")) ?? ParseBindNode(bindControl.Element("Secondary"));
        }

        private static List<string> ParseBindNode(XElement bindNode)
        {
            var keys = new List<string>();
            if (bindNode == null) return null;
            if (bindNode.Attribute("Device").Value != "Keyboard") return null;

            // Modifiers (shift, ctrl, alt) must be pressed before main key
            // Remove "Key_" prefix
            keys.AddRange(from element in bindNode.Elements()
                          where element.Attribute("Device").Value == "Keyboard"
                          select element.Attribute("Key").Value.Remove(0, 4));
            keys.Add(bindNode.Attribute("Key").Value.Remove(0, 4));
            return keys;
        }
    }
}