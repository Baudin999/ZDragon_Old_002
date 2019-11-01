using System;
using System.Collections.Generic;
using Compiler.AST;

namespace Compiler
{
    public abstract class VisitorBase
    {
        public IEnumerable<IASTNode> NodeTree { get; }

        public VisitorBase(IEnumerable<IASTNode> nodeTree)
        {
            this.NodeTree = nodeTree;
        }

        public void Start()
        {
            foreach (IASTNode node in NodeTree)
            {
                Visit(node);
            }
        }


        public void Visit(IASTNode node)
        {
            switch (node)
            {
                case ASTType t:
                    VisitASTType(t);
                    break;
                default:
                    this.VisitDefault(node);
                    break;
            }
        }

        public abstract void VisitASTType(ASTType astType);
        public abstract void VisitASTAlias(ASTAlias astAlias);
        public abstract void VisitASTChoice(ASTChoice astChoice);
        public abstract void VisitASTAnnotation(ASTAnnotation astAnnotation);
        public abstract void VisitASTDirective(ASTDirective astDirective);
        public abstract void VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition);
        public abstract void VisitASTTypeField(ASTTypeField astTypeField);

        public abstract void VisitDefault(IASTNode node);
    }
}
