using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
    public partial class Mapper
    {
        public static XmlSchemaType MapBoolean<T>(T e) where T : IElement, IRestrictable
        {
            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
            simpleType.Name = e.Name;


            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new System.Xml.XmlQualifiedName("boolean", DefaultSchemaNamespace);

            simpleType.Content = restriction;
            return simpleType;
        }
    }
}
