using System;
using System.Collections.Generic;
using Compiler.AST;

namespace Compiler
{
    public class VisitorDefault<T> : VisitorBase<T>
    {
        public VisitorDefault(ASTGenerator generator) : base(generator) { }

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
        public override T VisitASTView(ASTView astView) => d;
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
