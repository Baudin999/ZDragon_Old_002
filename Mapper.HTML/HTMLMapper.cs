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

        public string ToHtmlString(Dictionary<string, string> links)
        {

            return $@"
<!DOCTYPE html>
<html>
<head>
    <script src=""/mermaid.min.js""></script>
    <link rel='stylesheet' type='text/css' href='/style.css' />
  </head>
<body>

<a href='/index.html'>Home</a>

<ul>
<li><a href=""model.xsd"" alt=""XSD"">XSD</a></li>
{string.Join("\n", links.Select(l => $"<li><a href=\"{l.Value}\">{l.Key}</a></li>").ToList())}
</ul>

<div class=""mermaid"">{this.MermaidMapper.ToString()}
</div>

{ string.Join("\n\n", Parts)}

<script>
mermaid.initialize({{
    startOnLoad:true,
    classDiagram: {{
        useMaxWidth: false
    }}
}});

setTimeout(() => {{
    let svg = document.getElementsByTagName('svg')[0];
    let [a, b, width, height] = svg.attributes['viewBox'].value.split(' ');
    svg.attributes['width'].value = width;
    svg.attributes['height'].value = height;
}}, 30);

console.log(`
{this.MermaidMapper.ToString()}
`);
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
            var result = Markdown.ToHtml(astChapter.Content);
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

        public override string VisitASTFlow(ASTFlow astFlow)
        {
            var result = new MermaidFlowMapper(astFlow).ToString();
            this.Parts.Add(result);
            Console.WriteLine(result);
            return result;
        }

        public override string VisitASTParagraph(ASTParagraph astParagraph)
        {
            var result = Markdown.ToHtml(astParagraph.Content);
            Parts.Add(result);
            return result;
        }

        public override string VisitASTRestriction(ASTRestriction astRestriction)
        {
            return "";
        }

        public override string VisitASTType(ASTType astType)
        {
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
