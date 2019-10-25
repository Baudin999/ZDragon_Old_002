using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Lexer
    {

        private bool _ignoreWhiteSpace = true;

        private string prepareSource(string source)
        {
            source = new Regex(@"\r\n|\n").Replace(source, "↓");
            source = new Regex(@"[\s\t]+↓").Replace(source, "↓");
            source = new Regex("    |\t").Replace(source, "→");
            return source;
        }

        public Lexer IgnoreWhitespace(bool ignoreWhitespace)
        {
            _ignoreWhiteSpace = ignoreWhitespace;
            return this;
        }

        public IEnumerable<Token> Lex(string code)
        {
            string preparedSource = prepareSource(code);
            var input = new Input(preparedSource);
            var context = false;

            while (input.HasNext())
            {
                if (input.IsEqualTo("type") ||
                    input.IsEqualTo("alias") ||
                    input.IsEqualTo("data") ||
                    input.IsEqualTo("choice"))
                {
                    context = true;
                    yield return new Token() { TokenType = TokenType.ContextStarted };
                    yield return TokenLexers.Word(input);
                }
                else if (!context && Char.IsLetter(input.Current()))
                {
                    yield return TokenLexers.TakeUntillEndOfContext(input);
                    yield return new Token() { TokenType = TokenType.ContextEnded };
                }
                else if (context && Char2.IsNewLine(input.Current()) && TokenLexers.EndContext(input, 1))
                {
                    context = false;
                    yield return new Token() { TokenType = TokenType.ContextEnded };
                }
                else if (context && Char.IsNumber(input.Current()))
                {
                    yield return TokenLexers.Number(input);
                }
                else if (context && Char.IsUpper(input.Current()))
                {
                    yield return TokenLexers.Identifier(input);
                }
                else if (context && input.Current()== '\'')
                {
                    yield return TokenLexers.GenericParameter(input);
                }
                else if (context && Char.IsLower(input.Current()))
                {
                    yield return TokenLexers.Word(input);
                }
                else if (context && input.Current() == '"')
                {
                    yield return TokenLexers.String(input);
                }
                else if (context && char.IsNumber(input.Current()))
                {
                    yield return TokenLexers.Number(input);
                }
                else if (input.Current() == '{')
                {
                    yield return TokenLexers.Comment(input);
                }
                else if (input.Current() == '#')
                {
                    yield return TokenLexers.Chapter(input);
                }
                else if (input.Current() == '@')
                {
                    yield return TokenLexers.Annotation(input);
                }
                else if (input.Current() == '%')
                {
                    yield return TokenLexers.Directive(input);
                }
                else if (input.Current() == ';')
                {
                    yield return TokenLexers.Take(input, TokenType.EndStatement);
                }
                else if (Char2.Indent(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Indent);
                }
                else if (Char2.Equal(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Equal);
                }
                else if (Char2.Or(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Or);
                }
                else if (Char2.Separator(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Separator);
                }
                else if (Char2.IsNewLine(input.Current()))
                {
                    var newline = TokenLexers.Take(input, TokenType.NewLine);
                    newline.Value = "↓";
                    yield return newline;
                }
                else if (Char.IsWhiteSpace(input.Current()))
                {
                    var whiteSpace = TokenLexers.Whitespace(input);
                    if (!_ignoreWhiteSpace)
                    {
                        yield return whiteSpace;
                    }
                }
                else if (!context)
                {
                    yield return TokenLexers.TakeUntillEndOfContext(input);
                }
                else
                {
                    input.Next();
                }
            }

            // If we are still inside of a context when the code has been parsed,
            // we'll want to end the context, jsut for good measure!
            if (context)
            {
                yield return new Token() { TokenType = TokenType.ContextEnded };
                context = false;
            }
        }


    }

}

