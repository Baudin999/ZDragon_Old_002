
using Xunit;
using Compiler;
using System.Linq;
using Compiler.AST;

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

        [Fact]
        public void OverrideFields()
        {
            var code = @"
type Person =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;

type Customer extends Person =
    LastName: Boolean;
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());


            var person = g.Find<ASTType>("Person");
            var customer = g.Find<ASTType>("Customer");

            Assert.Equal(person.Fields.Count(), customer.Fields.Count());
            Assert.Equal("Boolean", customer.Fields.Last().Types.First().Value);

        }

        [Fact]
        public void OverrideFieldRestrictions()
        {
            var code = @"
type Person =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;

type Customer extends Person =
    LastName: String
        & min 2
        & max 40;
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());


            var person = g.Find<ASTType>("Person");
            var customer = g.Find<ASTType>("Customer");
            var lastName = customer.Fields.First(f => f.Name == "LastName");
            var min = lastName.Restrictions.First(r => r.Key == "min");
            var max = lastName.Restrictions.First(r => r.Key == "max");

            Assert.Equal(person.Fields.Count(), customer.Fields.Count());
            Assert.Equal("String", lastName.Types.First().Value);
            Assert.Equal("2", min.Value);
            Assert.Equal("40", max.Value);

        }

        [Fact]
        public void CascadingExtensions()
        {
            var code = @"
type Person =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;

type Customer extends Person
type OtherCustomer extends Customer
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());


            var person = g.Find<ASTType>("Person");
            var customer = g.Find<ASTType>("Customer");
            var otherCustomer = g.Find<ASTType>("OtherCustomer");

            Assert.Equal(person.Fields.Count(), customer.Fields.Count());
            Assert.Equal(person.Fields.Count(), otherCustomer.Fields.Count());


        }

        [Fact]
        public void MultipleExtensions()
        {
            var code = @"
type Person =
    FirstName: String;
    LastName: String;
type DatabaseObject =
    Id: String;
type Customer extends Person DatabaseObject =
    CustomerNumber: Number;
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());

            var customer = g.Find<ASTType>("Customer");
            Assert.Equal(4, customer.Fields.Count());
        }

        [Fact]
        public void MultipleNestedExtensions()
        {
            var code = @"
type Person =
    FirstName: String;
    LastName: String;

type DatabaseObject extends Identity Auditable;

type Identity =
    Id: String;
type Auditable =
    CreatedOn: DateTime;
    CreatedBy: String;
    ModifiedOn: Maybe DateTime;
    ModifiedBy: Maybe String;

type Customer extends Person DatabaseObject =
    CustomerNumber: Number;
";
            var g = new ASTGenerator(code);

            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors.ToList());

            var customer = g.Find<ASTType>("Customer");
            Assert.Equal(8, customer.Fields.Count());
        }
    }
}
