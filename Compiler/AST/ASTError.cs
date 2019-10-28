using System;
namespace Compiler.AST
{
    public class ASTError
    {
        public string Message { get; }
        public ASTError(string message)
        {
            this.Message = message;
        }

    }
}
