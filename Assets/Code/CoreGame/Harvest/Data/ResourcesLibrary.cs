using System;
using Core.Libraries;

namespace Code.CoreGame.Harvest
{
    [Serializable]
    public class ResourcesLibrary : Library<EResource, ResourceConfig>
    {
        protected override bool ThisIs(ResourceConfig value, EResource key)
        {
            return value.Type == key;
        }
    }
}