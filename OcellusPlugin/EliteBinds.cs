using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


// ********************************
// *  Read the Elite Keybindings  *
// ********************************
namespace OcellusPlugin
{
    internal partial class EliteBinds
    {
        private readonly Dictionary<string, List<string>> _bindList;

        public EliteBinds()
        {
            // TODO: move hard-coded file names to defines or similar
            var bindsFile = Elite.getBindsFilename();

            // TODO: do something if file not found
            var bindsTree = XElement.Load(bindsFile);
            _bindList = new Dictionary<string, List<string>>();
            foreach (var element in bindsTree.Elements())
            {
                var keys = ParseBindControlNode(element);
                if (keys == null) continue;
                _bindList.Add(element.Name.LocalName, keys);
            }

            // TODO: look at version in file and balk if unknown
        }

        public List<uint> GetCodes(string command)
        {
            var keys = _bindList.ContainsKey(command) ? _bindList[command] : null;
            // TODO: throw exceptions rather than returning null
            // TODO: die if key doesn't have key code
            return keys == null ? null : (from key in keys where _keyMap.ContainsKey(key) select _keyMap[key]).ToList();
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