using System;
using Compiler;
using System.Collections.Generic;
using System.Linq;

namespace CLI
{
    class Program
    {

        static string Example1 = @"

# This is a chapter

And here we have a Paragraph. This is somthing we can try and
parse in a sensible way instead of 'per' token. We should filter
out the use of the word type.

@ This is a Person annotation.
type Person a =
    @ The first name
    FirstName: Name = Name ""Carlos"";
    Gender: Gender;
    Age: Number = 0.75;

And another paragraph! These things should flow like the documentation
you are writing!

alias Name = String;


choice Gender =
    | ""Male""
    | ""Female""

    @ This is the same as non-binary
    | ""Other""

";

        static void Main(string[] args)
        {
            var lexer = new Lexer();
            var result = lexer.Lex(Example1);
            foreach (var r in result)
            {
                Console.WriteLine(r);
            }
        }
    }
}
