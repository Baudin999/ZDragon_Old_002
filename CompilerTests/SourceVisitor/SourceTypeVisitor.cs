using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST;
using Xunit;
using Xunit.Abstractions;

namespace CompilerTests.SourceVisitor
{
    public class SourceTypeVisitor
    {
        private readonly ITestOutputHelper output;

        public SourceTypeVisitor(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void TestASTVisitor()
        {
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
type Person
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());

            Assert.Equal("type Person", result);
        }

        [Fact]
        public void TestAnnotationsSource()
        {
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
@ The Person
@ Another annotation
type Person
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());

            Assert.Equal(@"@ The Person
@ Another annotation
type Person", result);
        }


        [Fact]
        public void TestDirectivesSource()
        {
            Lexer lexer = new Lexer();
            var tokenStream = lexer.Lex(@"
% api: /root/{param}
type Person
");
            IParser parser = new Parser(tokenStream);
            IEnumerable<IASTNode> nodeTree = parser.Parse();

            VisitorSource visitor = new VisitorSource(nodeTree);
            string result = string.Join("\n\n", visitor.Start());

            Assert.Equal(@"% api: /root/{param}
type Person", result);
        }
    }
}
