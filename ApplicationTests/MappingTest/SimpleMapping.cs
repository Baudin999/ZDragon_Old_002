using Compiler;
using Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ApplicationTests.MappingTest
{
    public class SimpleMapping
    {
        [Fact]
        public void NameMapper()
        {
            var code = @"
type Person =
    FirstName: Name;
alias Name = String;
";

            var g = new ASTGenerator(code);
            var nameMapper = new NameMapper();
            var names = nameMapper.Map(new NameVisitor(g));
            var result = nameMapper.Process();

            Assert.NotNull(result);
            Assert.Equal(@"Person
FirstName
Name", result);
        }
    }

    public class NameMapper : IMapper<string>
    {
        public IEnumerable<string> Result { get; private set; }

        public IEnumerable<string> Map<U>(VisitorBase<U> visitor) where U : IEnumerable<string>
        {
            this.Result = visitor.Start().SelectMany(i => i);
            return this.Result;
        }

        public string Process()
        {

            return String.Join(Environment.NewLine, Result);
        }
    }

    public class NameVisitor : VisitorDefault<IEnumerable<string>>
    {
        public NameVisitor(ASTGenerator generator) : base(generator) { }

        public override IEnumerable<string> VisitASTTypeField(ASTTypeField astTypeField)
        {
            yield return astTypeField.Name;
        }

        public override IEnumerable<string> VisitASTType(ASTType astType)
        {
            yield return astType.Name;
            foreach (var field in astType.Fields)
            {
                yield return Visit(field).First();
            }
        }

        public override IEnumerable<string> VisitASTAlias(ASTAlias astAlias)
        {
            yield return astAlias.Name;
        }
    }

    public interface IMapper<T>
    {
        IEnumerable<T> Result { get; }
        IEnumerable<T> Map<U>(VisitorBase<U> visitor) where U: IEnumerable<T>;
        string Process();
    }
}
