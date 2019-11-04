using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
	public partial class Mapper
	{
		public static XmlSchemaType MapList<T>(T e) where T : IElement, IRestrictable {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            complexType.Name = e.Name;

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            var min = e.Restrictions.FirstOrDefault(r => r.Key == "min");
            var max = e.Restrictions.FirstOrDefault(r => r.Key == "max");
            sequence.MinOccurs = min is null ? 0 : int.Parse(min.Value);
            sequence.MaxOccurs = max is null ? 10 : int.Parse(max.Value);


            string _type = e.Type.Last().Value;

            XmlSchemaElement element = new XmlSchemaElement();
            if (_type == "String")
            {
                element.SchemaType = Mapper.MapString(e);
            } else if (_type == "Number")
            {
                element.SchemaType = Mapper.MapNumber(e);
            }
            else
            {
                element.RefName = new System.Xml.XmlQualifiedName("self:" + _type);
            }
            sequence.Items.Add(element);
            complexType.Particle = sequence;
            return complexType;
        }
	}
}
