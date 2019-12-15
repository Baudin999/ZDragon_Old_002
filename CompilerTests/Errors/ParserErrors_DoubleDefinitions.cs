using System;
using System.Linq;
using Compiler;
using Xunit;

namespace CompilerTests.Errors
{

    public class ParserErrors_DoubleDefinitions
    {
        [Fact(DisplayName = "Type Already Exists")]
        public void TypeAlreadyExists()
        {
            var code = @"
type Person

type Person
";
            var g = new ASTGenerator(code);
            Assert.Single(g.Errors);

            var error = g.Errors.First();
        }

        [Fact(DisplayName = "Type Already Exists 02")]
        public void TypeAlreadyExists02()
        {
            var code = @"
type Person =
    FirstName: String;

type Person =
    LastName: String;
";
            var g = new ASTGenerator(code);
            Assert.Single(g.Errors);
        }

        [Fact(DisplayName = "Type Already Exists 03")]
        public void TypeAlreadyExists03()
        {
            var code = @"
alias Person = String;

type Person =
    LastName: String;
";
            var g = new ASTGenerator(code);
            Assert.Single(g.Errors);
        }

        [Fact(DisplayName = "Type Already Exists 04")]
        public void TypeAlreadyExists04()
        {
            var code = @"
type Person =
    FirstName: String;

data Person =
    | Person
    | Other
";
            var g = new ASTGenerator(code);
            Assert.Single(g.Errors);
        }

        [Fact(DisplayName = "Type Already Exists 05")]
        public void TypeAlreadyExists05()
        {
            var code = @"
type Person 'a =
    FirstName: 'a;

alias Person = Person String;
";
            var g = new ASTGenerator(code);
            Assert.Single(g.Errors);
        }
    }
}
