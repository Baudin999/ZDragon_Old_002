using System;
using System.Linq;
using CLI;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Imports
    {
        [Fact]
        public void ImportAll()
        {
            var code = @"
open Prelude
";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Prelude", import.Name);
            Assert.Empty(import.Imports);
        }

        [Fact]
        public void SingleImport()
        {
            var code = @"
open Customer importing (Customer)
";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Customer", import.Name);
            Assert.Single(import.Imports);
        }


        [Fact]
        public void ImportAlist()
        {
            var code = @"
open Customer importing (Customer, Person, Product)
";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Customer", import.Name);
            Assert.Equal(3, import.Imports.Count());
            Assert.Equal(new string[] { "Customer", "Person", "Product" }, import.Imports);
        }


        [Fact]
        public void ImportAndParseAST()
        {
            
            var baseCode = @"
type Person =
    FirstName: String;
";
            var _g = new ASTGenerator(baseCode);
            
            var code = @"
open Sample;

type Student extends Person;
";
            var g = new ASTGenerator(code);
            
            g.Resolve(_g.AST);


            var student = (ASTType)g.AST[1];
            Assert.Equal("Student", student.Name);
            Assert.Single(student.Fields);
        }
    }
}
