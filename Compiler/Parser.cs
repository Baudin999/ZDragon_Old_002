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

        private Parser()
        {
            length = 0;
            tokenStream = new List<Token>();
            position = 0;
            Errors = new List<IASTError>();
        }
        public Parser(IEnumerable<Token> tokenStream)
        {
            this.tokenStream = tokenStream.ToList();
            this.length = tokenStream.Count();
            this.position = 0;
            Errors = new List<IASTError>();
        }

        public List<IASTError> Errors { get; }
        public Token Current => tokenStream[position];
        public bool HasNext() => HasPeek(); 
        public Token Next() => tokenStream[position++];
        public bool HasPeek(int index = 1) => (position + index) < length;
        public Token Peek(int index = 1) => tokenStream[position + index];

        public static Parser Empty() => new Parser();
        public static string[] BaseTypes = { "String", "Number", "Boolean", "Date", "Time", "DateTime" };

        public IEnumerable<IASTNode> Parse()
        {
            var annotations = new List<ASTAnnotation>();
            var directives = new List<ASTDirective>();
            while (HasNext() && Current.TokenType != TokenType.EndOfFile)
            {
                
                if (Current.TokenType == TokenType.KW_Type)
                {
                    var (errors, t) = ASTType.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
                    if (!(t is null)) yield return t;
                }
                else if (Current.TokenType == TokenType.KW_Alias)
                {
                    var (errors, alias) = ASTAlias.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
                    yield return alias;
                }
                else if (Current.TokenType == TokenType.KW_Choice)
                {
                    var (errors, result) = ASTChoice.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    yield return result;
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
                }
                else if (Current.TokenType == TokenType.KW_Data)
                {
                    var (errors, data) = ASTData.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    yield return data;
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
                }
                else if (Current.TokenType == TokenType.KW_View)
                {
                    var (errors, data) = ASTView.Parse(this, annotations, directives);
                    Errors.AddRange(errors);
                    yield return data;
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
                }
                else if (Current.TokenType == TokenType.KW_Open)
                {
                    var (errors, data) = ASTImport.Parse(this);
                    Errors.AddRange(errors);
                    yield return data;
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
                }
                else if (Current.TokenType == TokenType.KW_Flow)
                {
                    var (errors, data) = ASTFlow.Parse(this);
                    Errors.AddRange(errors);
                    yield return data;
                    annotations = new List<ASTAnnotation>();
                    directives = new List<ASTDirective>();
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
                else if (Current.TokenType == TokenType.Chapter)
                {
                    yield return new ASTChapter(Current.Value);
                    Next();
                }
                else if (Current.TokenType == TokenType.Paragraph)
                {
                    yield return new ASTParagraph(Current.Value);
                    Next();
                }
                else
                {

                    Next();
                } 
            }
            yield break;
        }

        public Token? Previous(int index = -1)
        {
            return this.HasPeek(index) ? this.Peek(index) : null;
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
                    var previous1 = this.Previous(-2)?.Value ?? "";
                    var previous = this.Previous()?.Value ?? "";
                    var value = this.Current.Value;
                    var next = this.Peek()?.Value ?? "";
                    var next1 = this.Peek(2)?.Value ?? "";
                    var message = tokenType switch
                    {
                        TokenType.Identifier => $@"
Expected an Identifier but found a {this.Current.TokenType}: '{value}'.
Line {this.Current.StartLine}, Column {this.Current.StartColumn}
...{previous1} {previous} {value} {next} {next1}...

In ZDragon we expect types to be represented by an Identifier and
identifiers always start with a capital letter and have no spaces
or other symbols.
",
                        TokenType.EndStatement => $@"
Expected an Enstatement but found a {this.Current.TokenType}: '{value}'.
...{previous1} {previous} {value} {next} {next1}...
",
                        _ => $"Invalid Token: Expected a {tokenType} but found {this.Current.TokenType} on line {this.Current.StartLine} and column {this.Current.StartColumn}"
                    };
                    throw new InvalidTokenException(message);
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

        public IEnumerable<Token> ConsumeWhile(TokenType first, TokenType second, bool ignoreWhitespace = true)
        {
            while (true)
            {
                if (this.Current.TokenType == first || this.Current.TokenType == second)
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

        public Token Or(TokenType first, TokenType second)
        {
            return TryConsume(first) ?? TryConsume(second) ?? throw new InvalidTokenException($@"
Expected either {first} or {second} but encoutered {this.Current.TokenType}.
{Current.ToString()}
");
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
            var index = 0;
            while (HasPeek(index))
            {
                var token = Peek(index);
                if (token.TokenType == tokenType)
                {
                    result = token;
                    for (var i = 0; i <= index; ++i)
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

        public bool IsNext(TokenType tokenType)
        {
            var index = 0;
            while (HasPeek(index))
            {
                var token = Peek(index);
                if (token.TokenType == tokenType)
                {
                    return true;
                }
                else if (token.TokenType == TokenType.Indent || token.TokenType == TokenType.NewLine)
                {
                    index += 1;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

    }
}
