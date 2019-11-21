using System;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_PluckedFields
    {
        [Fact]
        public void SimplePlucking()
        {
            var code = @"
type Person =
    FirstName: String;

type Customer =
    pluck Person.FirstName;
";

            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var person = (ASTType)g.AST.First();
            var customer = (ASTType)g.AST.Last();
            Assert.Single(person.Fields);
            Assert.Single(customer.Fields);

            var firstName = customer.Fields.First();
            Assert.Equal("FirstName", firstName.Name);
            Assert.Single(firstName.Type);
            Assert.Equal("String", firstName.Type.First().Value);
        }

        [Fact]
        public void PluckAnnotations()
        {
            var code = @"
type Person =
    @ The firstName of a person
    FirstName: String;

type Customer =
    pluck Person.FirstName;
";

            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var customer = (ASTType)g.AST.Last();
            var firstName = customer.Fields.First();
            Assert.Single(firstName.Annotations);
            Assert.Equal("The firstName of a person", firstName.Annotations.First().Value);
        }

        [Fact]
        public void PluckRestrictions()
        {
            var code = @"
type Person =
    @ The firstName of a person
    FirstName: String
        & min 12
        & max 32;

type Customer =
    pluck Person.FirstName;
";

            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var customer = (ASTType)g.AST.Last();
            var firstName = customer.Fields.First();
            Assert.Equal(2, firstName.Restrictions.Count());

            var _min = firstName.Restrictions.First();
            var _max = firstName.Restrictions.Last();

            Assert.Equal(12, int.Parse(_min.Value));
            Assert.Equal(32, int.Parse(_max.Value));
        }


        [Fact]
        public void PluckOverrideRestrictions()
        {
            var code = @"
type Person =
    @ The firstName of a person
    FirstName: String
        & min 12
        & max 32;

type Customer =
    pluck Person.FirstName
        & max 30;
";

            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var customer = (ASTType)g.AST.Last();
            var firstName = customer.Fields.First();
            Assert.Equal(2, firstName.Restrictions.Count());

            var _min = firstName.Restrictions.First();
            var _max = firstName.Restrictions.Last();

            Assert.Equal(12, int.Parse(_min.Value));
            Assert.Equal(30, int.Parse(_max.Value));
        }

        [Fact]
        public void PluckAddRestrictions()
        {
            var code = @"
type Person =
    @ The firstName of a person
    FirstName: String
        & min 12
        & max 32;

type Customer =
    pluck Person.FirstName
        & max 30
        & pattern /[A-Z][a-z]{29}/;
";

            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var person = (ASTType)g.AST.Last();
            

            var customer = (ASTType)g.AST.Last();
            var firstName = customer.Fields.First();
            Assert.Equal(3, firstName.Restrictions.Count());

            var restrictions = firstName.Restrictions.ToList();
            var _min = restrictions[0];
            var _max = restrictions[1];
            var _pattern = restrictions[2];

            Assert.Equal(12, int.Parse(_min.Value));
            Assert.Equal(30, int.Parse(_max.Value));
            Assert.Equal("/[A-Z][a-z]{29}/", _pattern.Value);
        }

        [Fact]
        public void PluckedWithExtension()
        {
            var code = @"
type Addressable =
    Addresses: List String;

type Person =
    @ The firstName of a person
    FirstName: String
        & min 12
        & max 32;

type Customer extends Person =
    pluck Addressable.Addresses;
";

            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());

            var customer = (ASTType)g.AST.Last();

            Assert.Equal(2, customer.Fields.Count());

            var addressable = customer.Fields.First();
            var firstName = customer.Fields.Last();

            Assert.Equal(2, firstName.Restrictions.Count());
            Assert.Empty(addressable.Restrictions);
        }
    }
}
