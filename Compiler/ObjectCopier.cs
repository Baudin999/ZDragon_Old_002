using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    public static class ObjectCopier
    {
        public static List<T> CopyList<T>(List<T> source) where T : ICloneable
        {
            return new List<T>(source.Select(i => i.Clone()).Cast<T>());
        }
    }
}
