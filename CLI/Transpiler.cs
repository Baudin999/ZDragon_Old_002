using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.AST;
using Mapper.HTML;
using Mapper.XSD;

namespace CLI
{
    public class Transpiler
    {
        public string Code { get; }
        public XSDMapper XsdMapper { get; }
        public HtmlMapper HtmlMapper { get; }
        public List<IASTError> Errors { get; private set; } = new List<IASTError>();
        public Transpiler(string code)
        {
            this.Code = code;
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();
            parseTree = new Resolver(parseTree).Resolve().ToList();

            this.Errors = parser.Errors;

            this.XsdMapper = new XSDMapper(parseTree);
            this.XsdMapper.Start().ToList();

            this.HtmlMapper = new HtmlMapper(parseTree);
            this.HtmlMapper.Start().ToList();
        }


        public string XsdToString()
        {
            TextWriter writer = new StringWriter();
            XsdMapper.Schema.Write(writer);
            return writer.ToString();
        }

        public string HtmlToString()
        {
            return this.HtmlMapper.ToString();
        }


    }
}
