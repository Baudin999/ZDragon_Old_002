using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Compiler.AST
{
    public class ASTTypeDefinition : IASTNode
    {
        public string Value { get; }
        public ASTTypeDefinition(string value)
        {
            this.Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ASTTypeDefinition)
            {
                return ((ASTTypeDefinition)obj).Value == this.Value;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }


        public static IEnumerable<ASTTypeDefinition> ParseType(IParser parser)
        {
            parser.TryConsume(TokenType.Identifier, out Token? t);
            while (!(t is null))
            {
                yield return new ASTTypeDefinition(t.Value);
                parser.TryConsume(TokenType.Identifier, out t);
            }
        }
    }
}
