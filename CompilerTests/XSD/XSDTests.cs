
using System.IO;
using System.Linq;
using System.Xml.Schema;
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


            TestXSD(mapper.Schema, "./XSD/CreateXSD.xsd");

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

            TestXSD(mapper.Schema, "./XSD/TestSimpleAlias.xsd");
        }

        [Fact]
        public void TestChoice()
        {

            var code = @"
alias Name = String;

choice Gender =
    | ""Male""
    | ""Female""
    | ""Other""

type Person =
    FirstName: Name;
    Gender: Gender;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            XSDMapper mapper = new XSDMapper(parseTree);
            _ = mapper.Start().ToList();

            TestXSD(mapper.Schema, "./XSD/TestChoice.xsd");
        }

        private XmlSchema LoadXSD(string path)
        {
            using var stream = new StreamReader(path);
            return XmlSchema.Read(stream, null);
        }

        private void TestXSD(XmlSchema source, string path)
        {
            TextWriter writer = new StringWriter();
            source.Write(writer);

            var xsd = LoadXSD(path);
            TextWriter resultWriter = new StringWriter();
            xsd.Write(resultWriter);

            Assert.Equal(writer.ToString(), resultWriter.ToString());
        }

    }

}

