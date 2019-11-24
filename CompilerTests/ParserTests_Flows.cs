using System;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Flows
    {

        [Fact]
        public void SimpelFlows()
        {
            var code = @"
flow ""Get Student"" =
    API -> Database :: String -> String;


";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());


            var flow = (ASTFlow)g.AST[0];
            Assert.Equal("Get Student", flow.Name);
            Assert.Single(flow.Steps);

            var step = (ASTFlowStep)flow.Steps.First();
            Assert.Equal("API", step.From);
            Assert.Equal("Database", step.To);

            var param1 = step.Parameters.First();
            Assert.Single(param1.Types);
            Assert.Equal("String", param1.Types.First().Value);

            var param2 = step.Parameters.Last();
            Assert.Single(param2.Types);
            Assert.Equal("String", param2.Types.First().Value);
        }

        [Fact]
        public void Example01()
        {
            var code = @"
flow GetStudents =
    API -> Database :: FilterParams -> List Student;


";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());


            var flow = (ASTFlow)g.AST[0];
            Assert.Equal("GetStudents", flow.Name);
            Assert.Single(flow.Steps);

            var step = (ASTFlowStep)flow.Steps.First();
            Assert.Equal("API", step.From);
            Assert.Equal("Database", step.To);

            var param1 = step.Parameters.First();
            Assert.Single(param1.Types);
            Assert.Equal("FilterParams", param1.Types.First().Value);

            var param2 = step.Parameters.Last();
            Assert.Equal(2, param2.Types.Count());
            Assert.Equal("List", param2.Types.First().Value);
            Assert.Equal("Student", param2.Types.Last().Value);
        }

        [Fact]
        public void MultipleFlows()
        {
            var code = @"
flow GetStudents =
    API -> Service :: FilterParams -> List Student;
    Service -> Database :: FilterParams -> List Student;

    API -> ""Some Database"" :: String -> List Number;

";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.ParseTree);
            Assert.Empty(g.Parser.Errors.ToList());


            var flow = (ASTFlow)g.AST[0];
            Assert.Equal("GetStudents", flow.Name);
            Assert.Equal(3, flow.Steps.Count());

            Assert.Equal("Some Database", ((ASTFlowStep)flow.Steps.Last()).To);
        }


        [Fact(DisplayName = "Flow Composition 01")]
        public void FlowComposition01()
        {
            var code = @"
flow GetStudents =

    compose
        API -> Service :: FilterParams -> List Student;
        Service -> Database :: FilterParams -> List Student;
    ;

    API -> ""Some Database"" :: String -> List Number;

";
            var g = new ASTGenerator(code);
            var flow = (ASTFlow)g.AST[0];

            Assert.Equal(2, flow.Steps.Count());
        }
    }

}

