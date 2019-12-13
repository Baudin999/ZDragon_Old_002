using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeDefinition : IASTNode, ICloneable
    {
        public string Value { get; }
        public string Module { get; }
        public bool IsGeneric => this.Value.StartsWith("'", StringComparison.Ordinal);
        public ASTTypeDefinition(string value, string module)
        {
            this.Value = value;
            this.Module = module;
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


        public static IEnumerable<ASTTypeDefinition> Parse(IParser parser, string module)
        {
            return parser
                .ConsumeWhile(TokenType.Identifier, TokenType.GenericParameter)
                .Select(t => new ASTTypeDefinition(t.Value, module))
                .ToList();
        }

        public object Clone()
        {
            return new ASTTypeDefinition(
                (string)this.Value.Clone(),
                (string)this.Module.Clone());
        }

        public override string ToString() => Value;
    }
}
