using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Mapper.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CLI.Controllers
{
    public class ModuleController : ControllerBase
    {
        public string GetProject()
        {
            return "Carlos";
        }

        [HttpGet("/api/modules")]
        public IEnumerable<string> GetModules()
        {
            return Project.Current.Modules.Select(m => m.Name);
        }

        [HttpGet("/api/search/{param}")]
        public IEnumerable<Descriptor> Search(string param)
        {
            return Project
                .Current
                .Modules
                .SelectMany(m => m.GetDescriptions(param))
                .OrderBy(d => d.Module)
                .ThenBy(d => d.Parent)
                .ThenBy(d => d.Name);
        }


        [HttpGet("/api/svg")]
        public string RenderDescriptor(Descriptor descriptor)
        {
            if (descriptor.DescriptorType == DescriptorType.Type.ToString("g"))
            {
                var module = Project.Current.Modules.FirstOrDefault(m => m.Name == descriptor.Module);
                var node = module.Transpiler.AST.FirstOrDefault(a => a is INamable && ((INamable)a).Name == descriptor.Name);
                return "";
            }
            else
            {
                return "";
            }
        }
    }

}
