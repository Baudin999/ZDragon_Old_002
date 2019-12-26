using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.AST;
using Mapper.Application;
using Configuration;
using System.Threading.Tasks;

namespace Project
{
    public class Module : IDisposable
    {
        public string Name { get; }
        public string FilePath { get; }
        public string BasePath { get; }
        public string OutPath { get; }
        public FileProject Project { get; }
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
                    return IO.ReadFileText(this.FilePath);
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

        public Module(string path, string basePath, FileProject project)
        {
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
            var code = IO.ReadFileText(this.FilePath) + Environment.NewLine;
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
            await IO.SaveFile("Model.xsd", this.OutPath, Transpiler.XsdToString());
            await IO.SaveFile("index.html", this.OutPath, Transpiler.HtmlToString());

            foreach (var (key, value) in Transpiler.JsonToString())
            {
                await IO.SaveFile(key, this.OutPath, value);
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
        public void Clean()
        {
            try
            {
                if (OutPath != null && Directory.Exists(OutPath))
                {
                    Task.Factory.StartNew(() => Directory.Delete(OutPath, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Cleaning Module " + this.Name);
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SaveCode(string code) {
            await IO.SaveFile(this.FilePath, code);
        }

        public IEnumerable<Descriptor> GetDescriptions()
        {
            var mapper = new DescriptionMapper(this.Generator, this.Name);
            var nodes = mapper.Start().SelectMany(s => s);
            return nodes.ToList();
        }

        private string CreateModuleName()
        {
            return Module.FromPathToName(this.FilePath, this.BasePath);
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
