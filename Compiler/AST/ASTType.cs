using System;
namespace Compiler.AST
{
    public class ASTType : IASTNode
    {
        public string Name { get; private set; }

        public ASTType(Parser parser)
        {
            parser.Next();
            if (parser.Current.TokenType == TokenType.Identifier)
            {
                this.Name = parser.Current.Value;
                parser.Next();
            }
        }

    }
}
