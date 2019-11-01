using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class Parser : IParser
    {
        private readonly int length;
        private readonly List<Token> tokenStream;
        private int position;

        public Parser(IEnumerable<Token> tokenStream)
        {
            this.tokenStream = tokenStream.ToList();
            this.length = tokenStream.Count();
            this.position = 0;
            Errors = new List<IASTError>();
        }

        public List<IASTError> Errors { get; }
        public Token Current => tokenStream[position];
        public bool HasNext() => position < length;
        public Token Next() => tokenStream[position++];
        public bool HasPeek(int index = 1) => (position + index) < length;
        public Token Peek(int index = 1) => tokenStream[position + index];


        public IEnumerable<IASTNode> Parse()
        {
            List<ASTAnnotation> annotations = new List<ASTAnnotation>();
            List<ASTDirective> directives = new List<ASTDirective>();
            while (HasNext())
            {
                if (Current.TokenType == TokenType.KW_Type)
                {
                    var (errors, t) = ASTType.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    yield return t;
                }
                else if (Current.TokenType == TokenType.KW_Alias)
                {
                    var (errors, alias) = ASTAlias.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    yield return alias;
                }
                else if (Current.TokenType == TokenType.KW_Choice)
                {
                    yield return new ASTChoice(this);
                }
                else if (Current.TokenType == TokenType.Annotation)
                {
                    annotations = ASTAnnotation.Parse(this).ToList();
                }
                else if (Current.TokenType == TokenType.Directive)
                {
                    var (errors, dirs) = ASTDirective.Parse(this);
                    Errors.AddRange(errors.ToList());
                    directives = dirs.ToList();
                }
                else
                {
                    Next();
                }
            }
            yield break;
        }

        /// <summary>
        /// Consume a token of a type but ignore newlines, indentation or whitespace.
        /// This is a convenience method which can let you focus on the core of the
        /// application logic and not on teh fluff.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ignoreWhitespace"></param>
        /// <returns></returns>
        public Token Consume(TokenType tokenType, bool ignoreWhitespace = true)
        {
            while (true)
            {
                if (this.Current.TokenType == tokenType)
                {
                    var result = this.Current;
                    if (HasNext()) this.Next();
                    return result;
                }
                else if (this.Current.TokenType == TokenType.Indent || this.Current.TokenType == TokenType.NewLine)
                {
                    this.Next();
                }
                else
                {
                    throw new InvalidTokenException($"Trying to parse tokenType {tokenType}, but found {Current.TokenType}. ");
                }
            }
        }
        
        public IEnumerable<Token> ConsumeWhile(TokenType tokenType, bool ignoreWhitespace = true)
        {
            while (true)
            {
                if (this.Current.TokenType == tokenType)
                {
                    var result = this.Current;
                    if (this.HasNext()) this.Next();
                    yield return result;
                }
                else if (HasNext() && (this.Current.TokenType == TokenType.Indent || this.Current.TokenType == TokenType.NewLine))
                {
                    this.Next();
                }
                else
                {
                    break;
                }
            }
        }

        public Token? TryConsume(TokenType tokenType)
        {
            Token? t;
            TryConsume(tokenType, out t);
            return t;
        }


        public Token? TryConsume(TokenType tokenType, out Token? t)
        {
            Token? result = default;
            int index = 0;
            while (HasPeek(index))
            {
                var token = Peek(index);
                if (token.TokenType == tokenType)
                {
                    result = token;
                    for (int i = 0; i <= index; ++i)
                    {
                        if (HasNext()) Next();
                    }
                    break;
                }
                else if (token.TokenType == TokenType.Indent || token.TokenType == TokenType.NewLine)
                {
                    index += 1;
                }
                else
                {
                    break;
                }
            }

            t = result;
            return t;
        }

    }
}
