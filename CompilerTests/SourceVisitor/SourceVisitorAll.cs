using System;
using Compiler;
using Xunit;

namespace CompilerTests.SourceVisitor
{
    public class SourceVisitorAll
    {
        [Fact]
        public void SimpleTest()
        {
            var code = @"
# Chapter

type Person =
    FirstName: String;

And a simple paragraph!
";
            var generator = new ASTGenerator(code);
            var sourceGenerator = new ASTGenerator("MyModule", generator.AST);
            Assert.Equal(code.Trim(), sourceGenerator.Code);
        }


        [Fact]
        public void SimpleAliasTest()
        {
            var code = @"
# Chapter

alias Name = String
    & min 12
    & max 34;

type Person =
    FirstName: String;

And a simple paragraph!
";
            var generator = new ASTGenerator(code);
            var sourceGenerator = new ASTGenerator("MyModule", generator.AST);
            Assert.Equal(code.Trim(), sourceGenerator.Code);
        }
    }
}
