using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTData : IASTNode, INamable, ICloneable
    {
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<string> Parameters { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }
        public IEnumerable<ASTDataOption> Options { get; }

        public ASTData(
            string name,
            string module,
            IEnumerable<string> parameters,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTDataOption> options)
        {
            this.Name = name;
            this.Module = module;
            this.Parameters = parameters;
            this.Directives = directives;
            this.Annotations = annotations;
            this.Options = options;
        }

        public static (IEnumerable<IASTError>, ASTData) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives,
                string module = "")
        {
            var errors = new List<ASTError>();

            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            var parameters = parser.ConsumeWhile(TokenType.GenericParameter).Select(p => p.Value).ToList();
            parser.Consume(TokenType.Equal);
            var (optionErrors, options) = ASTDataOption.Parse(parser);
            parser.Consume(TokenType.ContextEnded);

            var result = new ASTData(
                nameId.Value,
                module,
                parameters,
                annotations,
                directives,
                options);

            return (errors.Concat(optionErrors), result);
        }

        public object Clone()
        {
            return new ASTData(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Parameters.ToList()),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Options.ToList()))
            { };
        }
    }
}
