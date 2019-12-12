using System;
using System.Linq;
using Compiler;
using Mapper.JSON;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Recursive
    {
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

//        [Fact(DisplayName = "Recursive JSON schema")]
//        public void RecursiveJSONSchema()
//        {
//            var code = @"
//% api: /person
//type Person =
//    Familly: List Person;
//";
//            var g = new ASTGenerator(code);
//            var jsonMapper = new JsonMapper(g);
//            jsonMapper.Start();
//            var dict = jsonMapper.ToFileNameAndContentDict();

//            Assert.NotNull(g.ParseTree);
//            Assert.Empty(g.Errors);
//            Assert.Single(g.AST);
//        }
    }
}
