using System;
using Compiler;
using Xunit;

namespace CompilerTests.Errors
{
    public class ParserErrors_CircularReference
    {
        [Fact(DisplayName = "Circular Reference")]
        public void CircularReference()
        {
            var code = @"
type Person =
    Student: Student;

type Student =
    Person: Person;
";
            var g = new ASTGenerator(code);
            //Assert.Single(g.Errors);
        }
    }
}
