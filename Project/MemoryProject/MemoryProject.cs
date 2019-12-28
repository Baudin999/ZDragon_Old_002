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

            Task.Run( async () =>
            {
                var schoolModule = await CreateModule("School", @"
open Base
# School

% api: /school
type School =
    Name: Name;
    Students: List Student;
type Student extends Person;
");
                var baseModule = await CreateModule("Base", @"

# Base

alias Name = String;
type Person =
    FirstName: String;
    LastName: String;
");
            });
        }


        public async Task<IModule> CreateModule(string moduleName, string? code)
        {
            var source = code ?? $"# {moduleName}";
            var module = new MemoryModule(moduleName, source);
            await this.FileSystem.SaveFile(module.FilePath, source);
            this.Modules.Add(module);
            _ = module.SaveModuleOutput();
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
            var nodes = new List<TopologyNode>();
            var edges = new List<TopologyEdge>();

            this.Modules.ToList().ForEach(m =>
            {
                var moduleName = "m::" + m.Name;
                nodes.Add(new TopologyNode(moduleName, m.Name, new TopologyColor("Orange")) { Module = m.Name });
                m.Generator.AST.ForEach(node =>
                {
                    if (node is ASTImport)
                    {
                        var _import = ((ASTImport)node);
                        edges.Add(new TopologyEdge(moduleName, "m::" + _import.ModuleName, "")
                        {
                            Arrows = "to"
                        });
                    }
                    else if (includeDetails && node is ASTType)
                    {
                        var _type = ((ASTType)node);
                        var _id = $"{m.Name}.{_type.Name}";
                        nodes.Add(new TopologyNode(_id, _type.Name));
                        edges.Add(new TopologyEdge(moduleName, _id, ""));
                    }
                    else if (includeDetails && node is ASTAlias alias)
                    {
                        var _id = $"{m.Name}.{alias.Name}";
                        nodes.Add(new TopologyNode(_id, alias.Name, new TopologyColor("#0096a0")));
                        edges.Add(new TopologyEdge(moduleName, _id, ""));
                    }
                    else if (includeDetails && node is ASTData data)
                    {
                        var _id = $"{m.Name}.{data.Name}";
                        nodes.Add(new TopologyNode(_id, data.Name, new TopologyColor("#fef6e1")));
                        edges.Add(new TopologyEdge(moduleName, _id, ""));

                        foreach (var option in data.Options)
                        {
                            //edges.Add(new TopologyEdge(_id, $"{data.Module}.{option.Name}", ""));
                        }
                    }
                });
            });

            return new Topology(nodes, edges);
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
            
        }
    }
}
