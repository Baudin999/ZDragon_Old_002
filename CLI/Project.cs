using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compiler.AST;

namespace CLI
{
    public class Project: IDisposable
    {
        public string Path { get; }
        public string OutPath { get; }
        public List<Module> Modules { get; } = new List<Module>();
        public Action Cleanup { get; private set; }

        public Project(string path)
        {
            this.Path = path;
            this.OutPath = System.IO.Path.GetFullPath($"out", this.Path);

            string[] allfiles = Directory.GetFiles(path, "*.car", SearchOption.AllDirectories);
            foreach (string file in allfiles)
            {
                var module = new Module(file, path, this);
                Modules.Add(module);
                module.Parse();
            }
            CreateIndexPage();
            Cleanup = () => { };
        }

        private void CreateIndexPage()
        {
            var moduleLinks = Modules.Select(m => $"<li><a href=\"/{m.Name}/index.html\">{m.Name}</a></li>");
            var page = $@"
<!DOCTYPE html>
<html>
<head></head>
<body>

<ul>
{string.Join("", moduleLinks)}
</ul>
</body>
</html>
";
            string filePath = System.IO.Path.GetFullPath("index.html", OutPath);
            Directory.CreateDirectory(OutPath);
            File.WriteAllText(filePath, page);
        }

        public void OnClose(Action cleanup)
        {
            this.Cleanup = cleanup;
        }


        internal List<IASTNode> GetAstForModule(string moduleName)
        {
            var module = Modules.FirstOrDefault(m => m.Name == moduleName);
            if (module is null) {
                return new List<IASTNode>();
            }
            else
            {
                if (module.Generator is null)
                {
                    module.Parse();
                    
                }
                return module.Generator.AST;
            }
        }

        public void Watch()
        {
            // Create a new FileSystemWatcher and set its properties.
            using FileSystemWatcher watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                Path = this.Path,

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName,

                // Only watch text files.
                Filter = "*.car"
            };

            this.Cleanup = watcher.Dispose;

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnCreate;
            watcher.Deleted += OnDelete;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            watcher.Dispose();
            Cleanup();
        }


        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var module = Modules.FirstOrDefault(m => m.Path == e.FullPath);
            if (module is null)
            {
                Console.WriteLine("Non Module changed, something went wrong, please restart your project.");
                return;
            }

            module.Parse();
        }

        // Define the event handlers.
        private void OnCreate(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            var module = new Module(e.FullPath, this.Path, this);
            Modules.Add(module);
        }

        // Define the event handlers.
        private void OnDelete (object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }

        public void Dispose()
        {
            
        }
    }
}
