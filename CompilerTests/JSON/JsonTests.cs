using Compiler;
using Configuration;
using Mapper.JSON;
using Xunit;

namespace CompilerTests.JSON
{
    public class JsonTests
    {
        private CarConfig CarConfig { get; }
        public JsonTests()
        {
            this.CarConfig = new CarConfig();
        }

        [Fact]
        public void TestJson()
        {
            var code = @"
alias Name = String
    & min 5
    & max 28

alias Names = List Name

type Address =
    Street: String;

% api: /person
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
    Address: Address;

";
            var generator = new ASTGenerator(code);
            var mapper = new JsonMapper(generator, this.CarConfig);
            mapper.Start();


            Assert.Single(mapper.Schemas);

        }


        [Fact]
        public void TestChoiceJson()
        {
            var code = @"


% api: /person
type Person =
    Gender: Gender;

choice Gender =
    | ""Male""
    | ""Female""
    | ""Other""
";
            var generator = new ASTGenerator(code);
            var mapper = new JsonMapper(generator, this.CarConfig);
            mapper.Start();


            Assert.Single(mapper.Schemas);

        }

        [Fact]
        public void TestDataJson()
        {
            var code = @"
type School =
    Name: String;

% api: /person
data Response =
    | Person
    | Animal
    | Customer
    | School
";
            var generator = new ASTGenerator(code);
            var mapper = new JsonMapper(generator, this.CarConfig);
            mapper.Start();


            Assert.Single(mapper.Schemas);

        }
    }
}