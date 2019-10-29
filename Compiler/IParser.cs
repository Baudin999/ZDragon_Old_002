
using System.Collections.Generic;
using Compiler.AST;

namespace Compiler
{
    public interface IParser
    {
        public List<IASTError> Errors { get; }
        public Token Current { get; }

        public bool HasNext();
        public Token Next();
        public bool HasPeek(int index = 1);
        public Token Peek(int index = 1);

        public Token Consume(TokenType tokenType, bool ignoreWhitespace = true);
        public IEnumerable<Token> ConsumeWhile(TokenType tokenType, bool ignoreWhitespace = true);
        public void TryConsume(TokenType tokenType);
        public void TryConsume(TokenType tokenType, out Token t);
    }
}
