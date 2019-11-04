using System;
using System.IO;
using System.Linq;
using Compiler;
using Mapper.XSD;
using Xunit;
using Xunit.Abstractions;

namespace CompilerTests.XSD
{
    public class XSDTests
    {
        private readonly ITestOutputHelper output;

        public XSDTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void CreateXSD()
        {

            var code = @"
alias Name = String
    & min 5
    & max 28

alias Names = List Name

type Person =
    @ The First Name of the Person
    FirstName: Name;
    LastName: Maybe String;
    Age: Number;
    Tags: List String
        & min 3
        & max 30
    ;
    Names: Names;

";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            XSDMapper mapper = new XSDMapper(parseTree);
            _ = mapper.Start().ToList();


            //Assert.NotNull(mapper);

            TextWriter writer = new StringWriter();
            mapper.Schema.Write(writer);
            this.output.WriteLine(writer.ToString());

        }

        [Fact]
        public void TestSimpleAlias()
        {

            var code = @"
alias Name = String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            XSDMapper mapper = new XSDMapper(parseTree);
            _ = mapper.Start().ToList();


            //var expectedResult = @"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<xs:schema xmlns:self=\"org.schema.something\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">\n  <xs:simpleType name=\"Name\">\n    <xs:restriction base=\"xs:string\">\n      <xs:minLength value=\"1\" />\n      <xs:maxLength value=\"100\" />\n    </xs:restriction>\n  </xs:simpleType>\n</xs:schema>";

            //TextWriter writer = new StringWriter();
            //mapper.Schema.Write(writer);

            //var txt = writer.ToString();

            //Assert.Equal(expectedResult, writer.ToString());

        }
    }
}
