using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public interface IElement
    {
        public string Name { get; }
        public IEnumerable<ASTTypeDefinition> Type { get;  }
    }
}
