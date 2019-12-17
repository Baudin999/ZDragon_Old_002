using System;
namespace System
{
    public static class Helpers
    {
        public static T Clone<T>(this T clonable) where T : ICloneable
        {
            return (T)clonable.Clone();
        }
    }
}
