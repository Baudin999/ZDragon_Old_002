using System;
namespace Compiler.AST
{
    public interface IASTError
    {
        public string Message { get;  }
    }
}
