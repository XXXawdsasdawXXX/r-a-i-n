using System;
using TriInspector;

namespace Essential
{
    [Serializable]
    public struct UniqueID
    {
        [ReadOnly, ShowInInspector] public string Value { get; private set; }
        
        [Button]
        public void Validate()
        {
            if (string.IsNullOrEmpty(Value))
            {
                Value = Guid.NewGuid().ToString();
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Value);
        }
    }
}