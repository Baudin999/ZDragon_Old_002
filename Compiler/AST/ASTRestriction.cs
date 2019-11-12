using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTRestriction : IASTNode
    {
        public string Key { get; set;  }
        public string Value { get; set;  }
        public int Depth { get; set; }
        public IEnumerable<ASTAnnotation> Annotations { get; set;  }
        public Token Token { get; set; }

        public ASTRestriction() { }
        public ASTRestriction(string key, string value, IEnumerable<ASTAnnotation> annotations, Token token, int depth)
        {
            this.Key = key;
            this.Value = value;
            this.Annotations = annotations;
            this.Token = token;
            this.Depth = depth;
        }

        public static IEnumerable<ASTRestriction> CreateRestrictions(IParser parser, TokenType root)
        {
            int depth = root == TokenType.KW_Alias ? 1 : 2;
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

        public ASTRestriction Clone()
        {
            return new ASTRestriction(this.Key, this.Value, this.Annotations.Select(a => new ASTAnnotation(a.Value)), this.Token, this.Depth);
        }
    }
}
