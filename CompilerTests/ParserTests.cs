using System;
using Xunit;
using Compiler;
using System.Linq;
using Compiler.AST;
using System.Collections.Generic;

namespace CompilerTests
{
    public class ParserTests
    {
        [Fact]
        public void BasicParserTest()
        {
            var code = @"
type Person =
    @ The First Name of the Person
    FirstName: String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse();
            Assert.NotNull(parseTree);

            List<object> list = parseTree.ToList();
            Assert.Equal(1, list.Count());

            ASTType t = list[0] as ASTType;
            Assert.Equal("Person", t.Name);
            Assert.Equal(0, t.Parameters.Count);
            Assert.Equal(1, t.Fields.Count);


            ASTTypeField field = t.Fields[0];
            Assert.Equal(1, field.Annotations.Count);
            Assert.True(field.Annotations[0] is ASTAnnotation);
            Assert.Equal("@ The First Name of the Person", field.Annotations[0].Value);

            Assert.Equal("FirstName", field.Name);
        }
    }
}
