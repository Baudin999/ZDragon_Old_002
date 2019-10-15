using System;
using Compiler;
using System.Collections.Generic;
using System.Linq;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer();
            var result = lexer.Lex("type Person = Foo;");
            foreach (var r in result)
            {
                Console.WriteLine(r);
            }
        }
    }
}
