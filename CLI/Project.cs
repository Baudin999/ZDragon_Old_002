using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CLI.Signals;
using Compiler.AST;

namespace CLI
{
    public class Project : IDisposable
    {

        public static Project? Current { get; private set; }

        public string Path { get; } = "";
        public string OutPath { get; } = "";
        public List<Module> Modules { get; } = new List<Module>();
        public Action Cleanup { get; private set; } = () => { };

        public Project(string path)
        {
            this.Path = path;
            this.OutPath = System.IO.Path.GetFullPath($"out", this.Path);
            Directory.CreateDirectory(OutPath);

            var allfiles = Directory.GetFiles(path, "*.car", SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                var module = new Module(file, path, this);
                Modules.Add(module);
                module.Parse();
            }

            
            CreateIndexPage();
            CreateAssets();
            Cleanup = () => { };

            Task.Run(Parse);

            Project.Current = this;
        }

        public void Parse()
        {
            foreach (var module in this.Modules)
            {
                module.Parse();
                module.SaveModuleOutput();
            }
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
            var filePath = System.IO.Path.GetFullPath("index_old.html", OutPath);
            File.WriteAllText(filePath, page);
        }

        private void CreateAssets()
        {
            Helpers.ReadAndWriteAsset("CLI.Assets.style.css", System.IO.Path.GetFullPath("style.css", OutPath));
            Helpers.ReadAndWriteAsset("CLI.Assets.mermaid.min.js", System.IO.Path.GetFullPath("mermaid.min.js", OutPath));
        }

        public void OnClose(Action cleanup)
        {
            this.Cleanup = cleanup;
        }

        internal List<IASTNode> GetAstForModule(string moduleName)
        {
            var module = Modules.FirstOrDefault(m => m.Name == moduleName);
            if (module is null)
            {
                return new List<IASTNode>();
            }
            else
            {
                if (module.Generator is null)
                {
                    module.Parse();
                }
                return module?.Generator?.AST ?? new List<IASTNode>();
            }
        }

        public void Watch()
        {
            // Create a new FileSystemWatcher and set its properties.
            using var watcher = new FileSystemWatcher
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

            SignalSingleton.ExitSignal.Subscribe(() =>
            {
                watcher.Dispose();
                Cleanup();
            });

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }


        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                var module = Modules.FirstOrDefault(m => m.Path == e.FullPath);
                if (module is null)
                {
                    Console.WriteLine("Non Module changed, something went wrong, please restart your project.");
                    return;
                }

                module.Parse();
                module.SaveModuleOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnCreate(object source, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
                var module = new Module(e.FullPath, this.Path, this);
                Modules.Add(module);
                module.Parse();
                module.SaveModuleOutput();
                CreateIndexPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnDelete(object source, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
                Modules.Remove(Modules.FirstOrDefault(m => m.Path == e.FullPath));
                CreateIndexPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            try
            {
                Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
                Modules.Remove(Modules.FirstOrDefault(m => m.Path == e.OldFullPath));

                var module = new Module(e.FullPath, this.Path, this);
                Modules.Add(module);
                module.Parse();
                module.SaveModuleOutput();
                CreateIndexPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {

        }

        private class Helpers
        {
            public static string ReadAsset(string name)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = name;

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (!(stream is null))
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                } else
                {
                    return "";
                }
            }

            public static void WriteAsset(string path, string content)
            {
                File.WriteAllText(path, content);
            }

            public static void ReadAndWriteAsset(string assetName, string outPath)
            {
                Helpers.WriteAsset(outPath, Helpers.ReadAsset(assetName));
            }
        }
    }

}
