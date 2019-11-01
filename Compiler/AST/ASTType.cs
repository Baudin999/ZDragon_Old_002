using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTType : IASTNode
    {
        public string Name { get; private set; } = "";
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; set; } = Enumerable.Empty<ASTDirective>();

        public IEnumerable<string> Parameters = Enumerable.Empty<string>();
        public IEnumerable<ASTTypeField> Fields = Enumerable.Empty<ASTTypeField>();

        public ASTType() { }
        public ASTType(
                string name,
                IEnumerable<string> parameters,
                IEnumerable<ASTTypeField> fields,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives)
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
            result.Name = parser.Consume(TokenType.Identifier).Value;
            result.Parameters =
                parser
                    .ConsumeWhile(TokenType
                    .GenericParameter)
                    .Select(v => v.Value)
                    .ToList();

            /*
             * If there is an '=' sign we know that there will be
             * Fields which we can parse...
             */
            if (parser.Current.TokenType == TokenType.Equal)
            {
                parser.Next();
                List<ASTTypeField> fields = new List<ASTTypeField>();
                while (parser.Current.TokenType != TokenType.ContextEnded)
                {
                    fields.Add(new ASTTypeField(parser));
                }
                result.Fields = fields;
                if (fields.Count == 0)
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
