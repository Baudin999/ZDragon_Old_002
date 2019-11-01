using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class VisitorSource
        : VisitorBase
    {
        List<string> value = new List<string>();
        public VisitorSource(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            // let's not call start inside of the contructor
        }

        public override void VisitASTAlias(ASTAlias astAlias)
        {
            
        }

        public override void VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            
        }

        public override void VisitASTChoice(ASTChoice astChoice)
        {
            
        }

        public override void VisitASTDirective(ASTDirective astDirective)
        {
            
        }

        public override void VisitASTType(ASTType astType)
        {
            List<string> parts = new List<string>();
            parts.AddRange(astType.Annotations.Select(a => $"@ {a.Value}"));
            parts.AddRange(astType.Directives.Select(a => $"% {a.Value}"));
        }

        public override void VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            
        }

        public override void VisitASTTypeField(ASTTypeField astTypeField)
        {
            
        }

        public override void VisitDefault(IASTNode node)
        {
            
        }
    }
}
