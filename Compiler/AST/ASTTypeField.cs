using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public List<ASTAnnotation> Annotations { get; private set; }

        public ASTTypeField(Parser parser)
        {
            this.Annotations = ASTAnnotation.Parse(parser).ToList();
            this.Name = parser.Consume(TokenType.Identifier).Value;
            Token Separator = parser.Consume(TokenType.Separator);
            this.Type = parser.Consume(TokenType.Identifier).Value;


            while (parser.Current.TokenType != TokenType.EndStatement)
            {
                parser.Next();
            }

            parser.TryConsume(TokenType.EndStatement);
        }

    }
}
