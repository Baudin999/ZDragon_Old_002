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
            var result = lexer.Lex(@"

# This is a chapter

And here we have a Paragraph. This is somthing we can try and
parse in a sensible way instead of 'per' token. We should filter
out the use of the word type.

@ This is a Person annotation.
type Person a =
    @ The first name
    FirstName: String;

And another paragraph!

alias Name = String;

");
            foreach (var r in result)
            {
                Console.WriteLine(r);
            }
        }
    }
}
