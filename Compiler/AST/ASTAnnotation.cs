
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Compiler.AST
{
    public class ASTDirective : IASTNode
    {
        public string Value { get; }
        public ASTDirective(string value)
        {
            this.Value = value;
        }


        public static IEnumerable<ASTDirective> Parse(Parser parser)
        {
            var directives = parser.ConsumeWhile(TokenType.Directive).ToList();

            var result = directives.Select(directive =>
            {
                var result = new Regex(@"\s*%\s*").Replace(directive.Value, "");
                return new ASTDirective(result);
            });

            if (parser.HasNext() && parser.Current.TokenType == TokenType.Directive) parser.Next();
            return result;
        }
    }
}
