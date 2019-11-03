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
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
alias Name = String
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());

            Assert.Equal("alias Name = String", result);
        }

        [Fact]
        public void TestAliasAnnotations()
        {
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
@ This is the Name alias
alias Name = String;
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());
            string resultExpected = @"
@ This is the Name alias
alias Name = String
".Trim();

            Assert.Equal(resultExpected, result);
        }


        [Fact]
        public void TestAliasDirectivesAndAnnotations()
        {
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
@ This is the Name alias
% xsd: nnnNaname
alias Name = String;
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());
            string resultExpected = @"
@ This is the Name alias
% xsd: nnnNaname
alias Name = String
".Trim();

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void TestAliasRestrictions()
        {
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
@ This is the Name alias
alias Name =
    String & min 12 & max
    40
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());
            string resultExpected = @"
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
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
alias Name = String
    @ Minimum of 12
    & min 12
    @ Maximum of 40
    & max 40
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());
            string resultExpected = @"
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
