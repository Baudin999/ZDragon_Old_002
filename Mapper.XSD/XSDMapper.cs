using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Compiler;
using Compiler.AST;

namespace Mapper.XSD
{
    public class XSDMapper : VisitorDefault<XmlSchemaObject?>
    {
        private const string DefaultSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        public XmlSchema Schema { get; } = new XmlSchema();

        public XSDMapper(ASTGenerator generator) : base(generator)
        {
            if (generator.ModuleName == string.Empty)
            {
                Schema.Namespaces.Add("self", "org.schema.zdragon");
            }
            else
            {
                Schema.Namespaces.Add("self", "org.schema." + generator.ModuleName.ToLower());
            }
        }

        public override XmlSchemaObject VisitASTType(ASTType astType)
        {
            var t = new XmlSchemaComplexType
            {
                Name = astType.Name
            };

            var fields = astType.Fields.Select(Visit);
            var all = new XmlSchemaAll();
            foreach (var field in fields)
            {
                all.Items.Add(field);
            }

            t.Particle = all;

            var description = string.Join(" ", astType.Annotations.Select(a => a.Value));
            var schemaAnnotation = new XmlSchemaAnnotation();
            var docs = new XmlSchemaDocumentation
            {
                Markup = TextToNodeArray(description)
            };
            schemaAnnotation.Items.Add(docs);
            t.Annotation = schemaAnnotation;

            this.Schema.Items.Add(t);

            ExtractElement(astType);

            return t;
        }

        public override XmlSchemaObject VisitASTAlias(ASTAlias astAlias)
        {
            var _modifier = astAlias.Types.First().Value;
            var _type = astAlias.Types.Last().Value;

            if (_modifier == "List")
            {
                var list = Mapper.MapList(astAlias);
                Schema.Items.Add(list);
                return list;
            }

            var result = _type switch
            {
                "String" => Mapper.MapString(astAlias),
                "Number" => Mapper.MapNumber(astAlias),
                "Decimal" => Mapper.MapNumber(astAlias),
                "Boolean" => Mapper.MapBoolean(astAlias),
                "Date" => Mapper.MapDate(astAlias),
                "Time" => Mapper.MapDate(astAlias),
                "DateTime" => Mapper.MapDate(astAlias),
                _ => Mapper.MapString(astAlias),
            };
            Schema.Items.Add(result);
            ExtractElement(astAlias);

            return result;
        }

        public override XmlSchemaObject VisitASTChoice(ASTChoice astChoice)
        {
            /*
<xs:simpleType name="color" final="restriction" >
    <xs:restriction base="xs:string">
        <xs:enumeration value="green" />
        <xs:enumeration value="red" />
        <xs:enumeration value="blue" />
    </xs:restriction>
</xs:simpleType>
             */
            var enumeration = new XmlSchemaSimpleType
            {
                Name = astChoice.Name
            };
            var restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new XmlQualifiedName("string", DefaultSchemaNamespace);
            foreach (ASTOption option in astChoice.Options)
            {
                var facet = new XmlSchemaEnumerationFacet
                {
                    Value = option.Value
                };
                restriction.Facets.Add(facet);
            }
            enumeration.Content = restriction;
            this.Schema.Items.Add(enumeration);

            return enumeration;
        }

        public override XmlSchemaObject? VisitASTTypeField(ASTTypeField astTypeField)
        {
            return Mapper.Element(astTypeField);
        }

        private void ExtractElement<T>(T node) where T : INamable, IRootNode
        {
            var xsdDirective = node.Directives.FirstOrDefault(d => d.Key == "xsd");
            if (!(xsdDirective is null))
            {
                var element = new XmlSchemaElement();
                element.Name = xsdDirective.Value.Replace(" ", "_");
                element.RefName = new System.Xml.XmlQualifiedName("self:" + (node as INamable).Name);
                Schema.Items.Add(element);
            }
        }
        private XmlNode[] TextToNodeArray(string text)
        {
            var doc = new XmlDocument();
            return new XmlNode[] { doc.CreateTextNode(text) };
        }

        public override XmlSchemaObject? VisitASTData(ASTData astData)
        {
            var xmlSchemaComplexType = new XmlSchemaComplexType();
            xmlSchemaComplexType.Name = astData.Name;

            var xmlSchemaChoice = new XmlSchemaChoice();

            foreach (ASTDataOption option in astData.Options)
            {
                var o = new XmlSchemaElement
                {
                    RefName = new System.Xml.XmlQualifiedName("self:" + option.Name)
                };
                xmlSchemaChoice.Items.Add(o);
            }

            xmlSchemaComplexType.Particle = xmlSchemaChoice;
            this.Schema.Items.Add(xmlSchemaComplexType);
            return xmlSchemaComplexType;
        }


        public override XmlSchemaObject? VisitASTView(ASTView astView) => null;
    }
}

