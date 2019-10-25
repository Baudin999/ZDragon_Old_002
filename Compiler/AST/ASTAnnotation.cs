using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTAnnotation
    {
        public string Value { get; }
        public ASTAnnotation(string value)
        {
            this.Value = value;
        }


        public static IEnumerable<ASTAnnotation> Parse(Parser parser)
        {
            parser.TryConsume(TokenType.NewLine);

            while (parser.Current.TokenType == TokenType.Indent && parser.Peek().TokenType == TokenType.Annotation)
            {
                parser.TryConsume(TokenType.Indent);
                parser.TryConsume(TokenType.Annotation, out Token t);
                if (t != null)
                {
                    yield return new ASTAnnotation(t.Value);
                }
            }
        }
    }
}
