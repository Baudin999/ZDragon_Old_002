using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST;
using Xunit;
using Xunit.Abstractions;

namespace CompilerTests
{
    public class VisitorTests
    {


        [Fact]
        public void TestASTVisitor()
        {
            var code = @"
type Person
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join(Environment.NewLine + Environment.NewLine, visitor.Start());

            Assert.True(result.Length > 0);
        }
    }
}
