using System;
namespace Compiler.AST
{
    public class ASTError : IASTError
    {
        public Token? Token { get; }
        public string Message { get; }
        public string Title { get; } = "";

        public ASTError(string message, string title = "", Token? token = null)
        {
            this.Message = message;
            this.Token = token;
            this.Title = title;
        }

    }
}
