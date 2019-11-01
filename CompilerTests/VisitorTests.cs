using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class VisitorTests
    {
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
            Console.WriteLine(visitor);
        }
    }
}
