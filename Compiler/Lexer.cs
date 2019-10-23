using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Lexer
    {

        private bool ignoreWhiteSpace = true;

        private string prepareSource(string source)
        {
            source = new Regex(@"\r\n|\n").Replace(source, "↓");
            source = new Regex(@"[\s\t]+↓").Replace(source, "↓");
            source = new Regex("    |\t").Replace(source, "→");
            Console.WriteLine(source);
            return source;
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
                    Console.WriteLine("Starting Context");
                    yield return TokenLexers.Word(input);
                }
                else if (!context && Char.IsLetter(input.Current()))
                {
                    yield return TokenLexers.TakeUntillEndOfContext(input);
                }
                else if (context && Char2.IsNewLine(input.Current()) && TokenLexers.EndContext(input))
                {
                    // do nothing but end Context...
                    Console.WriteLine("Ending Context " + input.Current().ToString());
                    context = false;
                }
                else if (context && Char.IsUpper(input.Current()))
                {
                    yield return TokenLexers.Identifier(input);
                }
                else if ( context && Char.IsLower(input.Current()))
                {
                    yield return TokenLexers.Word(input);
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
                    yield return TokenLexers.Take(input, TokenType.Ident);
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
                else if (Char.IsWhiteSpace(input.Current())) {
                    var whiteSpace = TokenLexers.Whitespace(input);
                    if (!ignoreWhiteSpace)
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
        }

        
    }

}

