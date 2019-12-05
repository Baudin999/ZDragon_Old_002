using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.AST;
using Mapper.Application;
using Mapper.HTML;
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

        [HttpPost("/api/modules/{name}")]
        public IEnumerable<Descriptor> CreateModule(string name)
        {
            Project.Current?.CreateModule(name);
            return new List<Descriptor> {
                new Descriptor
                {
                    Module = name,
                    Description = "Your newly created module!",
                    Name = name,
                    DescriptorType = DescriptorType.Module.ToString("g")
                }
            };
        }

        [HttpGet("/api/search/{param}")]
        public IEnumerable<Descriptor> Search(string param)
        {

            var moduleDescriptions = Project.Current?.Modules.Select(m =>
            {
                return new Descriptor
                {
                    Module = m.Name,
                    Name = m.Name,
                    Description = "The " + m
                };
            }) ?? Enumerable.Empty<Descriptor>();

            var descriptors = Project
                .Current?
                .Modules
                .SelectMany(m => m.GetDescriptions(param));

            if (param == "modules:")
            {
                return moduleDescriptions;
            }

            return moduleDescriptions.Concat(descriptors)
                .OrderBy(d => d.Module)
                .ThenBy(d => d.Parent)
                .ThenBy(d => d.Name) ?? Enumerable.Empty<Descriptor>();
        }

        [HttpGet("/api/svg")]
        public IActionResult RenderDescriptor([FromQuery]Descriptor descriptor)
        {
            
            var module = Project.Current?.Modules.FirstOrDefault(m => m.Name == descriptor.Module);
            var node = module?.Transpiler.AST.FirstOrDefault(a => a is INamable && ((INamable)a).Name == descriptor.Name);
            if (node is null) return NotFound(descriptor);

            var mapper = new MermaidMapper(new Compiler.ASTGenerator(new List<IASTNode> { node }));
            mapper.Start().ToList();
            return Ok(mapper.ToString());
            
        }

        [HttpGet("/api/module/{module}")]
        public IActionResult GetModuleText(string module)
        {
            var m = Project.Current?.FindModule(module);
            if (m is null) return NotFound();

            return Ok(m.Generator.Code);
        }

        [HttpPost("/api/module/{module}")]
        public async Task<IActionResult> SaveModuleTextAsync(string module)
        {
            var m = Project.Current?.FindModule(module);

            
            if (!(m is null))
            {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var code = await reader.ReadToEndAsync();
                    m.SaveCode(code);
                    return Ok(m.Generator.Code);
                }
            }

            return NotFound();
        }

    }

}
