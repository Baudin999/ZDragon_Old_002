using System;
using System.IO;
using System.Linq;
using Compiler;
using Mapper.XSD;

namespace CLI
{
    public class Transpiler
    {
        public string Code { get; }
        public XSDMapper XsdMapper { get; }
        public Transpiler(string code)
        {
            this.Code = code;
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();

            this.XsdMapper = new XSDMapper(parseTree);
            this.XsdMapper.Start().ToList();
        }


        public string XsdToString()
        {
            TextWriter writer = new StringWriter();
            XsdMapper.Schema.Write(writer);
            return writer.ToString();
        }

        
    }
}
