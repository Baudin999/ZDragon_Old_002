using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTType : IASTNode, INamable, IRootNode, ICloneable
    {
        public ASTName ASTName { get; }
        public string Name { get { return this.ASTName.Name; } }
        public string Module { get; }
        public Token? Token { get; } = Token.Empty();
        public IEnumerable<string> Parameters { get; }
        public IEnumerable<string> Extensions { get; }
        public IEnumerable<ASTTypeField> Fields { get; private set; }

        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }

        public ASTType(
                ASTName name,
                string module,
                IEnumerable<string> parameters,
                IEnumerable<string> extensions,
                IEnumerable<ASTTypeField> fields,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives,
                Token? token = null)
        {
            this.ASTName = name;
            this.Module = module;
            this.Parameters = parameters;
            this.Fields = fields;
            this.Extensions = extensions;
            this.Annotations = annotations;
            this.Directives = directives;
        }


        public void AddFields(IEnumerable<ASTTypeField> fields)
        {
            this.Fields = this.Fields.Union(fields);
        }

        public static (List<IASTError>, ASTType?) Parse(IParser parser, List<ASTAnnotation> annotations, List<ASTDirective> directives, string module = "")
        {
            try
            {
                var startColumn = parser.Current.StartColumn;
                var startLine = parser.Current.StartLine;
                var endColumn = 0;
                var endLine = 0;

                var errors = new List<IASTError>();

                parser.Next();
                var name = ASTName.Parse(parser);
  
                var parameters =
                    parser
                        .ConsumeWhile(TokenType.GenericParameter)
                        .Select(v => v.Value)
                        .ToList();


                /*
                 * If there is an 'extends' extension.
                 */
                var extends = parser.TryConsume(TokenType.KW_Extends);
                var extensions = new List<string>();
                if (!(extends is null))
                {
                    extensions =
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
                var fields = new List<ASTTypeField>();
                if (!(equals is null))
                {
                    Token? contextEnded;
                    while ((contextEnded = parser.TryConsume(TokenType.ContextEnded)) == null)
                    {
                        var (field_errors, field) = ASTTypeField.Parse(parser, module);
                        fields.Add(field);
                        errors.AddRange(field_errors);
                    }

                    endLine = contextEnded?.EndLine ?? 0;
                    endColumn = contextEnded?.EndColumn ?? 0;
                    
                    if (fields.Count == 0)
                    {

                        if (parser.Current.TokenType == TokenType.Paragraph)
                        {
                            var nextPhrase = parser.Current.Value;
                            errors.Add(new ASTError($@"
Missing type body. If you use an '=' sign you should have at least one field.
It might be that you are missing an indentation:

type {name} =
{nextPhrase}

Example:
type {name} =
    {nextPhrase}
", "Invalid Syntax", parser.Current));
                        }
                        else
                        {
                            errors.Add(new ASTError($@"
Missing type body. If you use an '=' sign you should have at least one field.", "Invalid Syntax", parser.Current));
                        }
                    }
                }

                var result = new ASTType(
                    name,
                    module,
                    parameters,
                    extensions,
                    fields,
                    annotations,
                    directives,
                    new Token
                    {
                        StartColumn = startColumn,
                        StartLine = startLine,
                        EndColumn = endColumn,
                        EndLine = endLine
                    });

                return (errors.ToList(), result);
            }
            catch (InvalidTokenException ex)
            {
                return (new List<IASTError>
                {
                    new ASTError(ex.Message, "Invalid Syntax", parser.Current)
                }, null);
            }

        }

        public ASTType Clone()
        {
            return new ASTType(
                this.ASTName.Clone<ASTName>(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Parameters),
                ObjectCloner.CloneList(this.Extensions),
                ObjectCloner.CloneList(this.Fields),
                ObjectCloner.CloneList(this.Annotations),
                ObjectCloner.CloneList(this.Directives),
                this.Token
                );
        }

        public ASTType Clone(IEnumerable<ASTTypeField> fields)
        {
            return new ASTType(
                this.ASTName.Clone<ASTName>(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Parameters),
                ObjectCloner.CloneList(this.Extensions),
                fields.ToList(),
                ObjectCloner.CloneList(this.Annotations),
                ObjectCloner.CloneList(this.Directives),
                this.Token
                );
        }

        public override string ToString() => $"type {Module}.{Name}";
        object ICloneable.Clone() => this.Clone();
    }
}
