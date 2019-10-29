using System;
using System.Collections;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTType : IASTNode
    {
        public string Name { get; private set; }
        public List<ASTAnnotation> Annotations { get; set; }
        public List<ASTDirective> Directives { get; set; }

        public List<string> Parameters = new List<string>();
        public List<ASTTypeField> Fields = new List<ASTTypeField>();

        public ASTType() { }
        public ASTType(string name, List<string> parameters, List<ASTTypeField> fields, List<ASTAnnotation> annotations, List<ASTDirective> directives)
        {
            this.Name = name;
            this.Parameters = parameters;
            this.Fields = fields;
            this.Annotations = annotations;
            this.Directives = directives;
        }

        public static (List<ASTError>, ASTType) Parse(IParser parser, List<ASTAnnotation> annotations, List<ASTDirective> directives)
        {
            List<ASTError> errors = new List<ASTError>();
            ASTType result = new ASTType();

            result.Annotations = annotations;
            result.Directives = directives;
            parser.Next();
            if (parser.Current.TokenType == TokenType.Identifier)
            {
                result.Name = parser.Current.Value;
                parser.Next();
            }

            while (parser.Current.TokenType == TokenType.GenericParameter)
            {
                result.Parameters.Add(parser.Current.Value);
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
                    result.Fields.Add(new ASTTypeField(parser));
                }
                if (result.Fields.Count == 0)
                {
                    errors.Add(new ASTError($"Missing type body. If you use an '=' sign you should have at least one field."));
                }
            }
            else
            {
                parser.Next();
            }

            return (errors, result);
        }

    }
}
