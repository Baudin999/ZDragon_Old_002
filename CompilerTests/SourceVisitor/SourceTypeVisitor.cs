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
            var code = @"
type Person
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join("\n\n", visitor.Start());

            Assert.Equal("type Person", result);
        }

        [Fact]
        public void TestAnnotationsSource()
        {
            var code = @"
@ The Person
@ Another annotation
type Person
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join("\n\n", visitor.Start());

            Assert.Equal(@"@ The Person
@ Another annotation
type Person", result);
        }


        [Fact]
        public void TestDirectivesSource()
        {
            var code = @"
% api: /root/{param}
type Person
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join("\n\n", visitor.Start());
            Assert.Equal(@"
% api: /root/{param}
type Person".Trim(), result);
        }

        [Fact]
        public void TestMessedUpFieldSource()
        {
            var code = @"
type Person =
    FirstName:
        String ;
    LastName: String & min 12 & max
    30;
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join("\n\n", visitor.Start());
            var resultTest = @"
type Person =
    FirstName: String;
    LastName: String
        & min 12
        & max 30
    ;
".Trim();

            Assert.Equal(resultTest, result);
        }

        [Fact]
        public void TestFieldRestrictionAnnotations()
        {
            var code = @"
type Person =
    FirstName:
        String ;
    LastName: String & min 12
    @ annotation
    & max 30;
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join("\n\n", visitor.Start());
            var resultTest = @"
type Person =
    FirstName: String;
    LastName: String
        & min 12

        @ annotation
        & max 30
    ;
".Trim();

            Assert.Equal(resultTest, result);
        }

        [Fact]
        public void TestMultipleTypesFormatting()
        {
            var code = @"
type Person =
    FirstName: String;
    LastName: String
        & min 12
        @ annotation
        & max 30;
type School
type Other =
    Something: String;
";
            var generator = new ASTGenerator(code);
            var visitor = new VisitorSource(generator);
            var result = string.Join("\n\n", visitor.Start());
            var resultTest = @"
type Person =
    FirstName: String;
    LastName: String
        & min 12

        @ annotation
        & max 30
    ;

type School

type Other =
    Something: String;
".Trim();

            Assert.Equal(resultTest, result);
        }
    }
}
