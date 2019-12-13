using System;
using Compiler;

namespace Mapper.Application.Topology
{
    public class TopologyMapper
    {
        public ASTGenerator Generator { get; }

        public TopologyMapper(ASTGenerator generator)
        {
            this.Generator = generator;
        }

    }
}
