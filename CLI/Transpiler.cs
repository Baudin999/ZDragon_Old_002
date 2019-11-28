using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.AST;
using Mapper.HTML;
using Mapper.JSON;
using Mapper.XSD;
using Mapper.Application;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CLI
{
    public class Transpiler
    {
        private IEnumerable<Descriptor> Descriptions;

        public Project Project { get; }
        public string Code => Generator.Code;
        public ASTGenerator Generator { get; }
        public XSDMapper XsdMapper { get; private set; }
        public HtmlMapper HtmlMapper { get; private set; }
        public JsonMapper JsonMapper { get; private set; }
        public DescriptionMapper DescriptionMapper { get; private set; }
        public List<IASTError> Errors { get { return this.Generator.Errors; } }
        public List<IASTNode> Imports { get; private set; } = new List<IASTNode>();

        public Transpiler(ASTGenerator generator, Project project)
        {
            this.Generator = generator;
            this.Project = project;
        }

        public string XsdToString()
        {
            TextWriter writer = new StringWriter();
            XsdMapper.Schema.Write(writer);
            return writer.ToString();
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


        public void StartMappings(string moduleName = "")
        {
            this.ResolveImports();
            this.XsdMapper = new XSDMapper(this.Generator.AST.Concat(this.Imports).ToList());
            this.XsdMapper.Start().ToList();

            this.HtmlMapper = new HtmlMapper(this.Generator.AST.Concat(this.Imports).ToList());
            this.HtmlMapper.Start().ToList();

            this.JsonMapper = new JsonMapper(this.Generator.AST.Concat(this.Imports).ToList());
            this.JsonMapper.Start();

            this.DescriptionMapper = new DescriptionMapper(this.Generator.AST.Concat(this.Imports).ToList(), moduleName);
            Descriptions = this.DescriptionMapper.Start().ToList().SelectMany(s => s).ToList();
        }

        private void ResolveImports()
        {
            this.Imports = new List<IASTNode>();
            var imports = Generator.AST.FindAll(n => n is ASTImport).ToList();
            imports.ForEach(node =>
            {
                var import = (ASTImport)node;
                var ast = this.Project.GetAstForModule(import.Name);
                if (!import.Imports.Any())
                {
                    var copies = ast
                        .FindAll(a => a is ASTType || a is ASTAlias || a is ASTData || a is ASTChoice)
                        .Select(a => {
                            return a switch
                            {
                                ASTType t => t.Clone() as IASTNode,
                                ASTAlias t => t.Clone() as IASTNode,
                                ASTData t => t.Clone() as IASTNode,
                                ASTChoice t => t.Clone() as IASTNode,
                                _ => throw new Exception("Can only serialize real AST nodes.")
                            };
                        })
                        .ToList();
                    this.Imports.AddRange(copies);
                } else
                {
                    var importedNodes = import.Imports.Select(import =>
                    {
                        return ast.FirstOrDefault(a =>
                        {
                            return a switch
                            {
                                ASTType n => n.Name == import,
                                ASTAlias n => n.Name == import,
                                ASTData n => n.Name == import,
                                ASTChoice n => n.Name == import,
                                _ => false
                            };
                        });                    
                    })
                    .ToList()
                    .FindAll(n => !(n is null));
                    this.Imports.AddRange(importedNodes);
                }
            });
        }
    }
}
