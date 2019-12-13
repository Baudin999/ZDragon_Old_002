
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
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
            _ = mapper.Start().ToList();


            TestXSD(mapper.Schema, "./XSD/CreateXSD.xsd");

        }

        [Fact]
        public void TestSimpleAlias()
        {

            var code = @"
alias Name = String;
";
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
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
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
            _ = mapper.Start().ToList();

            TestXSD(mapper.Schema, "./XSD/TestChoice.xsd");
        }

        [Fact]
        public void TestDateXSD()
        {
            var code = @"

alias Then = Date;

type Start =
    Now: Date;
    Something: Then;
";
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
            _ = mapper.Start().ToList();

            TestXSD(mapper.Schema, "./XSD/TestDateXSD.xsd");
        }

        [Fact]
        public void TestBooleanXSD()
        {
            var code = @"

alias Naha = Boolean;

type Start =
    Yup: Boolean;
    Something: Naha;
";
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
            _ = mapper.Start().ToList();

            TestXSD(mapper.Schema, "./XSD/TestBooleanXSD.xsd");
        }

        [Fact]
        public void TestPluckedFieldsXSD()
        {
            var code = @"
type Addressable =
    Addresses: List String;

type Person =
    @ The firstName of a person
    FirstName: String
        & min 12
        & max 32;

type Customer extends Person =
    pluck Addressable.Addresses;
";
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
            _ = mapper.Start().ToList();

            TestXSD(mapper.Schema, "./XSD/TestPluckedFieldsXSD.xsd");
        }

        [Fact]
        public void TestEveryFieldType()
        {
            var code = @"
type Root =
    F1: Number;
    F2: String;
    F3: Boolean;
    F4: Date;
    F5: Time;
    F6: DateTime;
    F7: Maybe Number;
    F8: Maybe String;
    F9: Maybe Boolean;
    F10: Maybe Date;
    F11: Maybe Time;
    F12: Maybe DateTime;
    F13: List Number;
    F14: List String;
    F15: List Boolean;
    F16: List Date;
    F17: List Time;
    F18: List DateTime;
";
            var generator = new ASTGenerator(code);
            var mapper = new XSDMapper(generator);
            _ = mapper.Start().ToList();

            TestXSD(mapper.Schema, "./XSD/TestEveryFieldType.xsd");
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

            Assert.Equal(resultWriter.ToString(), writer.ToString());
        }



    }

}

