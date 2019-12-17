using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
	public partial class Mapper
	{
		public static XmlSchemaElement Element<T>(T e) where T : IElement, INamable, IRestrictable {
            var _modifier = e.Types.First().Value;
            var _type = e.Types.Last().Value;

            var element = new XmlSchemaElement();
            element.Name = e.Name;
            element.IsNillable = _modifier == "Maybe";
            element.MinOccurs = _modifier == "Maybe" ? 0 : 1;
            element.MaxOccurs = 1;

            if (_modifier == "List")
            {
                element.SchemaType = Mapper.MapList(e);
            }
            else if (_type == "String")
            {
                XmlSchemaType simpleType = Mapper.MapString(e);
                element.SchemaType = simpleType;
            }
            else if (_type == "Number")
            {
                XmlSchemaType simpleType = Mapper.MapNumber(e);
                element.SchemaType = simpleType;
            }
            else if (_type == "Boolean")
            {
                XmlSchemaType simpleType = Mapper.MapBoolean(e);
                element.SchemaType = simpleType;
            }
            else if (_type == "DateTime")
            {
                XmlSchemaType simpleType = Mapper.MapDateTime(e);
                element.SchemaType = simpleType;
            }
            else if (_type == "Date")
            {
                XmlSchemaType simpleType = Mapper.MapDate(e);
                element.SchemaType = simpleType;
            }
            else if (_type == "Time")
            {
                XmlSchemaType simpleType = Mapper.MapTime(e);
                element.SchemaType = simpleType;
            }
            else
            {
                element.RefName = new System.Xml.XmlQualifiedName("self:" + _type);
            }
            if (!(element.SchemaType is null))
            {
                element.SchemaType.Name = null;
            }
            return element;
        }
	}
}
