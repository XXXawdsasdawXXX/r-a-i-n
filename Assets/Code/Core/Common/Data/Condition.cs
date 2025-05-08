using System;
using System.Collections.Generic;

namespace Core.Data
{
    public sealed class Condition
    {
        private readonly List<Func<bool>> _conditions = new();

        public void Add(Func<bool> condition)
        {
            _conditions.Add(condition);
        }

        public bool AreMet()
        {
            if (_conditions.Count == 0)
            {
                return true;
            }

            foreach (Func<bool> condition in _conditions)
            {
                if (!condition.Invoke())
                {
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {
            _conditions.Clear();
        }
    }
}