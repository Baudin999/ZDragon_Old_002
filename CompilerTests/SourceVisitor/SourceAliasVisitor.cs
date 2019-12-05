using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST;
using Xunit;
using Xunit.Abstractions;

namespace CompilerTests.SourceVisitor
{
    public class SourceAliasVisitor
    {
        private readonly ITestOutputHelper output;

        public SourceAliasVisitor(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestASTVisitor()
        {
            var code = @"
alias Name = String
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join(Environment.NewLine + Environment.NewLine, visitor.Start());

            Assert.Equal("alias Name = String", result);
        }

        [Fact]
        public void TestAliasAnnotations()
        {
            var code = @"
@ This is the Name alias
alias Name = String;
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join(Environment.NewLine + Environment.NewLine, visitor.Start());
            var resultExpected = @"
@ This is the Name alias
alias Name = String
".Trim();

            Assert.Equal(resultExpected, result);
        }


        [Fact]
        public void TestAliasDirectivesAndAnnotations()
        {
            var code = @"
@ This is the Name alias
% xsd: nnnNaname
alias Name = String;
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join(Environment.NewLine + Environment.NewLine, visitor.Start());
            var resultExpected = @"
@ This is the Name alias
% xsd: nnnNaname
alias Name = String
".Trim();

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void TestAliasRestrictions()
        {
            var code = @"
@ This is the Name alias
alias Name =
    String & min 12 & max
    40
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join(Environment.NewLine + Environment.NewLine, visitor.Start());
            var resultExpected = @"
@ This is the Name alias
alias Name = String
    & min 12
    & max 40
".Trim();

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void TestAliasRestrictionAnnotations()
        {
            var code = @"
alias Name = String
    @ Minimum of 12
    & min 12
    @ Maximum of 40
    & max 40
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join(Environment.NewLine + Environment.NewLine, visitor.Start());
            var resultExpected = @"
alias Name = String

    @ Minimum of 12
    & min 12

    @ Maximum of 40
    & max 40
".Trim();

            Assert.Equal(resultExpected, result);
        }

    }
}
