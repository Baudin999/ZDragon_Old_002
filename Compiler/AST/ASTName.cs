using System;
namespace Compiler.AST
{
    public class ASTName : ICloneable
    {
        public string Name { get; }
        public Token Token { get; }

        public ASTName(string name, Token token )
        {
            this.Name = name;
            this.Token = token;
        }


        public static ASTName Parse(IParser parser)
        {
            var identifier = parser.Consume(TokenType.Identifier);
            return new ASTName(identifier.Value, identifier);
        }

        public object Clone()
        {
            return new ASTName(
                this.Name.Clone<string>(),
                Token.Clone<Token>());
        }
    }
}
