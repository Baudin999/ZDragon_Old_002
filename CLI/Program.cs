using System;
using Compiler;
using System.Collections.Generic;
using System.Linq;

namespace CLI
{
    class Program
    {
        static void Main()
        {
            Lexer lexer = new Lexer();
            List<Token> result = lexer.Lex(@"
type Person =
    FirstName: String;
").ToList();
            result.ForEach(Console.WriteLine);
        }
    }
}
