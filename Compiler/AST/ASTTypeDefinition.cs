using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeDefinition : IASTNode, ICloneable
    {
        public string Value { get; set; }
        public bool IsGeneric => this.Value.StartsWith("'", StringComparison.Ordinal);
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
            return parser
                .ConsumeWhile(TokenType.Identifier, TokenType.GenericParameter)
                .Select(t => new ASTTypeDefinition(t.Value))
                .ToList();
        }

        public object Clone()
        {
            return new ASTTypeDefinition((string)this.Value.Clone());
        }
    }
}
