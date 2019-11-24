using System;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Generics
    {
        [Fact(DisplayName = "Can define a single generic parameter")]
        public void CanDefineSingleGenericParameter ()
        {
            var code = @"
type Foo 'a
";
            var g = new ASTGenerator(code);
            var t = (ASTType)g.AST[0];

            Assert.Single(t.Parameters);
        }

        [Fact(DisplayName = "Can define multiple generic parameters")]
        public void MultipleGenericParameters()
        {
            var code = @"
type Foo 'a 'b 'c 'd
";
            var g = new ASTGenerator(code);
            var t = (ASTType)g.AST[0];

            Assert.Equal(4, t.Parameters.Count());
        }

        [Fact(DisplayName = "Can assign a generic type parameter")]
        public void CanAssignAGenericTypeParameter()
        {
            var code = @"
type Foo 'a =
    Base: 'a;
";
            var g = new ASTGenerator(code);
            var t = (ASTType)g.AST[0];
            var field = t.Fields.First();
            var fieldType = field.Type.First().Value;

            Assert.Single(t.Parameters);
            Assert.Equal("'a", fieldType);
        }

        [Fact(DisplayName = "Resolve simple generic type")]
        public void ResolveSimple()
        {
            var code = @"
type Foo 'a =
    Base: 'a;

alias ConcreteFoo = Foo String;
";
            var g = new ASTGenerator(code);
            Assert.Equal(2, g.AST.Count);
            var t = (ASTType)g.AST[1];
            Assert.Equal("ConcreteFoo", t.Name);
            Assert.NotNull(t);
            Assert.Single(t.Fields);
            Assert.Equal("String", t.Fields.First().Type.First().Value);
        }

        [Fact(DisplayName = "Resolve simple generic type 02")]
        public void ResolveSimple02()
        {
            var code = @"
type Foo 'a 'b =
    Base1: 'a;
    Base2: 'b;
    Base3: 'a;

alias ConcreteFoo = Foo String Number;
";
            var g = new ASTGenerator(code);
            Assert.Equal(2, g.AST.Count);
            var t = (ASTType)g.AST[1];
            Assert.Equal(3, t.Fields.Count());


            var fields = t.Fields.ToList();
            var base1 = fields[0];
            var base2 = fields[1];
            var base3 = fields[2];

            
            Assert.Equal("String", base1.Type.First().Value);
            Assert.Equal("Number", base2.Type.First().Value);
            Assert.Equal("String", base3.Type.First().Value);
        }
    }
}
