using System;
namespace Compiler.AST
{
    public class ASTError : IASTError
    {
        public string Message { get; }
        public ASTError(string message)
        {
            this.Message = message;
        }

    }
}
