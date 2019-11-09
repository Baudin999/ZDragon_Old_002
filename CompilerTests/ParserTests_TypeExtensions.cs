using System;
using Xunit;
using Compiler;
using System.Linq;
using Compiler.AST;
using System.Collections.Generic;

namespace CompilerTests
{
    public class ParserTests_TypeExtensions
    {
        
        [Fact]
        public void SimpleExtension()
        {
            var code = @"
type Person =
    FirstName: String;

type Customer extends Person
";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());
        }


        [Fact]
        public void ExtendMultipleFields()
        {
            var code = @"
type Person =
    FirstName: String;

    @ The last name of the person
    LastName: String;

type Customer extends Person
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());

            var person = g.AST[0] as ASTType;
            var customer = g.AST[1] as ASTType;
            Assert.Equal(person.Fields.Count(), customer.Fields.Count());
            Assert.Equal(
                person.Fields.Last().Annotations.First().Value,
                customer.Fields.Last().Annotations.First().Value);
        }

        [Fact]
        public void RestrictionsOnExtension()
        {
            var code = @"
type Person =
    FirstName: String
        & min 2
        & max 23
    ;
    LastName: String;

type Customer extends Person
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());


            var person = g.AST[0] as ASTType;
            var customer = g.AST[1] as ASTType;

            Assert.Equal(person.Fields.Count(), customer.Fields.Count());
            Assert.Equal("min", customer.Fields.First().Restrictions.First().Key);
            Assert.Equal("2", customer.Fields.First().Restrictions.First().Value);
            Assert.Equal("max", customer.Fields.First().Restrictions.Last().Key);
            Assert.Equal("23", customer.Fields.First().Restrictions.Last().Value);
        }

        [Fact]
        public void ExtendPatternRestriction()
        {
            var code = @"
type Person =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;

type Customer extends Person
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());


            var person = g.AST[0] as ASTType;
            var customer = g.AST[1] as ASTType;

            Assert.Equal(person.Fields.Count(), customer.Fields.Count());
            Assert.Equal("pattern", customer.Fields.First().Restrictions.First().Key);
            Assert.Equal("/[A-Z][a-z]{2, 23}/", customer.Fields.First().Restrictions.First().Value);

        }
    }
}
