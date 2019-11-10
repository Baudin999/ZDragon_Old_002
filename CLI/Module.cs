using System;
using Compiler;

namespace CLI
{
    public class Module
    {
        public string Name { get; }
        public string Path { get; }
        public string BasePath { get; }
        public string OutPath { get; }
        public Project Project { get; }
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
        }

        public void Parse()
        {
            var text = System.IO.File.ReadAllText(Path) + "\n";
            this.Transpiler = new Transpiler(text, this.Project);
            this.Generator = Transpiler.Generator;
            this.LastParsed = DateTime.Now;

            if (Transpiler.Errors.Count == 0)
            {
                Console.WriteLine($"Perfectly parsed: {Name}");
                SaveResult(Transpiler.XsdToString(), "Model.xsd");
                SaveResult(Transpiler.HtmlToString(), "index.html");
            }
            else
            {
                foreach (var error in Transpiler.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        public void SaveResult(string source, string fileName)
        {
            string filePath = System.IO.Path.GetFullPath(fileName, OutPath);
            System.IO.Directory.CreateDirectory(OutPath);
            System.IO.File.WriteAllText(filePath, source);
        }

        private string CreateModuleName()
        {
            var p = Path.Replace(BasePath, "").Replace("/", ".").Replace(".car", "");
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

    }
}
