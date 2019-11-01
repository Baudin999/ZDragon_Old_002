
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Compiler.AST
{
    public class ASTDirective : IASTNode
    {
        public string Key { get; }
        public string Value { get; }
        public ASTDirective(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }


        public static (List<ASTError>, List<ASTDirective>) Parse(IParser parser)
        {
            List<ASTError> errors = new List<ASTError>();
            var directives = parser.ConsumeWhile(TokenType.Directive);

            var result = directives.Select(directive =>
            {
                var result = new Regex(@"\s*%\s*").Replace(directive.Value, "").Split(":");
                if (result.Length != 2)
                {
                    errors.Add(new ASTError($@"Directives should have a key and a value part separated by a semi-colon ':'

This directive looked like:
{directive.Value}

Example:
% api: /some/url/{{param}}
type Person =
    FirstName: String;
"));
                    result = new[] { "no-key", result[0] };
                }
                return new ASTDirective(result[0].Trim(), result[1].Trim());
            }).ToList();

            if (parser.HasNext() && parser.Current.TokenType == TokenType.Directive) parser.Next();
            return (errors, result);
        }
    }
}
