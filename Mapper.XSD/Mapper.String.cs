using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
	public partial class Mapper
	{
		public static XmlSchemaType MapString<T>(T e) where T : IElement, IRestrictable { 
            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
            simpleType.Name = e.Name;


            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new System.Xml.XmlQualifiedName("string", DefaultSchemaNamespace);

            var min = e.Restrictions.FirstOrDefault(r => r.Key == "min");
            var max = e.Restrictions.FirstOrDefault(r => r.Key == "max");
            var pattern = e.Restrictions.FirstOrDefault(r => r.Key == "pattern");

            XmlSchemaMinLengthFacet mMin = new XmlSchemaMinLengthFacet();
            mMin.Value = min is null ? "1" : min.Value;
            restriction.Facets.Add(mMin);

            XmlSchemaMaxLengthFacet mMax = new XmlSchemaMaxLengthFacet();
            mMax.Value = max is null ? "100" : max.Value;
            restriction.Facets.Add(mMax);

            if (pattern != null)
            {
                XmlSchemaMaxLengthFacet? mPattern = new XmlSchemaMaxLengthFacet();
                mPattern.Value = pattern is null ? "" : mPattern.Value;
                restriction.Facets.Add(mPattern);
            }


            simpleType.Content = restriction;
            return simpleType;
        }
	}
}
