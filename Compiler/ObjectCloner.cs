using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    public static class ObjectCloner
    {
        public static List<T> CloneList<T>(IEnumerable<T> source) where T : ICloneable
        {
            return new List<T>(source.Select(i => i.Clone()).Cast<T>());
        }
    }
}
