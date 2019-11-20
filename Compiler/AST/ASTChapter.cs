using System;
namespace Compiler.AST
{
    public class ASTChapter : IASTNode, IASTMardownNode, ICloneable
    {
        public string Content { get; set; }
        public ASTChapter() { }
        public ASTChapter(string content)
        {
            this.Content = content;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
