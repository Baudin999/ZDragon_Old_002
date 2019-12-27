using Compiler;
using Compiler.AST;
using Mapper.Application;
using Project.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.FileProject
{
    public class FileModule : IModule, IDisposable
    {
        private IFileSystem FileSystem { get; }
        public string Name { get; }
        public string FilePath { get; }
        public string BasePath { get; }
        public string OutPath { get; }
        public IProject Project { get; }
        public List<string> ReferencedModules { get; private set; } = new List<string>();
        public Transpiler Transpiler { get; private set; }
        public ASTGenerator Generator { get; private set; }
        public DateTime LastParsed { get; private set; }

        public List<IASTNode> AST
        {
            get
            {
                if (this.Generator is null)
                {
                    this.Parse();
                }
                return Generator?.AST ?? new List<IASTNode>();
            }
        }

        public string Code
        {
            get
            {
                if (Generator.Code == String.Empty)
                {
                    return FileSystem.ReadFileText(this.FilePath);
                }
                else
                {
                    return Generator.Code;
                }
            }
        }

        public Descriptor ToDescriptor(string description)
        {
            return new Descriptor(this.Name)
            {
                Module = this.Name,
                Description = description,
                Name = this.Name,
                DescriptorType = DescriptorType.Module.ToString("g")
            };
        }

        public FileModule(string path, string basePath, IProject project)
        {
            this.FileSystem = ProjectContext.FileSystem ?? throw new Exception("FileSystem does not exist");
            this.FilePath = path;
            this.BasePath = basePath;
            this.Name = CreateModuleName();
            this.OutPath = System.IO.Path.GetFullPath($"out/{Name}", basePath);
            this.Project = project;

            this.Generator = new ASTGenerator("", this.Name);
            this.Transpiler = new Transpiler(this.Generator, this.Project);
        }

        public void Parse()
        {
            var code = FileSystem.ReadFileText(this.FilePath) + Environment.NewLine;
            this.Generator = new ASTGenerator(code, this.Name);
            this.LastParsed = DateTime.Now;
            this.Transpiler = new Transpiler(this.Generator, this.Project);

            ReferencedModules = Generator.AST.FindAll(n => n is ASTImport).Select(i => ((ASTImport)i).ModuleName).ToList();
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
            await FileSystem.DeleteDirectory(OutPath);
        }

        public async Task SaveCode(string code) {
            await FileSystem.SaveFile(this.FilePath, code);
        }

        public IEnumerable<Descriptor> GetDescriptions()
        {
            var mapper = new DescriptionMapper(this.Generator, this.Name);
            var nodes = mapper.Start().SelectMany(s => s);
            return nodes.ToList();
        }

        private string CreateModuleName()
        {
            return FileModule.FromPathToName(this.FilePath, this.BasePath);
        }

        public override string ToString()
        {
            return $@"
Module:     {Name}
Path:       {FilePath}
Dir:        {BasePath}
LastParsed: {LastParsed}
";
        }

        public void Dispose() { }

        public static string FromPathToName(string path, string basePath)
        {
            var p = path.Replace(basePath, "").Replace("/", ".").Replace("\\", ".").Replace(".car", "");
            if (p.StartsWith(".", StringComparison.Ordinal))
            {
                p = p.Substring(1);
            }
            return p;
        }

    }

    public static class DictionaryHelpers
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }

    }
}
