using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compiler
{
    public class Lexer
    {

        private bool _ignoreWhiteSpace = true;
        internal const char NEWLINE = '↓';
        internal const char INDENT = '→';

        private string PrepareSource(string source)
        {
            source = source += " ";
            source = new Regex(@"\r\n|\n").Replace(source, NEWLINE.ToString());
            source = new Regex(@"[\s\t]+↓").Replace(source, NEWLINE.ToString());
            source = new Regex("    |\t").Replace(source, INDENT.ToString());
            return source;
        }

        public Lexer IgnoreWhitespace(bool ignoreWhitespace)
        {
            _ignoreWhiteSpace = ignoreWhitespace;
            return this;
        }

        public IEnumerable<Token> Lex(string code)
        {
            var preparedSource = PrepareSource(code);
            var input = new Input(preparedSource);
            var context = false;

            while (input.HasNext())
            {
                if (input.IsEqualTo("type") ||
                    input.IsEqualTo("alias") ||
                    input.IsEqualTo("data") ||
                    input.IsEqualTo("choice") ||
                    input.IsEqualTo("open") ||
                    input.IsEqualTo("view") ||
                    input.IsEqualTo("flow") ||
                    input.IsEqualTo("aggregate") ||
                    input.IsEqualTo("entity"))
                {
                    context = true;
                    var token = TokenLexers.Word(input);
                    yield return new Token() {
                        EndLine = input.Line,
                        EndColumn = 0,
                        StartLine = input.Line,
                        StartColumn = 0,
                        TokenType = TokenType.ContextStarted
                    };
                    yield return token;
                } else if (input.IsEqualTo("extends") ||
                    input.IsEqualTo("importing") ||
                    input.IsEqualTo("compose") ||
                    input.IsEqualTo("loop") ||
                    input.IsEqualTo("pluck"))
                {
                    yield return TokenLexers.Word(input);
                }
                else if (input.IsEqualTo("->"))
                {
                    yield return TokenLexers.Operators(input, "->");
                }
                else if (input.IsEqualTo("::"))
                {
                    yield return TokenLexers.Operators(input, "::");
                }
                else if (!context && Char.IsLetter(input.Current))
                {
                    yield return TokenLexers.TakeUntillEndOfContext(input);
                    if (context)
                    {
                        yield return new Token() {
                            EndLine = input.Line,
                            EndColumn = 0,
                            StartLine = input.Line,
                            StartColumn = 0,
                            TokenType = TokenType.ContextEnded
                        };
                    }
                }
                else if (context && Char2.IsNewLine(input.Current) && TokenLexers.EndContext(input, 1))
                {
                    context = false;
                    yield return new Token() {
                        EndLine = input.Line,
                        EndColumn = 0,
                        StartLine = input.Line,
                        StartColumn = 0,
                        TokenType = TokenType.ContextEnded
                    };
                }
                else if (context && Char.IsNumber(input.Current))
                {
                    yield return TokenLexers.Number(input);
                }
                else if (context && Char.IsUpper(input.Current))
                {
                    yield return TokenLexers.Identifier(input);
                }
                else if (context && input.Current== '\'')
                {
                    yield return TokenLexers.GenericParameter(input);
                }
                else if (context && Char.IsLower(input.Current))
                {
                    yield return TokenLexers.Word(input);
                }
                else if (context && input.Current == '&')
                {
                    yield return TokenLexers.Take(input, TokenType.And);
                }
                else if (context && input.Current == '/')
                {
                    yield return TokenLexers.Pattern(input);
                }
                else if (context && input.Current == '"')
                {
                    yield return TokenLexers.String(input);
                }
                else if (context && char.IsNumber(input.Current))
                {
                    yield return TokenLexers.Number(input);
                }
                else if (input.Current == '{')
                {
                    yield return TokenLexers.Comment(input);
                }
                else if (input.Current == '#')
                {
                    yield return TokenLexers.Chapter(input);
                }
                else if (input.Current == '@')
                {
                    yield return TokenLexers.Annotation(input);
                }
                else if (input.Current == '%')
                {
                    yield return TokenLexers.Directive(input);
                }
                else if (input.Current == ';')
                {
                    yield return TokenLexers.Take(input, TokenType.EndStatement);
                }
                else if (Char2.Indent(input.Current))
                {
                    yield return TokenLexers.Take(input, TokenType.Indent);
                }
                else if (Char2.Equal(input.Current))
                {
                    yield return TokenLexers.Take(input, TokenType.Equal);
                }
                else if (Char2.Or(input.Current))
                {
                    yield return TokenLexers.Take(input, TokenType.Or);
                }
                else if (Char2.Separator(input.Current))
                {
                    yield return TokenLexers.Take(input, TokenType.Separator);
                }
                else if (input.Current == '(')
                {
                    yield return TokenLexers.Take(input, TokenType.GroupOpen);
                }
                else if (input.Current == ')')
                {
                    yield return TokenLexers.Take(input, TokenType.GroupClosed);
                }
                else if (input.Current == ',')
                {
                    yield return TokenLexers.Take(input, TokenType.ListSeparator);
                }
                else if (Char2.IsNewLine(input.Current))
                {
                    yield return TokenLexers.TakeNewLine(input);
                }
                else if (Char.IsWhiteSpace(input.Current))
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
                yield return new Token() {
                    EndLine = input.Line,
                    EndColumn = 0,
                    StartLine = input.Line,
                    StartColumn = 0,
                    TokenType = TokenType.ContextEnded
                };
                context = false;
            }

            yield return new Token() {
                EndLine = input.Line,
                EndColumn = 0,
                StartLine = input.Line,
                StartColumn = 0,
                TokenType = TokenType.EndOfFile
            };
        }


    }

}

