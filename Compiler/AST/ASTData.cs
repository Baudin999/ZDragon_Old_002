using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTData : IASTNode, INamable, ICloneable
    {
        public string Name { get; set; } = "";
        public IEnumerable<string> Parameters { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; set; } = Enumerable.Empty<ASTDirective>();
        public IEnumerable<ASTDataOption> Options { get; set; } = Enumerable.Empty<ASTDataOption>();

        public ASTData() { }

        public static (List<ASTError>, ASTData) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives)
        {
            List<ASTError> errors = new List<ASTError>();
            ASTData result = new ASTData
            {
                Annotations = annotations,
                Directives = directives
            };

            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            result.Name = nameId.Value;
            result.Parameters = parser.ConsumeWhile(TokenType.GenericParameter).Select(p => p.Value).ToList();
            parser.Consume(TokenType.Equal);

            result.Options = ASTDataOption.Parse(parser).ToList();
            
            parser.Consume(TokenType.ContextEnded);

            return (errors, result);
        }

        public object Clone()
        {
            return new ASTData
            {
                Name = (string)this.Name.Clone(),
                Parameters = ObjectCloner.CloneList(this.Parameters.ToList()),
                Annotations = ObjectCloner.CloneList(this.Annotations.ToList()),
                Directives = ObjectCloner.CloneList(this.Directives.ToList()),
                Options = ObjectCloner.CloneList(this.Options.ToList())
            };
        }
    }
}
