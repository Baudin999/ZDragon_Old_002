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
        public List<string> ErdParts { get; } = new List<string>();

        public HtmlMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            
        }

        public override string ToString()
        {
            //System.Xml.Linq.XElement.Parse(
            return $@"
<!DOCTYPE html>
<html>
<head>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/dagre/0.8.4/dagre.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/nomnoml/0.6.1/nomnoml.js""></script>
</head>
<body>
{string.Join("\n\n", Parts)}
<canvas id=""erd_canvas""></canvas>

<script>
    var canvas = document.getElementById('erd_canvas');
    var source = `
{string.Join("\n\n", ErdParts)}
`;
    nomnoml.draw(canvas, source);
</script>

</body>
</html>

";
        }


        public override string VisitASTAlias(ASTAlias astAlias)
        {
            ErdParts.Add($"[<abstract>{astAlias.Name}]");
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
            var fields = astType.Fields.Select(f => $@"    {f.Name}: {string.Join(" ", f.Type.Select(t => t.Value))}").ToList();
            var fieldReferences = astType.Fields.Select(f =>
            {
                var _type = f.Type.Last().Value;
                if (_type != "String" && _type != "Number" && _type != f.Name)
                {
                    return $@"[{f.Name}] -> [{_type}]";
                } else
                {
                    return "";
                }
            });
            var typeReferences = astType.Fields.Select(f =>
            {
                var _type = f.Type.Last().Value;
                if (_type != "String" && _type != "Number")
                {
                    return $@"[{astType.Name}] -> [{_type}]";
                }
                else
                {
                    return "";
                }
            });
            //{string.Join("\n", fieldReferences)}
            ErdParts.Add($@"
[{astType.Name} |
{string.Join("\n", fields)} |
]
{string.Join("\n", typeReferences)}
"
);
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
    }
}
