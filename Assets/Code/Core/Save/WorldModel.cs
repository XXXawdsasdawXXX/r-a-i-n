using System;
using System.Collections.Generic;

namespace Core.Save
{
    [Serializable]
    public class WorldModel
    {
        public Dictionary<int, int> Resources;

        public WorldModel()
        { 
            Resources = new Dictionary<int, int>();
        }
    }
}