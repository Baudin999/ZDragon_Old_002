using System;
namespace Compiler.AST
{
    public class ASTParagraph: IASTNode, IASTMardownNode
    {
        public string Content { get; }
        public string Module { get; }

        public ASTParagraph(string content, string module = "")
        {
            this.Content = content;
            this.Module = module;
        }

    }
}
