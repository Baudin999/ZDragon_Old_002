using System;
namespace Compiler.AST
{
    public class ASTError : IASTError
    {
        public Token? Token { get; }
        public string Message { get; }
        public ASTError(string message, Token? token)
        {
            this.Message = message;
            this.Token = token;
        }

    }
}
