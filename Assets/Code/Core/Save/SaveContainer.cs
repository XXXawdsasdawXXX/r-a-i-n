using System;
using System.Collections.Generic;

namespace Core.Save
{
    [Serializable]
    public class SaveContainer
    {
        public SettingsModel UserSettings = new();
        public string LastSlot;
        public Dictionary<string, string> Slots = new();
    }
}