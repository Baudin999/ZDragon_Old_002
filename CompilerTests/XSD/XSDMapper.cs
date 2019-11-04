
using Compiler;
using Xunit;

namespace CompilerTests.XSD
{
    public class XSDMapper
    {
        [Fact]
        public void TransformationPipeline()
        {
            IPipeline pipeline = new Pipeline();
            Assert.NotNull(pipeline);

        }
    }
}
