using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public interface IElement
    {
        public ASTName ASTName { get; }
    }

    public interface ITypeble
    {
        public IEnumerable<ASTTypeDefinition> Types { get; }
    }
}
