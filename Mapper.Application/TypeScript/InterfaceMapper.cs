using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Mapper.Application.TypeScript
{
    public class InterfaceMapper : VisitorDefault<string>
    {
        public List<string> done = new List<string>();
        public InterfaceMapper(ASTGenerator generator) : base(generator) { }


        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            var (first, last) = (astTypeField.Type.First().Value, astTypeField.Type.Last().Value);
            var _type = (first, last) switch
            {
                ("String", _) => "string",
                _ => "string"
            };

            return $@"    public {_type} {astTypeField.Name};";
        }

        public override string VisitASTType(ASTType astType)
        {
            if (!done.Contains(astType.Name))
            {
                return $@"
public interface {astType.Name} {{
{string.Join("\n", astType.Fields.Select(Visit).ToList())}
}}
";
            }
            return "";
        }
    }
}
