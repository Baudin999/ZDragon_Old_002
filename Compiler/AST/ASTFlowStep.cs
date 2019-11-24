using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTFlowStep
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


        public static (List<ASTError>, ASTFlowStep) Parse(IParser parser)
        {
            Func<string, string> t = (string s) => s.Replace("\"", "");

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
