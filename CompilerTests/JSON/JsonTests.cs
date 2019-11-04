using System;
using Xunit;
using Mapper.JSON;
using Compiler;
using System.Linq;

namespace CompilerTests.JSON
{
    public class JsonTests
    {
        [Fact]
        public void TestJson()
        {
            var code = @"
alias Name = String
    & min 5
    & max 28

alias Names = List Name

type Person =
    @ The First Name of the Person
    FirstName: Name;
    LastName: Maybe String;
    Age: Number;
    Tags: List String
        & min 3
        & max 30
    ;
    Names: Names;

";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();

            JsonMapper c = new JsonMapper(parseTree);
        }
    }
    
}
