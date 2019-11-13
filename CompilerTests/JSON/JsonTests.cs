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
    }
    
}
