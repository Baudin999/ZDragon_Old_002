using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Compiler;
using Compiler.AST;

namespace Mapper.XSD
{
    public class XSDMapper : VisitorBase<XmlSchemaObject>
    {


        public XmlSchema Schema { get; } = new XmlSchema();

        public XSDMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            Schema.Namespaces.Add("self", "org.schema.something");
        }

        public override XmlSchemaObject VisitASTType(ASTType astType)
        {
            XmlSchemaComplexType t = new XmlSchemaComplexType
            {
                Name = astType.Name
            };

            var fields = astType.Fields.Select(Visit);
            XmlSchemaAll all = new XmlSchemaAll();
            foreach (var field in fields)
            {
                all.Items.Add(field);
            }

            t.Particle = all;
            this.Schema.Items.Add(t);
            return t;
        }

        public override XmlSchemaObject VisitASTAlias(ASTAlias astAlias)
        {
            string _modifier = astAlias.Type.First().Value;
            string _type = astAlias.Type.Last().Value;
            
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
                _ => Mapper.MapString(astAlias),
            };
            Schema.Items.Add(result);

            return result;
        }

        public override XmlSchemaObject VisitASTChoice(ASTChoice astChoice)
        {
            throw new NotImplementedException();
        }

        public override XmlSchemaObject VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            throw new NotImplementedException();
        }

        public override XmlSchemaObject VisitASTDirective(ASTDirective astDirective)
        {
            throw new NotImplementedException();
        }

        public override XmlSchemaObject VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            throw new NotImplementedException();
        }

        public override XmlSchemaObject VisitASTTypeField(ASTTypeField astTypeField)
        {
            return Mapper.Element(astTypeField);
        }

        public override XmlSchemaObject VisitASTRestriction(ASTRestriction astRestriction)
        {
            throw new NotImplementedException();
        }

        public override XmlSchemaObject VisitASTOption(ASTOption astOption)
        {
            throw new NotImplementedException();
        }

        public override XmlSchemaObject VisitDefault(IASTNode node)
        {
            throw new NotImplementedException();
        }

    }
}
