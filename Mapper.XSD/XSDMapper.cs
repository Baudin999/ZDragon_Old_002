using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Compiler;
using Compiler.AST;

namespace Mapper.XSD
{
    public class XSDMapper : VisitorBase<XmlSchemaObject?>
    {
        private const string DefaultSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        public XmlSchema Schema { get; } = new XmlSchema();

        public XSDMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            Schema.Namespaces.Add("self", "org.schema.something");
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
            var docs = new XmlSchemaDocumentation()
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
            var _modifier = astAlias.Type.First().Value;
            var _type = astAlias.Type.Last().Value;
            
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

        public override XmlSchemaObject? VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTDirective(ASTDirective astDirective)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTTypeField(ASTTypeField astTypeField)
        {
            return Mapper.Element(astTypeField);
        }

        public override XmlSchemaObject? VisitASTRestriction(ASTRestriction astRestriction)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTOption(ASTOption astOption)
        {
            return null;
        }

        public override XmlSchemaObject? VisitDefault(IASTNode node)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTChapter(ASTChapter astOption)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTParagraph(ASTParagraph astOption)
        {
            return null;
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
            return new XmlNode[1] { doc.CreateTextNode(text) };
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

        public override XmlSchemaObject? VisitASTFlow(ASTFlow astFlow)
        {
            return null;
        }

        public override XmlSchemaObject? VisitASTView(ASTView astView) => null;
    }
}

