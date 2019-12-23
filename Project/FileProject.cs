using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Compiler.AST;
using Configuration;

namespace Project
{
    public partial class FileProject : IProject, IDisposable
    {

        //public static FileProject? Current { get; private set; }

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

        public FileProject(string path)
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

            Task.Run(ParseAllModules);
        }

        public void ParseAllModules()
        {
            foreach (var module in this.Modules)
            {
                module.Parse();
                module.SaveModuleOutput(false);
            }
        }

        public Task<Module> CreateModule(string name, string? code = null)
        {
            var tcs = new TaskCompletionSource<Module>();
            try
            {
                var fileName = String.Join("/", name.Split(".").Select(UppercaseFirst)) + ".car";
                var modulePath = Path.GetFullPath(fileName, this.BasePath);
                var directoryName = Path.GetDirectoryName(modulePath);
                Directory.CreateDirectory(directoryName);

                if (this.ProjectWatcher != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Action cleanup = () => { };
                        var listenerName = "CREATEOR: " + DateTime.Now + new Random().Next(1000000).ToString();
                        cleanup = this.ProjectWatcher.ModuleStream.Subscribe(listenerName, MessageType.ModuleCreated, msm =>
                        {
                            var module = this.FindModule(msm.ModuleName);
                            if (module != null) tcs.TrySetResult(module);
                            else tcs.TrySetException(new Exception("Failed to create the Module"));
                            Task.Run(cleanup);
                        });
                    });
                }

                CreateNewModuleFile(modulePath, name, code);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public async Task<Module> MoveModule(string oldName, string newName)
        {
            var fileName = String.Join("/", newName.Split(".").Select(UppercaseFirst)) + ".car";
            var modulePath = Path.GetFullPath(fileName, this.BasePath);
            var directoryName = Path.GetDirectoryName(modulePath);
            Directory.CreateDirectory(directoryName);

            var oldModule = FindModule(oldName);
            var code = oldModule?.Code.Clone().ToString() ?? "";

            _ = await DeleteModule(oldName);
            var module = await CreateModule(newName);
            module.SaveCode(code);
            return module;
        }

        public List<IASTNode> GetAstForModule(string moduleName)
        {
            var module = Modules.FirstOrDefault(m => m.Name == moduleName);
            return module?.AST ?? new List<IASTNode>();
        }

        public Module? FindModule(string name)
        {
            return this.Modules.FirstOrDefault(m => m.Name == name);
        }

        public Task<bool> DeleteModule(string moduleName)
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                var module = FindModule(moduleName);
                if (module is null) return Task.FromResult(false);

                if (this.ProjectWatcher != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Action cleanup = () => { };
                        var listenerName = "CREATEOR: " + DateTime.Now + new Random().Next(1000000).ToString();
                        cleanup = this.ProjectWatcher.ModuleStream.Subscribe(listenerName, MessageType.ModuleDeleted, msm =>
                        {
                            module.Clean();
                            if (module != null) tcs.TrySetResult(true);
                            else tcs.TrySetException(new Exception("Failed to delete the Module"));
                            Task.Run(cleanup);
                        });
                    });
                }

                File.Delete(module.FilePath);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
            return tcs.Task;
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing of the project...");
            ProjectContext.Destruct();
            this.ProjectWatcher?.Dispose();
            foreach (Module m in this.Modules)
            {
                m.Dispose();
            }
        }

        private void CreateNewModuleFile(string modulePath, string moduleName, string? code = null)
        {
            try
            {
                using (var fs = new FileStream(modulePath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.Write(code ?? $@"
# {moduleName}

Have fun with your module!
");
                        sr.Flush();
                        sr.Close();
                        sr.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();

                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe);
                throw ioe;
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
