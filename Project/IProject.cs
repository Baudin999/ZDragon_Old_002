using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Compiler.AST;
using Configuration;
using Project.Models;

namespace Project
{
    public interface IProject : IDisposable
    {
        /// <summary>
        /// The Directory in which the project is created. This is the root path,
        /// the path where the zdragon.json file is located.
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// Where the assets, the results are published to.
        /// </summary>
        public string OutPath { get; }

        /// <summary>
        /// Where the zdragon.json file is located.
        /// </summary>
        public string ConfigPath { get; }


        CarConfig CarConfig { get; }

        ObservableCollection<Module> Modules { get; }

        Task<Module> CreateModule(string moduleName, string? code);

        Task<Module> MoveModule(string oldName, string newName);

        Module? FindModule(string moduleName);

        Task<bool> DeleteModule(string moduleName);

        List<IASTNode> GetAstForModule(string moduleName);

        void ParseAllModules();

        void Watch();

        Topology GetTopology(bool includeDetails);
    }
}
