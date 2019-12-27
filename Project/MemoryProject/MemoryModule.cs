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

        public string Code { get;  }

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
            await this.FileSystem.SaveFile(this.FilePath, code);
        }

        public Task SaveModuleOutput(bool decend = true, bool suppressMessage = false)
        {
            throw new NotImplementedException();
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
