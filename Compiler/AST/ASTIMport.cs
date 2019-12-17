using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTImport : IASTNode, INamable, IElement
    {
        public Token? Token { get; } = Token.Empty();
        public string ModuleName => ASTName.Name;
        public string Module { get; }
        public IEnumerable<string> Imports { get; }

        public ASTName ASTName { get; }
        public IEnumerable<ASTTypeDefinition> Types => Enumerable.Empty<ASTTypeDefinition>();
        public string Name => this.ASTName.Name;

        public ASTImport(
            ASTName name,
            string module,
            IEnumerable<string> imports)
        {
            this.ASTName = name;
            this.Module = module;
            this.Imports = imports;
        }

        public static (IEnumerable<IASTError>, ASTImport) Parse(IParser parser, string module = "")
        {
            var startColumn = parser.Current.StartColumn;
            var startLine = parser.Current.StartLine;
            var errors = new List<ASTError>();
            var imports = new List<string>();
            
            if (parser.HasNext()) parser.Next();
            var names = parser.ConsumeWhile(TokenType.Identifier).ToList();//.Select(name => name.Value);
            var name = string.Join(".", names.Select(name => name.Value).ToList());
            var astName = new ASTName(name, new Token
            {
                StartColumn = startColumn,
                StartLine = startLine,
                EndColumn = names.Last().EndColumn,
                EndLine = names.Last().EndLine
            });

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
                astName,
                module,
                imports);

            return (errors, result);
        }
    }
}
