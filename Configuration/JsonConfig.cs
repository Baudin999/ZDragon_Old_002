using System;
namespace Configuration
{
    public class JsonConfig
    {
        public Casing Casing { get; set; } = Casing.SnakeCase;
    }

    public enum Casing
    {
        SnakeCase,
        CamelCase,
        PascalCase
    }
}
