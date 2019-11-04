﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTType : IASTNode, INamable
    {
        public string Name { get; private set; } = "";
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; set; } = Enumerable.Empty<ASTDirective>();

        public IEnumerable<string> Parameters = Enumerable.Empty<string>();
        public IEnumerable<string> Extensions = Enumerable.Empty<string>();
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
            ASTType result = new ASTType();
            try
            {
                List<ASTError> errors = new List<ASTError>();

                result.Annotations = annotations;
                result.Directives = directives;

                parser.Next();
                result.Name = parser.Consume(TokenType.Identifier).Value;
                result.Parameters =
                    parser
                        .ConsumeWhile(TokenType.GenericParameter)
                        .Select(v => v.Value)
                        .ToList();


                /*
                 * If there is an 'extends' extension.
                 */
                var extends = parser.TryConsume(TokenType.KW_Extends);
                if (!(extends is null))
                {
                    result.Extensions =
                        parser
                            .ConsumeWhile(TokenType.Identifier)
                            .Select(v => v.Value)
                            .ToList();
                }


                /*
                 * If there is an '=' sign we know that there will be
                 * Fields which we can parse...
                 */
                var equals = parser.TryConsume(TokenType.Equal);
                if (!(equals is null))
                {
                    List<ASTTypeField> fields = new List<ASTTypeField>();
                    while (parser.TryConsume(TokenType.ContextEnded) == null)
                    {
                        fields.Add(new ASTTypeField(parser));
                    }
                    result.Fields = fields;
                    if (fields.Count == 0)
                    {

                        if (parser.Current.TokenType == TokenType.Paragraph)
                        {
                            var nextPhrase = parser.Current.Value;
                            errors.Add(new ASTError($@"
Missing type body. If you use an '=' sign you should have at least one field.
It might be that you are missing an indentation:

type {result.Name} =
{nextPhrase}

Example:
type {result.Name} =
    {nextPhrase}
", parser.Current));
                        }
                        else
                        {
                            errors.Add(new ASTError($@"
Missing type body. If you use an '=' sign you should have at least one field.", parser.Current));
                        }
                    }
                }

                return (errors, result);
            }
            catch (InvalidTokenException ex)
            {
                return (new List<ASTError>
                {
                    new ASTError(ex.Message, parser.Current)
                }, result);
            }

        }

    }
}
