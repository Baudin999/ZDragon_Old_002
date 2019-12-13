using System;
namespace Compiler.AST
{
    public class ASTChapter : IASTNode, IASTMardownNode
    {
        public string Content { get; }
        public string Module { get; }
        public ASTChapter(string content, string module = "")
        {
            this.Content = content.Trim();
            this.Module = module;
        }
    }
}
