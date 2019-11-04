using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public interface IRestrictable
    {
        public IEnumerable<ASTRestriction> Restrictions { get; }
    }
}
