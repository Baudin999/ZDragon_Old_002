using System;
using System.Linq;
using Compiler;
using Mapper.HTML;
using Xunit;

namespace CompilerTests.HTML
{
    public class HtmlTests
    {
        [Fact]
        public void TestHtml()
        {
            var code = @"
# Person things

Now this is a story all about how
my life got flipped turned upside down
and I'd like to take a minute just sit right there
I'll tell you how I become the prince of a town called Bel Air.


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

Points to look out for:

* Item 1
* Item 2
    * Other 1
    * Other 2
* Item 3

";
            var generator = new ASTGenerator(code);
            var mapper = new HtmlMapper(generator);
            _ = mapper.Start().ToList();

            var document = mapper.ToString();
            Assert.True(document.Any());
        }

        [Fact]
        public void TestList()
        {
            var code = @"

* Item 1
* Item 2
    * Other 1
    * Other 2
* Item 3

";
            var generator = new ASTGenerator(code);
            var mapper = new HtmlMapper(generator);
            _ = mapper.Start().ToList();

            var document = mapper.ToString();

            Assert.True(document.Any());

        }
    }
}
