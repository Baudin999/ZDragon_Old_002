using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Markdig;

namespace Mapper.HTML
{
    public class HtmlMapper : VisitorBase<string>
    {
        public List<string> Parts { get; } = new List<string>();
        public MermaidMapper MermaidMapper { get; }

        public HtmlMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            this.MermaidMapper = new MermaidMapper(nodeTree);
            this.MermaidMapper.Start().ToList();
        }

        public override string ToString()
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/mermaid/8.3.1/mermaid.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/dagre/0.8.4/dagre.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/nomnoml/0.6.1/nomnoml.js""></script>
</head>
<body>

<a href=""model.xsd"" alt=""XSD"">XSD</a>

<div class=""mermaid"">{this.MermaidMapper.ToString()}
</div>

{ string.Join("\n\n", Parts)}

<script>
mermaid.initialize({{
    securityLevel: ""strict"",
    startOnLoad:true,
    theme: ""default"",
    classDiagram: {{
        useMaxWidth: false
    }}
}});
</script>

</body>
</html>

";
        }


        public override string VisitASTAlias(ASTAlias astAlias)
        {
            return "";
        }

        public override string VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            return "";
        }

        public override string VisitASTChapter(ASTChapter astChapter)
        {
            string result = Markdown.ToHtml(astChapter.Content);
            Parts.Add(result);
            return result;
        }

        public override string VisitASTChoice(ASTChoice astChoice)
        {
            return "";
        }

        public override string VisitASTDirective(ASTDirective astDirective)
        {
            return "";
        }

        public override string VisitASTOption(ASTOption astOption)
        {
            return "";
        }

        public override string VisitASTParagraph(ASTParagraph astParagraph)
        {
            string result = Markdown.ToHtml(astParagraph.Content);
            Parts.Add(result);
            return result;
        }

        public override string VisitASTRestriction(ASTRestriction astRestriction)
        {
            return "";
        }

        public override string VisitASTType(ASTType astType)
        {
//            var extensions = astType.Extensions.Select(e => $"[{astType.Name}] -:> [{e}]");
//            var fields = astType.Fields.Select(f => $@"    {f.Name}: {string.Join(" ", f.Type.Select(t => t.Value))}").ToList();
//            var fieldReferences = astType.Fields.Select(f =>
//            {
//                var _type = f.Type.Last().Value;
//                if (_type != "String" && _type != "Number" && _type != f.Name)
//                {
//                    return $@"[{f.Name}] -> [{_type}]";
//                } else
//                {
//                    return "";
//                }
//            });
//            var typeReferences = astType.Fields.Select(f =>
//            {
//                var _type = f.Type.Last().Value;
//                if (_type != "String" && _type != "Number")
//                {
//                    return $@"[{astType.Name}] -> [{_type}]";
//                }
//                else
//                {
//                    return "";
//                }
//            });
//            ErdParts.Add($@"
//[{astType.Name} |
//{string.Join("\n", fields)} |
//]
//{string.Join("\n", typeReferences)}
//{string.Join("\n", extensions)}
//"
//);
            return "";
        }

        public override string VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            return "";
        }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            return "";
        }

        public override string VisitDefault(IASTNode node)
        {
            return "";
        }

        public override string VisitASTData(ASTData astData)
        {
            //ErdParts
            return "";
        }
    }
}
