using System;
namespace CompilerTests
{
    public static class Examples
    {
        public static string Example1 = @"

# This is a chapter

And here we have a Paragraph. This is somthing we can try and
parse in a sensible way instead of 'per' token. We should filter
out the use of the word type.

@ This is a Person annotation.
type Person 'a =
    @ The first name
    FirstName: Name;
    Gender: Gender;

And another paragraph! These things should flow like the documentation
you are writing!

alias Name = String;


choice Gender =
    | ""Male""
    | ""Female""

    @ This is the same as non-binary
    | ""Other""

";
    }
}
