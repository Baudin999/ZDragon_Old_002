using System;
namespace Compiler.AST
{
    public class ASTParagraph: IASTNode, IASTMardownNode
    {
        public string Content { get; }
        public ASTParagraph(string content)
        {
            this.Content = content;
        }

    }
}
