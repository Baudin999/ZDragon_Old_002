using System;
namespace Compiler.AST
{
    public class ASTChapter : IASTNode, IASTMardownNode
    {
        public string Content { get; set; }
        public ASTChapter() { }
        public ASTChapter(string content)
        {
            this.Content = content;
        }
    }
}
