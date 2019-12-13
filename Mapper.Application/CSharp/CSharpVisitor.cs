using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Mapper.Application.CSharp
{
    public class CSharpVisitor : VisitorDefault<string>
    {
        private string Namespace { get; }
        public List<string> Parts = new List<string>();
        public CSharpVisitor(ASTGenerator generator) : base(generator) {
            Namespace = generator.ModuleName;
        }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            var t = (astTypeField.Type.First().Value, astTypeField.Type.Last().Value);
            return $@"    public {CSharpHelpers.ToCSharpKeyword(t)} {astTypeField.Name} {{ get; set; }}";
        }

        public override string VisitASTType(ASTType astType)
        {
            return Intercept($@"
public class {astType.Name} {{
{string.Join(Environment.NewLine, astType.Fields.Select(Visit))}
}}
");
        }

        public override string VisitASTAlias(ASTAlias astAlias)
        {
            var t = (astAlias.Type.First().Value, astAlias.Type.Last().Value);
            return Intercept($@"using {astAlias.Name} = {CSharpHelpers.ToCSharpType(t)};");
        }

        private string Intercept(string s)
        {
            this.Parts.Add(s);
            return s;
        }


        public override string ToString()
        {
            return $@"
using System;
using System.Collections.Generic;

namespace {Namespace} {{
{string.Join(Environment.NewLine + Environment.NewLine, Parts)}
}}
";
        }
    }

    public class CSharpHelpers
    {
        public static string ToCSharpKeyword ((string, string) types)
        {
            var (a, b) = types;
            return types switch
            {
                ("List", _) => $"List<{ToCSharpType((b, ""))}>",
                ("Maybe", _) => $"{ToCSharpType((b, ""))}?",
                ("String", _) => "string",
                ("Number", _) => "int",
                ("Boolean", _) => "bool",
                ("Date", _) => "DateTime",
                ("Time", _) => "DateTime",
                ("DateTime", _) => "DateTime",
                _ => a
            };
        }

        public static string ToCSharpType((string, string) types)
        {
            var (a, b) = types;
            return types switch
            {
                ("List", _) => $"List<{ToCSharpType((b, ""))}>",
                ("Maybe", _) => $"{ToCSharpType((b, ""))}?",
                ("String", _) => "String",
                ("Number", _) => "Int32",
                ("Boolean", _) => "Boolean",
                ("Date", _) => "DateTime",
                ("Time", _) => "DateTime",
                ("DateTime", _) => "DateTime",
                _ => a
            };
        }
    }
}
