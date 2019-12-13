using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTView : IASTNode, INamable, ICloneable
    {
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<string> Nodes { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }
        public ASTView(
            string name,
            string module,
            IEnumerable<string> nodes,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives) {
            this.Name = name;
            this.Module = module;
            this.Nodes = nodes;
            this.Annotations = annotations;
            this.Directives = directives;
        }
        public static (List<ASTError>, ASTView) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives,
                string module = "")
        {
            var errors = new List<ASTError>();

            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            parser.Consume(TokenType.Equal);
            var nodes = parser.ConsumeWhile(TokenType.Identifier).Select(p => p.Value).ToList();
            parser.Consume(TokenType.ContextEnded);

            var result = new ASTView(
                nameId.Value,
                module,
                nodes,
                annotations,
                directives);

            return (errors, result);
        }

        public object Clone() {
            return new ASTView(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Nodes),
                ObjectCloner.CloneList(this.Annotations),
                ObjectCloner.CloneList(this.Directives)
                );
        }

        public object Clone(string name)
        {
            return new ASTView(
                name,
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Nodes),
                ObjectCloner.CloneList(this.Annotations),
                ObjectCloner.CloneList(this.Directives)
                );
        }
    }
}
