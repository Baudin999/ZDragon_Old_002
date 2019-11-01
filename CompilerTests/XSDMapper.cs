using System;
using Mapper.XSD;
using Xunit;

namespace CompilerTests
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
