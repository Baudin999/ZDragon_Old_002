using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class Parser
    {
        private int position;
        private readonly int length;
        private readonly List<Token> tokenStream;

        public Token Current => tokenStream[position];

        public Parser(IEnumerable<Token> tokenStream)
        {
            this.tokenStream = tokenStream.ToList();
            this.length = tokenStream.Count();
            this.position = 0;
        }

        public bool HasNext() => position < length;
        public Token Next() => tokenStream[position++];


        public IEnumerable<object> Parse()
        {
            while (HasNext())
            {
                if (Current.TokenType == TokenType.KW_Type)
                {
                    yield return new ASTType(this);
                }
                else
                {
                    Next();
                }

            }
        }

        /// <summary>
        /// Consume a token of a type but ignore newlines, indentation or whitespace.
        /// This is a convenience method which can let you focus on the core of the
        /// application logic and not on teh fluff.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ignoreWhitespace"></param>
        /// <returns></returns>
        public Token Consume(TokenType tokenType, bool ignoreWhitespace)
        {
            ////
            return null;
        }

        public void TryConsume(TokenType tokenType, out Token t)
        {
            if (this.Current.TokenType == tokenType)
            {
                t = Next();
            }
            else
            {
                t = null;
            }
        }

        public void TryConsume(TokenType tokenType)
        {
            Token t;
            TryConsume(tokenType, out t);
        }

        public Token Peek(int pos = 1) => tokenStream[position + pos];
    }
}
