using System;
namespace Compiler.AST
{
    public class ASTChapter : IASTNode, IASTMardownNode
    {
        public string Content { get; } = "";
        public ASTChapter() { }
        public ASTChapter(string content)
        {
            this.Content = content.Trim();
        }
    }
}
