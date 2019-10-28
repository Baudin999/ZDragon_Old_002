using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Compiler.AST
{
    public class ASTAnnotation : IASTNode
    {
        public string Value { get; }
        public ASTAnnotation(string value)
        {
            this.Value = value;
        }


        public static IEnumerable<ASTAnnotation> Parse(Parser parser)
        {
            var annotations = parser.ConsumeWhile(TokenType.Annotation).ToList();

            var result = annotations.Select(annotation =>
            {
                var result = new Regex(@"\s*@\s*").Replace(annotation.Value, "");
                return new ASTAnnotation(result);
            });

            if (parser.HasNext() && parser.Current.TokenType == TokenType.Annotation) parser.Next();
            return result;
        }
    }
}
