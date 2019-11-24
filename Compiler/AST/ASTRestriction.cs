using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTRestriction : IASTNode, ICloneable
    {
        public string Key { get;  }
        public string Value { get; private set; }
        public int Depth { get; }
        public IEnumerable<ASTAnnotation> Annotations { get;  }
        public Token Token { get; }

        public ASTRestriction(string key, string value, IEnumerable<ASTAnnotation> annotations, Token token, int depth)
        {
            this.Key = key;
            this.Value = value;
            this.Annotations = annotations;
            this.Token = token;
            this.Depth = depth;
        }

        public ASTRestriction ChangeValue(string value)
        {
            this.Value = value;
            return this;
        }

        public static IEnumerable<ASTRestriction> CreateRestrictions(IParser parser, TokenType root)
        {
            var depth = root == TokenType.KW_Alias ? 1 : 2;
            var annotations = ASTAnnotation.Parse(parser);
            parser.TryConsume(TokenType.And, out Token? t);
            while (!(t is null))
            {
                var word = parser.Consume(TokenType.Word);
                var key = word.Value.Trim();
                var numberValue = parser.TryConsume(TokenType.Number);
                var stringValue = parser.TryConsume(TokenType.String);
                var patternValue = parser.TryConsume(TokenType.Pattern);
                if (numberValue != null)
                {
                    yield return new ASTRestriction(key, numberValue.Value, annotations, numberValue, depth);
                }
                else if (stringValue != null)
                {
                    yield return new ASTRestriction(key, stringValue.Value, annotations, stringValue, depth);
                }
                else if (patternValue != null)
                {
                    yield return new ASTRestriction(key, patternValue.Value, annotations, patternValue, depth);
                }

                annotations = ASTAnnotation.Parse(parser);
                parser.TryConsume(TokenType.And, out t);
            }
        }

        public object Clone()
        {
            return new ASTRestriction(
                (string)this.Key.Clone(),
                (string)this.Value.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()) ,
                (Compiler.Token)this.Token.Clone(),
                this.Depth);
        }
    }
}
