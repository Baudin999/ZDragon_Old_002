using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTFlow : IASTNode
    {
        public string Name { get; }
        public IEnumerable<ASTFlowStep> Steps { get; } = Enumerable.Empty<ASTFlowStep>();

        public ASTFlow(string name, IEnumerable<ASTFlowStep> steps)
        {
            this.Name = name;
            this.Steps = steps;
        }


        public static (List<ASTError>, ASTFlow) Parse(IParser parser)
        {
            var errors = new List<ASTError>();
            parser.Next();
            var name = parser.Or(TokenType.Identifier, TokenType.String);
            var equals = parser.TryConsume(TokenType.Equal);
            var steps = new List<ASTFlowStep>();
            if (!(equals is null))
            {
                while (parser.TryConsume(TokenType.ContextEnded) == null)
                {
                    parser.ConsumeWhile(TokenType.KW_Compose);
                    var (_errors, step) = ASTFlowStep.Parse(parser);
                    steps.Add(step);
                    errors.AddRange(_errors);
                }
            }
            return (errors, new ASTFlow(name.Value.Replace("\"", ""), steps));
        }
    }
}
