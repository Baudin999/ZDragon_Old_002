using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CLI.Signals;
using Compiler.AST;

namespace CLI
{
    public partial class Project : IDisposable
    {

        public static Project? Current { get; private set; }

        public string Path { get; } = "";
        public string OutPath { get; } = "";
        public string ConfigPath { get; } = "";
        public CarConfig? CarConfig { get; private set; }
        public ObservableCollection<Module> Modules { get; } = new ObservableCollection<Module>();


        public Project(string path)
        {
            this.Path = path;
            this.OutPath = System.IO.Path.GetFullPath($"out", this.Path);
            this.ConfigPath = System.IO.Path.GetFullPath("zdragon.json", this.Path);

            Directory.CreateDirectory(OutPath);

            var allfiles = Directory.GetFiles(path, "*.car", SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                var module = new Module(file, path, this);
                Modules.Add(module);
                module.Parse();
            }

            InitCarConfig();
            CreateAssets();

            Project.Current = this;

            SignalSingleton.ExitSignal.Subscribe(() =>
            {
                Console.WriteLine("Disposing of the project...");
                Project.Current = null;
                foreach (Module m in this.Modules)
                {
                    m.Dispose();
                }
                this.Dispose();
            });

            Task.Run(Parse);
        }

        public void Parse()
        {
            foreach (var module in this.Modules)
            {
                module.Parse();
                module.SaveModuleOutput(false);
            }
        }

        private void CreateAssets()
        {
            // Nothing yet
        }

        public void CreateModule(string name)
        {
            
            var fileName = String.Join("/", name.Split(".").Select(UppercaseFirst)) + ".car";
            var modulePath = System.IO.Path.GetFullPath(fileName, this.Path);
            var directoryName = System.IO.Path.GetDirectoryName(modulePath);
            Directory.CreateDirectory(directoryName);
            try
            {
                using (var fs = new FileStream(modulePath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.Write($@"
# {name}

Have fun with your module!


");
                        sr.Flush();
                        sr.Close();
                        sr.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();

                    return;
                }
                throw new IOException("ReadModule failed with unknown exception.");
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe.ToString());
                throw ioe;
            }
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

        public Module? FindModule(string name)
        {
            return this.Modules.FirstOrDefault(m => m.Name == name);
        }

        public int Watch()
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

            // Add event handlers.
            watcher.Changed += OnChanged;
            //watcher.Created += OnCreate;
            watcher.Deleted += OnDelete;
            watcher.Renamed += OnRenamed;

            SignalSingleton.ExitSignal.Subscribe(() =>
            {
                Console.WriteLine("Disposing the file watcher...");
                watcher.Dispose();
            });

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
            Console.WriteLine();

            return SignalSingleton.ExitSignal.Dispatch();
        }


        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                var module = Modules.FirstOrDefault(m => m.Path == e.FullPath);
                if (!(module is null))
                {
                    module.Parse();
                    module.SaveModuleOutput();
                }
                else
                {
                    module = new Module(e.FullPath, this.Path, this);
                    module.Parse();
                    module.SaveModuleOutput();
                    Modules.Add(module);
                }
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
                module.SaveModuleOutput(false);
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

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (!(stream is null))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            reader.Close();
                            reader.Dispose();
                            return result;
                        }
                    }
                    else
                    {
                        return "";
                    }
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

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

}
