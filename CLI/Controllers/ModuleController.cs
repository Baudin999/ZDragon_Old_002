using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
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
        public IEnumerable<TypeResult> Search(string param)
        {
            return Project.Current.Modules.SelectMany(m =>
            {
                return m.Generator
                    .AST
                    .FindAll(n => n is INamable)
                    .Select(n =>
                    {
                        return new TypeResult
                        {
                            Module = m.Name,
                            Name = ((INamable)n).Name
                        };
                    });
            });
        }
    }

    public class TypeResult
    {
        public string Name { get; set; }
        public string Module { get; set; }
    }
}
