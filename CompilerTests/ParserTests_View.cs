using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_View
    {
        [Fact]
        public void BasicViewTest()
        {
            var code = @"
type Person
type Customer

view SalesView =
    Person

";
            var generator = new ASTGenerator(code);
            Assert.NotNull(generator.AST);
            Assert.Equal(3, generator.AST.Count);
            Assert.True(generator.AST[2] is ASTView);
            Assert.Single((generator.AST[2] as ASTView).Nodes);
        }

        
    }
}
