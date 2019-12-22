using System;
using System.Linq;
using Compiler;
using Configuration;
using Mapper.JSON;
using Newtonsoft.Json;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Recursive
    {
        private CarConfig CarConfig { get; }
        public ParserTests_Recursive()
        {
            this.CarConfig = new CarConfig();
        }

        [Fact(DisplayName = "Recursive Type")]
        public void RecursiveType()
        {
            var code = @"
type Person =
    Familly: List Person;
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Errors);
            Assert.Single(g.AST);
        }

        [Fact(DisplayName = "Recursive JSON schema")]
        public void RecursiveJSONSchema()
        {
            var code = @"
% api: /person
type Person =
    FirstName: String;
    LastName: Maybe String;
    Familly: List Person;
";
            var g = new ASTGenerator(code);
            var jsonMapper = new JsonMapper(g, this.CarConfig);
            jsonMapper.Start();
            var dict = jsonMapper.ToFileNameAndContentDict();


            dynamic result = JsonConvert.DeserializeObject(dict["Person.schema.json"]);


            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Errors);
            Assert.Single(g.AST);
        }
    }
}
