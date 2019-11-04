using System;
namespace CLI
{
    public class Module
    {
        public string Name { get; }
        public string Path { get; }
        public string BasePath { get; }
        public DateTime LastParsed { get; private set; }

        public Module(string path, string basePath)
        {
            this.Path = path;
            this.BasePath = basePath;
            this.Name = CreateModuleName();
        }

        public void Parse()
        {
            var text = System.IO.File.ReadAllText(Path) + "\n";
            Transpiler transpiler = new Transpiler(text);
            this.LastParsed = DateTime.Now;

            if (transpiler.Errors.Count == 0)
            {
                Console.WriteLine($"Perfectly parsed: {Name}");
                Console.WriteLine(transpiler.XsdToString());
            }else
            {
                foreach (var error in transpiler.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }

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
