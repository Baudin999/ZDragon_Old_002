using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTAlias
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public List<ASTRestriction> Restrictions { get; }

        public ASTAlias(Parser parser)
        {
            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            this.Name = nameId.Value;
            parser.Consume(TokenType.Equal);
            this.Type = parser.Consume(TokenType.Identifier).Value;
            this.Restrictions = ASTRestriction.CreateRestrictions(parser).ToList();
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);
        }

    }
}
