using System;
namespace Compiler.AST
{
    public interface IASTError
    {
        public Token? Token { get;  }
        public string Message { get;  }
    }
}
