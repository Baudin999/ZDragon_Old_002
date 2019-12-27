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
        string BasePath { get; }

        /// <summary>
        /// Where the assets, the results are published to.
        /// </summary>
        string OutPath { get; }

        /// <summary>
        /// Where the zdragon.json file is located.
        /// </summary>
        string ConfigPath { get; }


        CarConfig CarConfig { get; }

        ObservableCollection<IModule> Modules { get; }

        Task<IModule> CreateModule(string moduleName, string? code);

        Task<IModule> MoveModule(string oldName, string newName);

        IModule? FindModule(string moduleName);

        Task<bool> DeleteModule(string moduleName);

        List<IASTNode> GetAstForModule(string moduleName);

        void ParseAllModules();

        void Watch();

        Topology GetTopology(bool includeDetails);
    }
}
