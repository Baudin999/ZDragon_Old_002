using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTFlow : IASTNode
    {
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<IFlowStep> Steps { get; } = Enumerable.Empty<IFlowStep>();

        public ASTFlow(string name, string module, IEnumerable<IFlowStep> steps)
        {
            this.Name = name;
            this.Steps = steps;
            this.Module = module;
        }


        public static (List<ASTError>, ASTFlow) Parse(IParser parser, string module = "")
        {
            var errors = new List<ASTError>();
            parser.Next();
            var name = parser.Or(TokenType.Identifier, TokenType.String);
            var equals = parser.TryConsume(TokenType.Equal);
            var steps = new List<IFlowStep>();
            if (!(equals is null))
            {
                while (parser.TryConsume(TokenType.ContextEnded) == null)
                {
                    var (_errors, step) = ASTFlowStep.Parse(parser);
                    steps.Add(step);
                    errors.AddRange(_errors);
                }
            }
            return (errors, new ASTFlow(name.Value.Replace("\"", ""), module, steps));
        }
    }
}
