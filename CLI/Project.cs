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
using Configuration;

namespace CLI
{
    public partial class Project : IDisposable
    {

        public static Project? Current { get; private set; }

        /// <summary>
        /// The Directory in which the project is created. This is the root path,
        /// the path where the zdragon.json file is located.
        /// </summary>
        public string BasePath { get; } = "";

        /// <summary>
        /// Where the assets, the results are published to.
        /// </summary>
        public string OutPath { get; } = "";

        /// <summary>
        /// Where the zdragon.json file is located.
        /// </summary>
        public string ConfigPath { get; } = "";


        public CarConfig CarConfig { get; private set; }
        public ObservableCollection<Module> Modules { get; } = new ObservableCollection<Module>();

        public Project(string path)
        {
            this.BasePath = path;
            this.OutPath = Path.GetFullPath($"out", this.BasePath);
            this.ConfigPath = Path.GetFullPath("zdragon.json", this.BasePath);
            this.CarConfig = CarConfig.Load(this.ConfigPath);

            Directory.CreateDirectory(OutPath);

            var allfiles = Directory.GetFiles(path, "*.car", SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                var module = new Module(file, path, this);
                Modules.Add(module);
                module.Parse();
            }

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
            Helpers.ReadAndWriteAsset("mermaid.min.js", this.OutPath);
            Helpers.ReadAndWriteAsset("mermaid.min.js.map", this.OutPath);
        }

        public void CreateModule(string name)
        {
            
            var fileName = String.Join("/", name.Split(".").Select(UppercaseFirst)) + ".car";
            var modulePath = System.IO.Path.GetFullPath(fileName, this.BasePath);
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
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe);
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

        

        public void Dispose()
        {

        }

        private static class Helpers
        {
            public static string ReadAsset(string name)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "CLI.Assets." + name;

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
                var outName = System.IO.Path.GetFullPath(assetName, outPath);
                Helpers.WriteAsset(outName, Helpers.ReadAsset(assetName));
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
