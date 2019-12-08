using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Markdig;

namespace Mapper.HTML
{
    public class HtmlMapper : VisitorDefault<string>
    {
        public List<string> Parts { get; } = new List<string>();
        public IEnumerable<IASTError> Errors { get; }
        public MermaidMapper MermaidMapper { get; }
        public HtmlTableMapper TableMapper { get; }

        public HtmlMapper(ASTGenerator generator) : base(generator)
        {
            this.Errors = generator.Errors;
            this.MermaidMapper = new MermaidMapper(generator);
            this.MermaidMapper.Start().ToList();
            this.TableMapper = new HtmlTableMapper(generator);
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

        public override string VisitASTView(ASTView astView)
        {
            var ast = astView.Nodes.Select(Generator.Find).ToList();
            var mermaidMapper = new MermaidMapper(new ASTGenerator(ast));
            mermaidMapper.Start().ToList();
            var mermaidString = mermaidMapper.ToString().Trim();
            var result = $@"<div class=""svg-container""><div class=""mermaid"">{mermaidString}</div></div>";
            Parts.Add(result);
            return result;
        }


        public string ToHtmlString(Dictionary<string, string> links)
        {
            //
            var errorBlock = string.Join(Environment.NewLine, this.Errors.Select(error =>
            {
                return $@"
<div class=""error"">
    <div class=""title"">{error.Title}</div>
    <div><pre>{error.Message.Trim()}</pre></div>
</div>
";
            }));

            return $@"
<!DOCTYPE html>
<html>
<head>
    <script defer src=""/mermaid.min.js""></script>
    <link rel='stylesheet' type='text/css' href='/global.css' />
  </head>
<body>

<div class=""content-scrollable"">
<main>
    <ul style=""list-style=none;"">
    <li><a target=""_blank"" href=""model.xsd"" alt=""XSD"">XSD</a></li>
    {string.Join(Environment.NewLine, links.Select(l => $"<li><a target=\"_blank\" href=\"{l.Value}\">{l.Key}</a></li>").ToList())}
    </ul>
</main>

{ errorBlock }

{ string.Join(Environment.NewLine, Parts)}

<h2>ERD</h2>

<div class=""svg-container"">
    <div class=""mermaid"">{this.MermaidMapper.ToString()}</div>
</div>

<h2>Tables</h2>

{ string.Join(Environment.NewLine + Environment.NewLine, this.TableMapper.Start().ToList()) }
</div>
<script>
mermaid.initialize({{
    startOnLoad: false,
    sequence: {{ actorMargin: 250 }}
}});

[...document.getElementsByClassName(""mermaid"")].reverse().forEach((element, i) => {{
    const id = `mermaid-${{Date.now()}}`;	
	mermaid.render(id, element.textContent.trim(), (svg, bind) => {{element.innerHTML = svg;}}, element);
}});

</script>

</body>
</html>

";
        }

    }
}
