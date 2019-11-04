using System;
namespace Mapper.XSD
{
    public partial class Mapper
    {
        private const string DefaultSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        private static string ConvertToQualifiedName(string value)
        {
            return value switch
            {
                "String" => "string",
                "Number" => "decimal",
                _ => "string"
            };
        }

    }
}
