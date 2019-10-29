using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTAlias : IASTNode
    {
        public string Name { get; private set; }
        public List<ASTTypeDefinition> Type { get; private set; }
        public List<ASTRestriction> Restrictions { get; }

        public ASTAlias(IParser parser)
        {
            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            this.Name = nameId.Value;
            parser.Consume(TokenType.Equal);
            this.Type = ASTTypeDefinition.ParseType(parser).ToList();
            this.Restrictions = ASTRestriction.CreateRestrictions(parser).ToList();
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);
        }

    }
}
