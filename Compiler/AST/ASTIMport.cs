using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTImport : IASTNode, INamable
    {
        public string Name { get; set; } = "";
        public IEnumerable<string> Imports { get; set; } = Enumerable.Empty<string>();
        public ASTImport() { }
        public static (List<ASTError>, ASTImport) Parse(IParser parser)
        {
            List<ASTError> errors = new List<ASTError>();
            ASTImport result = new ASTImport();
            List<string> imports = new List<string>();
            
            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            result.Name = nameId.Value;

            parser.TryConsume(TokenType.ContextStarted);
            var importing = parser.TryConsume(TokenType.KW_Importing);
            if (!(importing is null))
            {
                parser.Consume(TokenType.GroupOpen);

                while(parser.Current.TokenType != TokenType.GroupClosed)
                {
                    var t = parser.Consume(TokenType.Identifier);
                    imports.Add(t.Value);
                    parser.TryConsume(TokenType.ListSeparator);
                }

                parser.Consume(TokenType.GroupClosed);
            }
            parser.TryConsume(TokenType.ContextStarted);
            result.Imports = imports;

            return (errors, result);
        }
    }
}
