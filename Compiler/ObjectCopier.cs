using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text.Json;

    public static class ObjectCopier
    {
        public static T Clone<T>(T source) where T : new()
        {
            if (Object.ReferenceEquals(source, null))
            {
                return new T();
            }

            var s = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<T>(s);
        }

        public static List<T> CopyList<T>(List<T> source) where T : ICloneable
        {
            return new List<T>(source.Select(i => i.Clone()).Cast<T>());
        }
    }
}
