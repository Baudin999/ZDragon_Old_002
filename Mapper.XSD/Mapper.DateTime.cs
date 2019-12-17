using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
    public partial class Mapper
    {
        public static XmlSchemaType MapDateTime<T>(T e) where T : INamable, IRestrictable
        {
            var simpleType = new XmlSchemaSimpleType();
            simpleType.Name = e.Name;


            var restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new System.Xml.XmlQualifiedName("dateTime", DefaultSchemaNamespace);

            simpleType.Content = restriction;
            return simpleType;
        }
    }
}
