using System;
using System.Collections.Generic;
using Compiler.AST;

namespace Compiler
{
    public abstract class VisitorBase<T>
    {
        public IEnumerable<IASTNode> NodeTree { get; }

        public VisitorBase(IEnumerable<IASTNode> nodeTree)
        {
            this.NodeTree = nodeTree;
        }

        public IEnumerable<T> Start()
        {
            foreach (IASTNode node in NodeTree)
            {
                yield return Visit(node);
            }
        }


        public T Visit(IASTNode node)
        {
            switch (node)
            {
                case ASTType n:
                    return VisitASTType(n);
                case ASTTypeField n:
                    return VisitASTTypeField(n);
                case ASTTypeDefinition n:
                    return VisitASTTypeDefinition(n);
                case ASTRestriction n:
                    return VisitASTRestriction(n);
                case ASTAlias n:
                    return VisitASTAlias(n);
                case ASTAnnotation n:
                    return VisitASTAnnotation(n);
                case ASTDirective n:
                    return VisitASTDirective(n);
                case ASTChoice n:
                    return VisitASTChoice(n);
                case ASTOption n:
                    return VisitASTOption(n);
                default:
                    return VisitDefault(node);
            }
        }

        public abstract T VisitASTType(ASTType astType);
        public abstract T VisitASTAlias(ASTAlias astAlias);
        public abstract T VisitASTChoice(ASTChoice astChoice);
        public abstract T VisitASTAnnotation(ASTAnnotation astAnnotation);
        public abstract T VisitASTDirective(ASTDirective astDirective);
        public abstract T VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition);
        public abstract T VisitASTTypeField(ASTTypeField astTypeField);
        public abstract T VisitASTRestriction(ASTRestriction astRestriction);
        public abstract T VisitASTOption(ASTOption astOption);

        public abstract T VisitDefault(IASTNode node);
    }
}
