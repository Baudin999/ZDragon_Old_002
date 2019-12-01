using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTImport : IASTNode, INamable
    {
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<string> Imports { get; }

        public ASTImport(
            string name,
            string module,
            IEnumerable<string> imports) {
            this.Name = name;
            this.Module = module;
            this.Imports = imports;
        }

        public static (IEnumerable<IASTError>, ASTImport) Parse(IParser parser, string module = "")
        {
            var errors = new List<ASTError>();
            var imports = new List<string>();
            
            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            var name = nameId.Value;

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

            var result = new ASTImport(
                name,
                module,
                imports);

            return (errors, result);
        }
    }
}
