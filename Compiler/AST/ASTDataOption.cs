using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTDataOption : IASTNode, ICloneable
    {
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<string> Parameters { get; }

        public ASTDataOption(string name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<string> parameters) {
            this.Name = name;
            this.Module = module;
            this.Annotations = annotations;
            this.Parameters = parameters;
        }

        public static (IEnumerable<IASTError>, IEnumerable<ASTDataOption>) Parse(IParser parser, string module = "")
        {
            var errors = new List<ASTError>();
            var result = new List<ASTDataOption>();

            while (!parser.IsNext(TokenType.ContextEnded))
            {
                var annotations = ASTAnnotation.Parse(parser);
                parser.Consume(TokenType.Or);

                var id = parser.Consume(TokenType.Identifier);
                var tokens = parser.ConsumeWhile(TokenType.Identifier, TokenType.GenericParameter);
                var parameters = tokens.Select(t => t.Value).ToList();
                parser.TryConsume(TokenType.EndStatement);

                result.Add(new ASTDataOption(id.Value, module, annotations, parameters));
            }
            return (errors, result);
        }

        public object Clone()
        {
            return new ASTDataOption(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Parameters.ToList())
                );
        }
    }
}
