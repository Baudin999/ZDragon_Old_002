using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.AST;
using Mapper.HTML;
using Mapper.JSON;
using Mapper.XSD;

namespace CLI
{
    public class Transpiler
    {
        public Project Project { get; }
        public string Code { get; }
        public ASTGenerator Generator { get; }
        public XSDMapper XsdMapper { get; }
        public HtmlMapper HtmlMapper { get; }
        public JsonMapper JsonMapper { get; }
        public List<IASTError> Errors { get { return this.Generator.Errors; } }
        public List<IASTNode> Imports { get; private set; } = new List<IASTNode>();

        public Transpiler(string code, Project project)
        {
            this.Project = project;
            this.Code = code;
            this.Generator = new ASTGenerator(code);
            this.ResolveImports();

            this.XsdMapper = new XSDMapper(this.Generator.AST.Concat(this.Imports).ToList());
            this.XsdMapper.Start().ToList();

            this.HtmlMapper = new HtmlMapper(this.Generator.AST.Concat(this.Imports).ToList());
            this.HtmlMapper.Start().ToList();

            this.JsonMapper = new JsonMapper(this.Generator.AST.Concat(this.Imports).ToList());
            this.JsonMapper.Start();
        }

        public string XsdToString()
        {
            TextWriter writer = new StringWriter();
            XsdMapper.Schema.Write(writer);
            return writer.ToString();
        }

        public string HtmlToString()
        {
            return this.HtmlMapper.ToHtmlString(this.JsonMapper.ToFileNameAndContentDict());
        }

        public Dictionary<string, string> JsonToString()
        {
            return this.JsonMapper.ToFileNameAndContentDict();
        }


        /// <summary>
        /// Resolve the imports of this module. Here we'll
        /// link multiple files together. This is not part
        /// of the compiler, but part of the project system.
        /// TODO: verify this approach!
        /// </summary>
        private void ResolveImports()
        {
            this.Imports = new List<IASTNode>();
            var imports = Generator.AST.FindAll(n => n is ASTImport).ToList();
            imports.ForEach(node =>
            {
                ASTImport import = (ASTImport)node;
                var ast = this.Project.GetAstForModule(import.Name);
                if (!import.Imports.Any())
                {
                    var copies = ast
                        .FindAll(a => a is ASTType || a is ASTAlias || a is ASTData || a is ASTChoice)
                        .Select(a => {
                            return a switch
                            {
                                ASTType t => ObjectCopier.Clone(t) as IASTNode,
                                ASTAlias t => ObjectCopier.Clone(t) as IASTNode,
                                ASTData t => ObjectCopier.Clone(t) as IASTNode,
                                ASTChoice t => ObjectCopier.Clone(t) as IASTNode,
                                _ => throw new Exception("Can only serialize real AST nodes.")
                            };
                        })
                        .ToList();
                    this.Imports.AddRange(copies);
                }
            });
        }
    }
}
