using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTChoice : IASTNode, INamable, ICloneable
    {
        public Token? Token { get; } = Token.Empty();
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }
        public IEnumerable<ASTOption> Options { get; }

        public ASTChoice(
            string name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTOption> options)
        {
            this.Name = name;
            this.Module = module;
            this.Annotations = annotations;
            this.Directives = directives;
            this.Options = options;
        }

        public static (IEnumerable<IASTError>, ASTChoice) Parse(
            IParser parser,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            string module = "")
        {
            if (parser.HasNext()) parser.Next();

            var nameId = parser.Consume(TokenType.Identifier);
            parser.Consume(TokenType.Equal);
            //result.Type = ASTTypeDefinition.Parse(parser).ToList();
            var options = ASTOption.Parse(parser).ToList();
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);

            var result = new ASTChoice(
                nameId.Value,
                module,
                annotations,
                directives,
                options);

            return (new List<ASTError>(), result);
        }

        public object Clone()
        {
            return new ASTChoice(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations),
                ObjectCloner.CloneList(this.Directives),
                ObjectCloner.CloneList(this.Options)
                );
        }
    }
}
