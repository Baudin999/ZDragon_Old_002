using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTFlowParameter
    {
        public string Name { get; }
        public IEnumerable<ASTTypeDefinition> Types { get; }

        public ASTFlowParameter(string name, IEnumerable<ASTTypeDefinition> types)
        {
            this.Name = name;
            this.Types = types;
        }


        public static ASTFlowParameter Parse(IParser parser)
        {
            var identifiers = ASTTypeDefinition.Parse(parser).ToList();
            return new ASTFlowParameter("", identifiers);
        }
    }
}
