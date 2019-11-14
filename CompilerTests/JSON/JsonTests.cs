using Compiler;
using Mapper.JSON;
using Xunit;

namespace CompilerTests.JSON
{
    public class JsonTests
    {
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
            ASTGenerator generator = new ASTGenerator(code);
            JsonMapper mapper = new JsonMapper(generator.AST);
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
            ASTGenerator generator = new ASTGenerator(code);
            JsonMapper mapper = new JsonMapper(generator.AST);
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
            ASTGenerator generator = new ASTGenerator(code);
            JsonMapper mapper = new JsonMapper(generator.AST);
            mapper.Start();


            Assert.Single(mapper.Schemas);

        }
    }
}