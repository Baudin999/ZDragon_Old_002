using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField
    {
        public string Name { get; private set; }
        public List<ASTAnnotation> Annotations { get; private set; }

        public ASTTypeField(Parser parser)
        {
            this.Annotations = ASTAnnotation.Parse(parser).ToList();

            parser.TryConsume(TokenType.NewLine);
            parser.TryConsume(TokenType.Indent);

            if (parser.Current.TokenType == TokenType.Identifier)
            {
                this.Name = parser.Current.Value;
            }

            while (parser.Current.TokenType != TokenType.EndStatement)
            {
                parser.Next();
            }

            parser.TryConsume(TokenType.EndStatement);
        }

    }
}
