using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class VisitorSource
        : VisitorBase<string>
    {
        List<string> value = new List<string>();
        public VisitorSource(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            // let's nostring call starstring inside of the contructor
        }

        public override string VisitASTAlias(ASTAlias astAlias)
        {
            return "";
        }

        public override string VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            return "";
        }

        public override string VisitASTChoice(ASTChoice astChoice)
        {
            return "";
        }

        public override string VisitASTDirective(ASTDirective astDirective)
        {
            return "";
        }

        public override string VisitASTType(ASTType astType)
        {
            List<string> parts = new List<string>();
            parts.AddRange(astType.Annotations.Select(a => $"@ {a.Value}"));
            parts.AddRange(astType.Directives.Select(a => $"% {a.Key}: {a.Value}"));

            if (astType.Fields.Count() == 0)
            {
                parts.Add($"type {astType.Name}");
            }

            return string.Join("\n", parts.ToArray());
        }

        public override string VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            return "";
        }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            return "";
        }

        public override string VisitDefault(IASTNode node)
        {
            return "";
        }
    }
}
