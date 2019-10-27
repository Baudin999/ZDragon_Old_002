using System;
using System.Collections;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTType : IASTNode
    {
        public string Name { get; private set; }
        public List<ASTAnnotation> Annotations { get; set; }

        public List<string> Parameters = new List<string>();
        public List<ASTTypeField> Fields = new List<ASTTypeField>();

        public ASTType(Parser parser, List<ASTAnnotation> annotations)
        {
            this.Annotations = annotations;
            parser.Next();
            if (parser.Current.TokenType == TokenType.Identifier)
            {
                this.Name = parser.Current.Value;
                parser.Next();
            }

            while (parser.Current.TokenType == TokenType.GenericParameter)
            {
                this.Parameters.Add(parser.Current.Value);
                parser.Next();
            }

            /*
             * If there is an '=' sign we know that there will be
             * Fields which we can parse...
             */
            if (parser.Current.TokenType == TokenType.Equal)
            {
                parser.Next();
                while (parser.Current.TokenType != TokenType.ContextEnded)
                {
                    Fields.Add(new ASTTypeField(parser));
                }
            }
            else
            {
                parser.Next();
            }
            
        }

    }
}
