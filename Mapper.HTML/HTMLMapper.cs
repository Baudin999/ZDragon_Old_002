using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Markdig;

namespace Mapper.HTML
{
    public class HtmlMapper : DefaultVisitor<string> //VisitorBase<string>
    {
        public List<string> Parts { get; } = new List<string>();
        public MermaidMapper MermaidMapper { get; }
        public HtmlTableMapper TableMapper { get; }

        public HtmlMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            this.MermaidMapper = new MermaidMapper(nodeTree);
            this.MermaidMapper.Start().ToList();
            this.TableMapper = new HtmlTableMapper(nodeTree);
        }


        public override string VisitASTChapter(ASTChapter astChapter)
        {
            var result = Markdown.ToHtml(astChapter.Content);
            Parts.Add(result);
            return result;
        }

       
        public override string VisitASTFlow(ASTFlow astFlow)
        {
            var result = new MermaidFlowMapper(astFlow).ToString();
            this.Parts.Add(result);
            return result;
        }

        public override string VisitASTParagraph(ASTParagraph astParagraph)
        {
            var result = Markdown.ToHtml(astParagraph.Content);
            Parts.Add(result);
            return result;
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

{ string.Join("\n\n", Parts)}

## ERD

<div class=""mermaid"">{this.MermaidMapper.ToString()}
</div>

## Tables

{ string.Join("\n\n", this.TableMapper.Start().ToList()) }

<script>
mermaid.initialize({{
    startOnLoad:true,
    classDiagram: {{
        useMaxWidth: false
    }},
    sequence: {{ actorMargin: 250 }}
}});

setTimeout(() => {{
    let svg = document.getElementsByTagName('svg')[0];
    let [a, b, width, height] = svg.attributes['viewBox'].value.split(' ');
    svg.attributes['width'].value = width;
    svg.attributes['height'].value = height;
}}, 30);

</script>

</body>
</html>

";
        }

    }
}
