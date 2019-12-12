using System;
using System.Linq;
using Compiler;
using Mapper.Application.CSharp;
using Xunit;

namespace ApplicationTests.CSharp
{
    public class ClassTest
    {
        [Fact]
        public void BasicClass()
        {
            var code = @"

alias Name = String;

type Person =
    FirstName: Name;
    LastName: String;
    
";

            var generator = new ASTGenerator(code, "MyModule");
            Assert.Empty(generator.Errors);

            var csharpVisitor = new CSharpVisitor(generator);
            csharpVisitor.Start().ToList();
            var cscode = csharpVisitor.ToString();

            Assert.NotNull(cscode);
        }
    }
}
