using Compiler;
using Compiler.AST;
using Mapper.Application;
using Project.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MemoryProject
{
    public class MemoryModule : IModule
    {
        private IFileSystem FileSystem { get; }
        public string Name { get; }

        public string FilePath { get; }

        public string BasePath => "";

        public string OutPath { get; }

        public IProject Project { get; }

        public List<string> ReferencedModules { get; private set; }

        public Transpiler Transpiler { get; private set; }

        public ASTGenerator Generator { get; private set; }

        public DateTime LastParsed { get; private set; }

        public List<IASTNode> AST => Generator.AST;

        public List<IASTError> Errors => Generator.Errors;

        public string Code { get; private set; }

        public MemoryModule(string moduleName, string code)
        {
            this.FileSystem = ProjectContext.FileSystem ?? throw new Exception("Invalid File System");
            this.Name = moduleName;
            this.Code = code;
            this.FilePath = $"{moduleName}.car";
            this.OutPath = $"out/{moduleName}";
            this.Project = ProjectContext.Instance ?? throw new Exception("Project not set");

            this.Generator = new ASTGenerator(code, moduleName);
            this.Transpiler = new Transpiler(this.Generator, this.Project);
            this.LastParsed = DateTime.Now;

            this.ReferencedModules = new List<string>();
        }

        public void Parse()
        {
            var code = this.FileSystem.ReadFileText(this.FilePath) + Environment.NewLine;
            this.Generator = new ASTGenerator(code, this.Name);
            this.LastParsed = DateTime.Now;
            this.Transpiler = new Transpiler(this.Generator, this.Project);

            this.ReferencedModules = Generator.AST.FindAll(n => n is ASTImport).Select(i => ((ASTImport)i).ModuleName).ToList();
        }

        public async Task SaveCode(string code)
        {
            this.Code = code;
            await this.FileSystem.SaveFile(this.FilePath, code);
            this.Parse();
            await this.SaveModuleOutput();
        }

        public async Task SaveModuleOutput(bool decend = true, bool suppressMessage = false)
        {
            this.Transpiler.StartTranspilation(this.Name);
            if (!suppressMessage)
            {
                Console.WriteLine($"Perfectly parsed: {Name}");
            }
            await FileSystem.SaveFile("Model.xsd", this.OutPath, Transpiler.XsdToString());
            await FileSystem.SaveFile("index.html", this.OutPath, Transpiler.HtmlToString());

            foreach (var (key, value) in Transpiler.JsonToString())
            {
                await FileSystem.SaveFile(key, this.OutPath, value);
            }

            // Trickery to not parse infinately...
            // TODO: replace with propper topological order...
            if (decend)
            {
                // We would now also want to resolve the other modules
                // which depend upon this module so that they are automatically
                // regenerated and their output changed.
                // TODO: notice circular dependencies, remove diamond of death...
                this.Project
                    .Modules
                    .ToList()
                    .FindAll(m => m.ReferencedModules.FirstOrDefault(r => r == this.Name) != null)
                    .ForEach(async m =>
                    {
                        m.Parse();
                        await m.SaveModuleOutput(false);
                    });
            }
        }

        public async Task Clean()
        {
            await Task.Run(() => { });

        }

        public Descriptor ToDescriptor(string description)
        {
            return new Descriptor
            {
                Description = description,
                Name = this.Name,
                Module = this.Name
            };
        }

        public IEnumerable<Descriptor> GetDescriptions()
        {
            return new List<Descriptor>();
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
