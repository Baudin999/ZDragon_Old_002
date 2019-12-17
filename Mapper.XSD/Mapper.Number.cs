﻿using System;
using System.Linq;
using System.Xml.Schema;
using Compiler.AST;

namespace Mapper.XSD
{
	public partial class Mapper
	{
		public static XmlSchemaType MapNumber<T>(T e) where T : IElement, INamable, IRestrictable { 
            var simpleType = new XmlSchemaSimpleType();
            simpleType.Name = e.Name;


            var restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new System.Xml.XmlQualifiedName("decimal", DefaultSchemaNamespace);

            var min = e.Restrictions.FirstOrDefault(r => r.Key == "min");
            var max = e.Restrictions.FirstOrDefault(r => r.Key == "max");

            if (min != null)
            {
                var mMin = new XmlSchemaMinLengthFacet();
                mMin.Value = min is null ? "1" : min.Value;
                restriction.Facets.Add(mMin);
            }

            if (max != null)
            {
                var mMax = new XmlSchemaMaxLengthFacet();
                mMax.Value = max is null ? "100" : max.Value;
                restriction.Facets.Add(mMax);
            }

            simpleType.Content = restriction;
            return simpleType;
        }
	}
}
