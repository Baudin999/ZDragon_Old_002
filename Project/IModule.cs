using Compiler;
using Compiler.AST;
using Mapper.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project
{
    public interface IModule : IDisposable
    {
        string Name { get; }
        string FilePath { get; }
        string BasePath { get; }
        string OutPath { get; }
        IProject Project { get; }
        List<string> ReferencedModules { get; }
        Transpiler Transpiler { get; }
        ASTGenerator Generator { get; }
        DateTime LastParsed { get; }
        List<IASTNode> AST { get; }
        string Code { get; }
        Task SaveCode(string code);
        void Parse();
        Task SaveModuleOutput(bool decend = true, bool suppressMessage = false);
        Task Clean();

        // Refactor these two out

        Descriptor ToDescriptor(string description);
        IEnumerable<Descriptor> GetDescriptions();
    }
}
