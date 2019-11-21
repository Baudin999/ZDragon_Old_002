using System;
using Compiler;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Flows
    {

        [Fact]
        public void CreateFlow()
        {
            var code = @"
type Person
";
            var g = new ASTGenerator(code);
            Assert.Single(g.AST);
        }

    }
}
