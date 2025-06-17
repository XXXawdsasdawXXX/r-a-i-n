using System;
using System.Collections.Generic;

namespace Core.Save
{
    [Serializable]
    public class SettingsModel
    {
        public float SFXVolume;
       
        public List<string> PreviousConnectedIPs;
        public int LastConnectedIPIndex;

        public SettingsModel()
        {
            SFXVolume = 1;
            PreviousConnectedIPs = new List<string>();
            LastConnectedIPIndex = 0;
        }
    }
}