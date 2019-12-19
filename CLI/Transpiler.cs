using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Compiler;
using Compiler.AST;
using Mapper.Application;
using Mapper.HTML;
using Mapper.JSON;
using Mapper.XSD;
using Configuration;

namespace CLI
{
    public class Transpiler
    {
        private IEnumerable<Descriptor> Descriptions = Enumerable.Empty<Descriptor>();

        public Project Project { get; }
        public string Code => Generator.Code;
        public ASTGenerator Generator { get; }
        public XSDMapper XsdMapper { get; private set; }
        public HtmlMapper HtmlMapper { get; private set; }
        public JsonMapper JsonMapper { get; private set; }
        public DescriptionMapper DescriptionMapper { get; private set; }
        public List<IASTError> Errors { get { return this.Generator.Errors; } }
        public List<IASTNode> Imports { get; private set; } = new List<IASTNode>();

        public List<IASTNode> AST => Imports.Concat(Generator.AST).ToList();

        public Transpiler(ASTGenerator generator, Project project)
        {
            this.Generator = generator;
            this.Project = project;
            XsdMapper = new XSDMapper(generator);
            HtmlMapper = new HtmlMapper(generator);
            JsonMapper = new JsonMapper(generator);
            DescriptionMapper = new DescriptionMapper(generator);
        }

        public string XsdToString()
        {
            TextWriter writer = new StringWriter();
            XsdMapper.Schema.Write(writer);
            return writer.ToString() ?? "";
        }

        public string HtmlToString()
        {
            var links = new Dictionary<string, string>();
            this.JsonMapper.Schemas.ToList().ForEach(s => links.Add(s.Key, s.Key));
            return this.HtmlMapper.ToHtmlString(links);
        }

        public string ToDescriptors()
        {
            var json = JsonSerializer.Serialize(Descriptions, typeof(IEnumerable<Descriptor>));
            return json;
        }

        public Dictionary<string, string> JsonToString()
        {
            return this.JsonMapper.ToFileNameAndContentDict();
        }


        public void StartTranspilation(string moduleName, ErdConfig erdConfig)
        {
            this.Imports = ImportResolver.ResolveImports(this.Generator);
            this.Generator.Resolve(this.Imports);

            this.XsdMapper = new XSDMapper(this.Generator);
            this.XsdMapper.Start().ToList();

            this.HtmlMapper = new HtmlMapper(this.Generator, erdConfig);
            this.HtmlMapper.Start().ToList();

            this.JsonMapper = new JsonMapper(this.Generator);
            this.JsonMapper.Start();
        }

        
    }
}
