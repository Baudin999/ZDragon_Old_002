using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Compiler.AST;
using Configuration;

namespace Project
{
    public interface IProject : IDisposable
    {
        CarConfig CarConfig { get; }

        ObservableCollection<Module> Modules { get; }

        Task<Module> CreateModule(string moduleName, string? code);

        Task<Module> MoveModule(string oldName, string newName);

        Module? FindModule(string moduleName);

        Task<bool> DeleteModule(string moduleName);

        List<IASTNode> GetAstForModule(string moduleName);

        void ParseAllModules();
    }
}
