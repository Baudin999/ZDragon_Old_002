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

            var composition = parser.TryConsume(TokenType.KW_Compose);
            if (!(composition is null))
            {
                return ASTFlowStepComposition.Parse(parser);
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
            parser.Next();
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


}
