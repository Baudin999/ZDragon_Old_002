using Compiler.AST;
using Configuration;
using Project.FileSystems;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MemoryProject
{
    public class MemoryProject : IProject
    {
        private IFileSystem FileSystem { get; }
        public string BasePath => "";

        public string OutPath => "out/";

        public string ConfigPath => "zdragon.json";

        public CarConfig CarConfig { get; }
        public ObservableCollection<IModule> Modules { get; }

        public MemoryProject()
        {
            this.FileSystem = ProjectContext.FileSystem ?? throw new Exception("Invalid file system");
            this.CarConfig = new CarConfig();
            this.Modules = new ObservableCollection<IModule>();
        }


        public async Task<IModule> CreateModule(string moduleName, string? code)
        {
            var source = code ?? $"# {moduleName}";
            var module = new MemoryModule(moduleName, source);
            await this.FileSystem.SaveFile(module.FilePath, source);
            this.Modules.Add(module);
            return module;
        }

        public async Task<bool> DeleteModule(string moduleName)
        {
            var module = FindModule(moduleName);
            if (module != null)
            {
                this.Modules.Remove(module);
                module.Dispose();
            }

            return await Task.FromResult(true);
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        public IModule? FindModule(string moduleName)
        {
            return this.Modules.FirstOrDefault(m => m.Name == moduleName);
        }

        public List<IASTNode> GetAstForModule(string moduleName)
        {
            return FindModule(moduleName)?.AST ?? new List<IASTNode>();
        }

        public Topology GetTopology(bool includeDetails)
        {
            throw new NotImplementedException();
        }

        public Task<IModule> MoveModule(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public void ParseAllModules()
        {
            throw new NotImplementedException();
        }

        public void Watch()
        {
            throw new NotImplementedException();
        }
    }
}
