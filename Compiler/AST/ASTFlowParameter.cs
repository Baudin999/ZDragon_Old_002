using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTFlowParameter : IASTNode
    {
        public string Name { get; }
        public Token? Token { get; } = Token.Empty();
        public string Module { get; } = "";
        public IEnumerable<ASTTypeDefinition> Types { get; }

        public ASTFlowParameter(string name, IEnumerable<ASTTypeDefinition> types)
        {
            this.Name = name;
            this.Types = types;
        }


        public static ASTFlowParameter Parse(IParser parser, string module = "")
        {
            var identifiers = ASTTypeDefinition.Parse(parser, module).ToList();
            return new ASTFlowParameter("", identifiers);
        }
    }
}
