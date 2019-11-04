using System;
namespace Compiler
{
    public class Pipeline : IPipeline
    {

        public Pipeline()
        {
        }

        public Pipeline AddStep(IPipelineStep pipelineStep)
        {
            return new Pipeline();
        }
    }

    public interface IPipelineStep
    {

    }
    public interface IPipeline
    {
        Pipeline AddStep(IPipelineStep pipelineStep);
    }
}
