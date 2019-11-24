using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Mapper.HTML
{
    public class MermaidFlowMapper
    {
        public ASTFlow Flow { get; }
        public MermaidFlowMapper(ASTFlow flow)
        {
            this.Flow = flow;
        }

        private string t(ASTTypeDefinition def) => def.Value;
        private string j(IEnumerable<ASTTypeDefinition> defs) => string.Join(" ", defs.Select(t).ToList());

        public string StepToString(ASTFlowStep step)
        {
            var result = step.Parameters.Last();
            var from = string.Join(" -> ", step.Parameters.SkipLast(1).Select(d => j(d.Types)).ToList());
            return $@"
    {step.From} ->> {step.To} : {from}
    {step.To} -->> {step.From} : {string.Join(" ", result.Types.Select(t => t.Value))}
".Trim();
        }

        public override string ToString() {
            return $@"
<div class=""mermaid"">sequenceDiagram
{string.Join("\n", this.Flow.Steps.Select(StepToString).ToList())}
</div>".Trim();
        }
    }
}
