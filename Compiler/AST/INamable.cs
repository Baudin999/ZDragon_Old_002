using System;
namespace Compiler.AST
{
    public interface INamable : IASTNode
    {
        string Name { get; }
    }
}
