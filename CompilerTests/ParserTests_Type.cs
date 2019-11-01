using System;
using Xunit;
using Compiler;
using System.Linq;
using Compiler.AST;
using System.Collections.Generic;

namespace CompilerTests
{
    public class ParserTests_Type
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

            List<IASTNode> list = parseTree.ToList();
            Assert.Single(list);

            ASTType t = list[0] as ASTType;
            Assert.Equal("Person", t.Name);
            Assert.Empty(t.Parameters);
            Assert.Single(t.Fields);


            ASTTypeField field = t.Fields.First();
            Assert.Single(field.Annotations);
            Assert.True(field.Annotations[0] is ASTAnnotation);
            Assert.Equal("The First Name of the Person", field.Annotations[0].Value);

            Assert.Equal("FirstName", field.Name);
            Assert.Equal(Helpers.ToTypeDefinition(new [] { "String" }), field.Type);
        }

        [Fact]
        public void MultipleFieldsParserTest()
        {
            var code = @"
type Person =
    @ The First Name of the Person
    @ A second Annotation is always cool to add
    FirstName: String;
    LastName: String;
    @ Age as a number is weird ofc!
    Age: Number;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse();
            Assert.NotNull(parseTree);

            List<IASTNode> list = parseTree.ToList();
            Assert.Single(list);

            ASTType t = list[0] as ASTType;
            Assert.Equal("Person", t.Name);
            Assert.Empty(t.Parameters);
            Assert.Equal(3, t.Fields.Count());


            List<ASTTypeField> fields = t.Fields.ToList();
            ASTTypeField field = fields.First();
            Assert.Equal(2, field.Annotations.Count);
            Assert.True(field.Annotations[0] is ASTAnnotation);
            Assert.Equal("The First Name of the Person", field.Annotations[0].Value);
            Assert.Equal("A second Annotation is always cool to add", field.Annotations[1].Value);

            Assert.Equal("FirstName", field.Name);
            Assert.Equal("String", field.Type[0].Value);


            ASTTypeField lastNameField = fields[1];
            Assert.Equal("LastName", lastNameField.Name);
            Assert.Equal("String", lastNameField.Type[0].Value);
            Assert.Empty(lastNameField.Annotations);

            ASTTypeField ageField = fields[2];
            Assert.Equal("Age", ageField.Name);
            Assert.Equal(new List<ASTTypeDefinition>() { new ASTTypeDefinition("Number") }, ageField.Type);
            Assert.Single(ageField.Annotations);
        }

        [Fact]
        public void MultipleTypes()
        {
            var code = @"
type Person =
    @ The First Name of the Person
    @ A second Annotation is always cool to add
    FirstName: String;
    LastName: String;
    @ Age as a number is weird ofc!
    Age: Number;
    Addresses: List;
type Address =
    Street: String;
    HouseNumber: Number;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Equal(2, parseTree.Count());

            ASTType personType = parseTree[0] as ASTType;
            Assert.Equal("Person", personType.Name);

            ASTType addressType = parseTree[1] as ASTType;
            Assert.Equal("Address", addressType.Name);

        }

        [Fact]
        public void TypeAnnotations()
        {
            var code = @"
@ The person type
type Person
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Single(parseTree);
            Assert.Single((parseTree[0] as ASTType).Annotations);
        }


        [Fact]
        public void NonBodyTypes()
        {
            var code = @"
type Person
type Address
type School =
    Name: String;
type Cohort
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Equal(4, parseTree.Count);
        }

        [Fact]
        public void TypeAnnotations2()
        {
            var code = @"
@ The person type
type Person =
    FirstName: String;
@ An address is a place
@ A second annotation line...
type Address
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Equal(2, parseTree.Count);
            Assert.Single((parseTree[0] as ASTType).Annotations);
            Assert.Equal(2, (parseTree[1] as ASTType).Annotations.Count());

        }

        [Fact]
        public void MultipleTypes2()
        {
            var code = @"
type Person =
    FirstName: String
        & min 2
        & max 30
    ;
@ The address
type Address =
    Street: String;
    HouseNumber: String;
type Company

@ A product is something you can sell to
@ a consumer or customer. Products can be
@ physical, financial or services.
@ Pricing can be recurring or one off, prices
@ can very depending on the amount you buy.
type Product =
    Name: String
        & min 1

        @ Product Name max length
        & max 50
    ;
    Description: String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Equal(4, parseTree.Count);
        }

        [Fact]
        public void FieldRestrictions()
        {
            var code = @"
type Person =
    FirstName: String
        & min 2
        & max 30
    ;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Single(parseTree);
            ASTType t = (ASTType)parseTree[0];
            Assert.Single(t.Fields);
            Assert.Equal(2, t.Fields.First().Restrictions.Count);
        }

        [Fact]
        public void FieldRestrictionsAnnotations()
        {
            var code = @"
type Person =
    FirstName: String
        & min 2

        @ Should not be 30
        & max 30
    ;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);

            Assert.Single(parseTree);
            ASTType t = (ASTType)parseTree[0];
            Assert.Single(t.Fields);
            Assert.Equal(2, t.Fields.First().Restrictions.Count);

            ASTTypeField field = t.Fields.First();
            Assert.Equal(2, field.Restrictions.Count);
            Assert.Equal("Should not be 30", field.Restrictions[1].Annotations.First().Value);
        }


        [Fact]
        public void DirectivesHaveKeyAndValue()
        {
            var code = @"
% error
% api: /some/url/{param}
type Person
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parser.Errors.ToList());

            var s = parser.Errors.First();

            ASTType t = parseTree[0] as ASTType;
            Assert.Equal(2, t.Directives.Count());
        }
    }


    public static class Helpers
    {
        public static List<ASTTypeDefinition> ToTypeDefinition(string[] parameters)
        {
            return parameters.Select(p => new ASTTypeDefinition(p)).ToList();
        }
    }
}
