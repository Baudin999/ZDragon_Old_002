using System;
using System.Linq;
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
        public void ImportAList()
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
    }
}
