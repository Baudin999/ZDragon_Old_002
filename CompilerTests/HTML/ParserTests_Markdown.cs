using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.AST;
using Mapper.HTML;
using Xunit;

namespace CompilerTests.HTML
{
    public class ParserTests_Markdown
    {
        [Fact]
        public void SimpleMarkdownTest()
        {
            var code = @"
# This is a chapter

## This is a subChapter

And a few paragraphs to see how
things are coming along!
";
            var g = new ASTGenerator(code);
            Assert.Equal(3, g.AST.Count);

            Assert.True(g.AST[0] is ASTChapter);
            Assert.True(g.AST[1] is ASTChapter);
            Assert.True(g.AST[2] is ASTParagraph);

        }


        [Fact]
        public void TestLargerMardownExample()
        {
            var code = @"
# This is a chapter

## This is a subChapter

And a few paragraphs to see how
things are coming along!


> NOTE: a simple note!

```
function foo() {
    return 12;
}
```

type Person =
    FirstName: String;

* One
    * Two
    * Three
* Four

";
            var g = new ASTGenerator(code);
            Assert.Equal(7, g.AST.Count);

            Assert.True(g.AST[0] is ASTChapter);
            Assert.True(g.AST[1] is ASTChapter);
            Assert.True(g.AST[2] is ASTParagraph);
            Assert.True(g.AST[3] is ASTParagraph);
            Assert.True(g.AST[4] is ASTParagraph);
            Assert.True(g.AST[5] is ASTType);
            Assert.True(g.AST[6] is ASTParagraph);
        }

        [Fact]
        public void ConvertToHTML()
        {
            var code = @"
# This is a chapter

And another chapter!

## This is a subChapter

And a few paragraphs to see how
things are coming along!


> NOTE: a simple note!

```
function foo() {
    return 12;
}
```

type Person =
    FirstName: String;

* One
    * Two
    * Three
* Four

";
            var g = new ASTGenerator(code);
            var mapper = new HtmlMapper(g.AST);
            _ = mapper.Start().ToList();
            var html = mapper.ToHtmlString(new Dictionary<string, string>()).Trim();
            var txt = File.ReadAllText("./HTML/Example01.html");
            Assert.Equal(txt, html);
        }

        
    }

    
}
