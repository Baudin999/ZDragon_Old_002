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
            return node switch
            {
                ASTType n => VisitASTType(n),
                ASTTypeField n => VisitASTTypeField(n),
                ASTTypeDefinition n => VisitASTTypeDefinition(n),
                ASTRestriction n => VisitASTRestriction(n),
                ASTAlias n => VisitASTAlias(n),
                ASTData n => VisitASTData(n),
                ASTAnnotation n => VisitASTAnnotation(n),
                ASTDirective n => VisitASTDirective(n),
                ASTChoice n => VisitASTChoice(n),
                ASTOption n => VisitASTOption(n),
                ASTChapter n => VisitASTChapter(n),
                ASTParagraph n => VisitASTParagraph(n),
                _ => VisitDefault(node),
            };
        }

        // language
        public abstract T VisitASTType(ASTType astType);
        public abstract T VisitASTAlias(ASTAlias astAlias);
        public abstract T VisitASTData(ASTData astData);
        public abstract T VisitASTChoice(ASTChoice astChoice);
        public abstract T VisitASTAnnotation(ASTAnnotation astAnnotation);
        public abstract T VisitASTDirective(ASTDirective astDirective);
        public abstract T VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition);
        public abstract T VisitASTTypeField(ASTTypeField astTypeField);
        public abstract T VisitASTRestriction(ASTRestriction astRestriction);
        public abstract T VisitASTOption(ASTOption astOption);

        // markdown functions
        public abstract T VisitASTChapter(ASTChapter astChapter);
        public abstract T VisitASTParagraph(ASTParagraph astParagraph);

        // default
        public abstract T VisitDefault(IASTNode node);
    }
}
