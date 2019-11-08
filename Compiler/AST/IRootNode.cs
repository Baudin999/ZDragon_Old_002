using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public interface IRootNode
    {
        public IEnumerable<ASTDirective> Directives { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
    }
}
