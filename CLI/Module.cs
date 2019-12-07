using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.AST;
using Mapper.Application;

namespace CLI
{
    public class Module : IDisposable
    {
        public string Name { get; }
        public string Path { get; }
        public string BasePath { get; }
        public string OutPath { get; }
        public Project Project { get; }
        public List<string> ReferencedModules { get; private set; } = new List<string>();
        public Transpiler Transpiler { get; private set; }
        public ASTGenerator Generator { get; private set; }
        public DateTime LastParsed { get; private set; }


        public Module(string path, string basePath, Project project)
        {
            this.Path = path;
            this.BasePath = basePath;
            this.Name = CreateModuleName();
            this.OutPath = System.IO.Path.GetFullPath($"out/{Name}", basePath);
            this.Project = project;

            this.Generator = new ASTGenerator("", this.Name);
            this.Transpiler = new Transpiler(this.Generator, this.Project);
        }

        public void Parse()
        {
            var code = ReadModuleText() + Environment.NewLine;
            this.Generator = new ASTGenerator(code, this.Name);
            this.LastParsed = DateTime.Now;
            this.Transpiler = new Transpiler(this.Generator, this.Project);

            ReferencedModules = Generator.AST.FindAll(n => n is ASTImport).Select(i => ((ASTImport)i).ModuleName).ToList();
        }

        public void SaveModuleOutput(bool decend = true)
        {
            this.Transpiler.StartTranspilation(this.Name);
            Console.WriteLine($"Perfectly parsed: {Name}");
            SaveResult("Model.xsd", Transpiler.XsdToString());
            SaveResult("index.html", Transpiler.HtmlToString());

            foreach (var (key, value) in Transpiler.JsonToString())
            {
                SaveResult(key, value);
            }

            // Trickery to no regenerate infinately...
            // TODO: replace with propper topological order...
            if (decend)
            {
                // We would now also want to resolve the other modules
                // which depend upon this module so that they are automatically
                // regenerated and their output changed.
                this.Project
                    .Modules
                    .FindAll(m => m.ReferencedModules.FirstOrDefault(r => r == this.Name) != null)
                    .ForEach(m =>
                        {
                            m.Parse();
                            m.SaveModuleOutput(false);
                        });
            }
        }

        public IEnumerable<Descriptor> GetDescriptions()
        {
            var mapper = new DescriptionMapper(this.Generator, this.Name);
            var nodes = mapper.Start().SelectMany(s => s);
            return nodes.ToList();
        }

        public bool SaveCode(string source)
        {
            try
            {

                System.IO.File.WriteAllText(this.Path, source);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private string ReadModuleText()
        {
            try
            {
                using (var fs = new FileStream(this.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        return sr.ReadToEnd();
                    }
                }
                throw new IOException("ReadModule failed with unknown exception.");
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe.ToString());
                throw ioe;
            }
        }

        private void SaveResult(string fileName, string source)
        {
            var filePath = System.IO.Path.GetFullPath(fileName, OutPath);
            System.IO.Directory.CreateDirectory(OutPath);
            System.IO.File.WriteAllText(filePath, source);
        }

        private string CreateModuleName()
        {
            var p = Path.Replace(BasePath, "").Replace("/", ".").Replace("\\", ".").Replace(".car", "");
            if (p.StartsWith(".", StringComparison.Ordinal))
            {
                p = p.Substring(1);
            }
            return p;
        }

        public override string ToString()
        {
            return $@"
Module:     {Name}
Path:       {Path}
Dir:        {BasePath}
LastParsed: {LastParsed}
";
        }

        public void Dispose(){}
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
