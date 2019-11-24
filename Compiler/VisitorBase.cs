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
                ASTFlow n => VisitASTFlow(n),
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

        // flows
        public abstract T VisitASTFlow(ASTFlow astFlow);

        // markdown functions
        public abstract T VisitASTChapter(ASTChapter astChapter);
        public abstract T VisitASTParagraph(ASTParagraph astParagraph);

        // default
        public abstract T VisitDefault(IASTNode node);
    }


    public class DefaultVisitor<T> : VisitorBase<T>
    {

        public DefaultVisitor(IEnumerable<IASTNode> nodeTree) : base(nodeTree) { }

        public override T VisitASTAlias(ASTAlias astAlias) => d;
        public override T VisitASTAnnotation(ASTAnnotation astAnnotation) => d;
        public override T VisitASTChapter(ASTChapter astChapter) => d;
        public override T VisitASTChoice(ASTChoice astChoice) => d;
        public override T VisitASTData(ASTData astData) => d;
        public override T VisitASTDirective(ASTDirective astDirective) => d;
        public override T VisitASTFlow(ASTFlow astFlow) => d;
        public override T VisitASTOption(ASTOption astOption) => d;
        public override T VisitASTParagraph(ASTParagraph astParagraph) => d;
        public override T VisitASTRestriction(ASTRestriction astRestriction) => d;
        public override T VisitASTType(ASTType astType) => d;
        public override T VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition) => d;
        public override T VisitASTTypeField(ASTTypeField astTypeField) => d;
        public override T VisitDefault(IASTNode node) => d;

        private T d
        {
            get
            {
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                T t = default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.

                return Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.String => (T)Convert.ChangeType(string.Empty, typeof(T)),
                    TypeCode.Int16 => (T)Convert.ChangeType(0, typeof(T)),
                    _ => t
                };
            }
        }
    }

}
