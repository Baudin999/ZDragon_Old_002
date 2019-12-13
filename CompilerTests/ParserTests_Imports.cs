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
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Prelude", import.ModuleName);
            Assert.Empty(import.Imports);
        }

        [Fact]
        public void ImportNested()
        {
            var code = @"
open Base.Person
";
            var g = new ASTGenerator(code);
            Assert.Empty(g.Errors);

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Base.Person", import.ModuleName);
            Assert.Empty(import.Imports);
        }

        [Fact]
        public void ImportNested4()
        {
            var code = @"
open Base.Person.Other.Something
";
            var g = new ASTGenerator(code);
            Assert.Empty(g.Errors);

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Base.Person.Other.Something", import.ModuleName);
            Assert.Empty(import.Imports);
        }

        [Fact]
        public void SingleImport()
        {
            var code = @"
open Customer importing (Customer)
";
            var g = new ASTGenerator(code);
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Customer", import.ModuleName);
            Assert.Single(import.Imports);
        }

        [Fact]
        public void SingleImportFromPath()
        {
            var code = @"
open Customer.Contract importing (Customer)
";
            var g = new ASTGenerator(code);
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Customer.Contract", import.ModuleName);
            Assert.Single(import.Imports);
            Assert.Equal("Customer", import.Imports.First());
        }


        [Fact]
        public void ImportAlist()
        {
            var code = @"
open Customer importing (Customer, Person, Product)
";
            var g = new ASTGenerator(code);
            Assert.Empty(g.Parser.Errors.ToList());

            var import = (ASTImport)g.AST[0];
            Assert.Equal("Customer", import.ModuleName);
            Assert.Equal(3, import.Imports.Count());
            Assert.Equal(new string[] { "Customer", "Person", "Product" }, import.Imports);
        }


        [Fact]
        public void ImportAndParseAST()
        {

            var baseCode = @"
type Address =
    Street: String;
type Person =
    FirstName: String;
";
            var _g = new ASTGenerator(baseCode, "Base");

            var code = @"
open Base;

type Student extends Person =
    pluck Address.Street;
    Registration: Number;
";
            var g = new ASTGenerator(code, "MyModule", _g.AST);
            Assert.Empty(g.Errors);
            //g.Resolve(_g.AST);

            var student = (ASTType)g.Find("Student");
            Assert.Equal("Student", student.Name);
            Assert.Equal(3, student.Fields.Count());

            var fields = student.Fields.ToList();
            var firstName = fields.Find(f => f.Name == "FirstName");
            var street = fields.Find(f => f.Name == "Street");
            var registration = fields.Find(f => f.Name == "Registration");

            Assert.Equal(FieldOrigin.Extended, firstName.Origin);
            Assert.Equal(FieldOrigin.Plucked, street.Origin);
            Assert.Equal(FieldOrigin.Original, registration.Origin);
        }

        [Fact]
        public void ImportOnlyUsedTypes_Extensions()
        {

            var baseCode = @"
type Address =
    Street: String;
type Person =
    FirstName: String;
";
            var _g = new ASTGenerator(baseCode, "Base");

            var code = @"
open Base;

type Student extends Person;
";
            var g = new ASTGenerator(code, "MyModule");
            g.Resolve(_g.AST);

            Assert.Empty(g.Errors);
            Assert.Equal(3, g.AST.Count);
        }

        [Fact]
        public void ImportOnlyUsedTypes_FieldResolution()
        {

            var baseCode = @"
type Address =
    Street: String;
type Person =
    FirstName: String;
    Address: Address;
";
            var _g = new ASTGenerator(baseCode, "Base");

            var code = @"
open Base;

type Student extends Person;
";
            var g = new ASTGenerator(code, "MyModule");
            g.Resolve(_g.AST);

            Assert.Empty(g.Errors);
            Assert.Equal(4, g.AST.Count);
        }
    }
}
