using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Mapper.Application;
using Microsoft.AspNetCore.Mvc;

namespace CLI.Controllers
{
    public class ModuleController : ControllerBase
    {

        [HttpGet("/api/modules")]
        public IEnumerable<string> GetModules()
        {
            return Project.Current?.Modules.Select(m => m.Name) ?? Enumerable.Empty<string>();
        }

        [HttpGet("/api/search/{param}")]
        public IEnumerable<Descriptor> Search(string param)
        {
            if (param == "modules:")
            {
                return Project.Current?.Modules.Select(m =>
                {
                    return new Descriptor
                    {
                        Module = m.Name,
                        Description = "The " + m
                    };
                }) ?? Enumerable.Empty<Descriptor>();
            }

            return Project
                .Current?
                .Modules
                .SelectMany(m => m.GetDescriptions(param))
                .OrderBy(d => d.Module)
                .ThenBy(d => d.Parent)
                .ThenBy(d => d.Name) ?? Enumerable.Empty<Descriptor>();
        }

        [HttpGet("/api/svg")]
        public string RenderDescriptor(Descriptor descriptor)
        {
            if (descriptor.DescriptorType == DescriptorType.Type.ToString("g"))
            {
                var module = Project.Current?.Modules.FirstOrDefault(m => m.Name == descriptor.Module);
                var node = module?.Transpiler.AST.FirstOrDefault(a => a is INamable && ((INamable)a).Name == descriptor.Name);
                return "";
            }
            else
            {
                return "";
            }
        }
    }

}
