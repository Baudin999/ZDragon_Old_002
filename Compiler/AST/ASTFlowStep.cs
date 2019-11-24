using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{

    public interface IFlowStep { }

    public class ASTFlowStep : IFlowStep
    {
        public string From { get; }
        public string To { get; }
        public IEnumerable<ASTFlowParameter> Parameters { get; }

        public ASTFlowStep(string from, string to, IEnumerable<ASTFlowParameter> parameters)
        {
            this.From = from;
            this.To = to;
            this.Parameters = parameters;
        }


        public static (List<ASTError>, IFlowStep) Parse(IParser parser)
        {
            Func<string, string> t = (string s) => s.Replace("\"", "");

            if (!(parser.TryConsume(TokenType.KW_Compose) is null))
            {
                return ASTFlowStepComposition.Parse(parser);
            }
            else if (!(parser.TryConsume(TokenType.KW_Loop) is null))
            {
                return ASTFlowStepLoop.Parse(parser);
            }
            else
            {
                var from = parser.Or(TokenType.Identifier, TokenType.String);
                var next = parser.Consume(TokenType.Op_Next);
                var to = parser.Or(TokenType.Identifier, TokenType.String);
                var def = parser.Consume(TokenType.Op_Def);

                var parameters = new List<ASTFlowParameter>();
                while (parser.Current.TokenType != TokenType.EndStatement)
                {
                    var typeDefinition = ASTFlowParameter.Parse(parser);
                    parser.TryConsume(TokenType.Op_Next);
                    parameters.Add(typeDefinition);
                }

                var endStep = parser.Consume(TokenType.EndStatement);

                return (new List<ASTError>(), new ASTFlowStep(t(from.Value), t(to.Value), parameters));
            }
        }
    }


    public class ASTFlowStepComposition : IFlowStep
    {
        public IEnumerable<IFlowStep> Steps { get; } = Enumerable.Empty<IFlowStep>();
        public ASTFlowStepComposition(IEnumerable<IFlowStep> steps)
        {
            this.Steps = steps;
        }

        public static (List<ASTError>, IFlowStep) Parse(IParser parser)
        {
            var errors = new List<ASTError>();
            var steps = new List<IFlowStep>();

            while (parser.TryConsume(TokenType.EndStatement) == null)
            {
                var (_errors, step) = ASTFlowStep.Parse(parser);
                steps.Add(step);
                errors.AddRange(_errors);
            }

            return (new List<ASTError>(), new ASTFlowStepComposition(steps));
        }
    }


    public class ASTFlowStepLoop : IFlowStep
    {
        public string Condition { get;  }
        public IFlowStep Step { get; }
        public ASTFlowStepLoop(string condition, IFlowStep step)
        {
            this.Condition = condition;
            this.Step = step;
        }

        public static (List<ASTError>, IFlowStep) Parse(IParser parser)
        {
            var condition = parser.Consume(TokenType.String);
            var (_errors, step) = ASTFlowStep.Parse(parser);
            parser.Consume(TokenType.EndStatement);

            return (_errors, new ASTFlowStepLoop(condition.Value.Replace("\"", ""), step));
        }
    }


}
