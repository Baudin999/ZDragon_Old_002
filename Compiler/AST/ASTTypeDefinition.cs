using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Compiler.AST
{
    public class ASTTypeDefinition : IASTNode, ICloneable
    {
        public string Value { get; set; }
        public ASTTypeDefinition() { }
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


        public static IEnumerable<ASTTypeDefinition> Parse(IParser parser)
        {
            parser.TryConsume(TokenType.Identifier, out Token? t);
            while (!(t is null))
            {
                yield return new ASTTypeDefinition(t.Value);
                parser.TryConsume(TokenType.Identifier, out t);
            }
        }

        public object Clone()
        {
            return new ASTTypeDefinition((string)this.Value.Clone());
        }
    }
}
