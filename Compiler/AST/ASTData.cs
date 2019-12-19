using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTData : IASTNode, INamable, ICloneable
    {
        public ASTName ASTName { get; }
        public string Name { get { return this.ASTName.Name; } }
        public Token? Token { get; } = Token.Empty();
        public string Module { get; }
        public IEnumerable<string> Parameters { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }
        public IEnumerable<ASTDataOption> Options { get; }

        public ASTData(
            ASTName astName,
            string module,
            IEnumerable<string> parameters,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTDataOption> options)
        {
            this.ASTName = astName;
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
            var name = ASTName.Parse(parser);//parser.Consume(TokenType.Identifier);
            var parameters = parser.ConsumeWhile(TokenType.GenericParameter).Select(p => p.Value).ToList();
            parser.Consume(TokenType.Equal);
            var (optionErrors, options) = ASTDataOption.Parse(parser);
            parser.Consume(TokenType.ContextEnded);

            var result = new ASTData(
                name,
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
                this.ASTName.Clone<ASTName>(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Parameters.ToList()),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Options.ToList()));
        }
    }
}
