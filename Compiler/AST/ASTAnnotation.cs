using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Compiler.AST
{
    public class ASTAnnotation : IASTNode, ICloneable
    {

        public string Value { get; private set; } = "";
        public string Module { get; } = "";

        public ASTAnnotation(string value)
        {
            this.Value = value;
        }

        public static IEnumerable<ASTAnnotation> Parse(IParser parser)
        {
            var annotations = parser.ConsumeWhile(TokenType.Annotation).ToList();

            return annotations.Select(annotation =>
            {
                var result = new Regex(@"\s*@\s*").Replace(annotation.Value, "");
                return new ASTAnnotation(result.Trim());
            });

            //if (parser.HasNext() && parser.Current.TokenType == TokenType.Annotation) parser.Next();
            //return result;
        }

        public object Clone()
        {
            return new ASTAnnotation((string)this.Value.Clone());
        }
    }
}
