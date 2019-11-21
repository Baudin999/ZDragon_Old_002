using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTChoice : IASTNode, INamable, ICloneable
    {
        public string Name { get; set; } = "";
        public List<ASTTypeDefinition> Type { get; set; } = new List<ASTTypeDefinition>();
        public List<ASTOption> Options { get; private set; } = new List<ASTOption>();

        public ASTChoice() { }

        public ASTChoice(string name, List<ASTTypeDefinition> type, List<ASTOption> options)
        {
            this.Name = name;
            this.Type = type;
            this.Options = options;
        }

        public static (List<ASTError>, ASTChoice) Parse(IParser parser)
        {
            if (parser.HasNext()) parser.Next();

            var result = new ASTChoice();

            var nameId = parser.Consume(TokenType.Identifier);
            result.Name = nameId.Value;
            parser.Consume(TokenType.Equal);
            result.Type = ASTTypeDefinition.ParseType(parser).ToList();
            result.Options = ASTOption.Parse(parser).ToList();
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);
            //var result = new ASTChoice(nameId.Value, ASTTypeDefinition.ParseType(parser).ToList(), ASTOption.Parse(parser).ToList());
            return (new List<ASTError>(), result);
        }

        public object Clone()
        {
            return new ASTChoice
            {
                Name = (string)this.Name.Clone(),
                Type = ObjectCloner.CloneList(this.Type),
                Options = ObjectCloner.CloneList(this.Options)
            };
        }
    }
}
