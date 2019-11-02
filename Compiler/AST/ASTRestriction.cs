using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTRestriction : IASTNode
    {
        public string Key { get; }
        public string Value { get; }
        public int Depth { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public Token Token { get; }

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
                var numberValue = parser.TryConsume(TokenType.Number);
                var stringValue = parser.TryConsume(TokenType.String);
                var patternValue = parser.TryConsume(TokenType.Pattern);
                if (numberValue != null)
                {
                    yield return new ASTRestriction(word.Value, numberValue.Value, annotations, numberValue, depth);
                }
                else if (stringValue != null)
                {
                    yield return new ASTRestriction(word.Value, stringValue.Value, annotations, stringValue, depth);
                }
                else if (patternValue != null)
                {
                    yield return new ASTRestriction(word.Value, patternValue.Value, annotations, patternValue, depth);
                }

                annotations = ASTAnnotation.Parse(parser);
                parser.TryConsume(TokenType.And, out t);
            }
        }
    }
}
