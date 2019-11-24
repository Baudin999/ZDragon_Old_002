using System;
using System.Linq;
using Compiler;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Data
    {
        [Fact(DisplayName = "Generic Data Type")]
        public void GenericDataType()
        {
            var code = @"
data Maybe 'a =
    | Just 'a
    | Nothing
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Errors);
            Assert.Equal(3, g.AST.Count());
        }


        [Fact]
        public void TestResolve()
        {
            var code = @"
data Maybe 'a =
    | Just 'a
    | Nothing
";
            
            var g = new ASTGenerator(code);

            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Errors);
            Assert.Equal(3, g.AST.Count);
        }

        [Fact]
        public void SimpleCustomerTest()
        {
            var code = @"
data Customer =
    | Person
    | Organisation
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Errors);
            Assert.Equal(3, g.AST.Count);
        }
    }
}
