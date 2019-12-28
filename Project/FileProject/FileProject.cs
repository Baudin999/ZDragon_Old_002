using Compiler.AST;
using Configuration;
using Project.FileProject;
using Project.FileSystems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.File
{
    public partial class FileProject : IProject, IDisposable
    {
        private IFileSystem FileSystem { get; }

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

        public ObservableCollection<IModule> Modules { get; } = new ObservableCollection<IModule>();

        public FileProject(string path)
        {
            this.BasePath = path;
            this.OutPath = Path.GetFullPath($"out", this.BasePath);
            this.ConfigPath = Path.GetFullPath("zdragon.json", this.BasePath);
            this.CarConfig = CarConfig.Load(this.ConfigPath);
            this.FileSystem = ProjectContext.FileSystem ?? throw new Exception("FileSystem does not exist");

            this.FileSystem.CreateDirectory(OutPath);
            this.InitializeModules();
        }

        public async void InitializeModules()
        {
            await this.FileSystem.GetAllFiles(this.BasePath, "*.car", file =>
            {
                var module = new FileModule(file, this.BasePath, this);
                Modules.Add(module);
                module.Parse();
            });

            _ = Task.Run(ParseAllModules);
        }

        public async void ParseAllModules()
        {
            var awaitables = new List<Task>();
            foreach (var module in this.Modules)
            {
                module.Parse();
                awaitables.Add(module.SaveModuleOutput(false));
            }
            await Task.WhenAll(awaitables);
        }

        public Task<IModule> CreateModule(string name, string? code = null)
        {
            var tcs = new TaskCompletionSource<IModule>();
            try
            {
                var fileName = String.Join("/", name.Split(".").Select(UppercaseFirst)) + ".car";
                var modulePath = Path.GetFullPath(fileName, this.BasePath);
                var directoryName = Path.GetDirectoryName(modulePath) ?? throw new Exception("Directory does not exist.");

                if (this.ProjectWatcher != null)
                {
                    this.ProjectWatcher.ModuleStream.Once(msm =>
                    {
                        var module = this.FindModule(msm.ModuleName);
                        if (module != null) tcs.TrySetResult(module);
                        else tcs.TrySetException(new Exception("Failed to create the Module"));
                    });
                }

                _ = FileSystem.SaveFile(modulePath, directoryName, $"# {name}");
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public async Task<IModule> MoveModule(string oldName, string newName)
        {
            var fileName = String.Join("/", newName.Split(".").Select(UppercaseFirst)) + ".car";
            var modulePath = Path.GetFullPath(fileName, this.BasePath);
            var directoryName = Path.GetDirectoryName(modulePath);
            Directory.CreateDirectory(directoryName);

            var oldModule = FindModule(oldName);
            var code = oldModule?.Code.Clone().ToString() ?? "";

            _ = await DeleteModule(oldName);
            var module = await CreateModule(newName);
            await module.SaveCode(code);
            return module;
        }

        public List<IASTNode> GetAstForModule(string moduleName)
        {
            var module = Modules.FirstOrDefault(m => m.Name == moduleName);
            return module?.AST ?? new List<IASTNode>();
        }

        public IModule? FindModule(string name)
        {
            return this.Modules.FirstOrDefault(m => m.Name == name);
        }

        public async Task<bool> DeleteModule(string moduleName)
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                var module = FindModule(moduleName);
                if (module is null) return await Task.FromResult(false);

                if (this.ProjectWatcher != null)
                {
                    this.ProjectWatcher.ModuleStream.Once(msm =>
                    {
                        tcs.TrySetResult(true);
                    });
                }

                await FileSystem.DeleteFile(module.FilePath);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
            return await tcs.Task;
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing of the project...");
            ProjectContext.Destruct();
            this.ProjectWatcher?.Dispose();
            foreach (FileModule m in this.Modules)
            {
                m.Dispose();
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
