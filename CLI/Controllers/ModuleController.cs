using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.AST;
using Mapper.Application;
using Mapper.HTML;
using Microsoft.AspNetCore.Mvc;
using Project;

namespace CLI.Controllers
{
    public class ModuleController : ControllerBase
    {

        [HttpGet("/api/modules")]
        public IEnumerable<string> GetModules()
        {
            return ProjectContext.Instance?.Modules.Select(m => m.Name) ?? Enumerable.Empty<string>();
        }

        [HttpPost("/api/modules/{name}")]
        public async Task<IActionResult> CreateModuleAsync(string name)
        {
            var checkModule = ProjectContext.Instance?.FindModule(name);
            if (checkModule != null)
            {
                return BadRequest($"Module: {name}, already exists and cannot be created.");
            }

            var project = ProjectContext.Instance;
            if (project != null)
            {
                Module? module = await project.CreateModule(name, null);
                if (module is null) return BadRequest($"Failed to created module {name}.");

                return Ok(new List<Descriptor> {
                    module.ToDescriptor("Your newly created module")
                });
            } else
            {
                return BadRequest($"Failed to created module {name}.");
            }
        }

        [HttpGet("/api/search/{param}")]
        public IEnumerable<Descriptor> Search(string param)
        {

            var moduleDescriptions = ProjectContext.Instance?.Modules.Select(m =>
            {
                return new Descriptor(m.Name)
                {
                    Module = m.Name,
                    Name = m.Name,
                    Description = "The \"" + m.Name + "\" module",
                    DescriptorType = DescriptorType.Module.ToString("g")
                };
            }) ?? Enumerable.Empty<Descriptor>();

            if (param == "modules:" || param == "modules")
            {
                return moduleDescriptions;
            }

            var descriptors = ProjectContext.Instance?
               .Modules
               .SelectMany(m => m.GetDescriptions());

            return moduleDescriptions.Concat(descriptors)
                .Where(d => d.Title.Contains(param, System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d.Module)
                .ThenBy(d => d.Parent)
                .ThenBy(d => d.Name) ?? Enumerable.Empty<Descriptor>();
        }

        [HttpGet("/api/svg")]
        public IActionResult RenderDescriptor([FromQuery]Descriptor descriptor)
        {
            
            var module = ProjectContext.Instance?.Modules.FirstOrDefault(m => m.Name == descriptor.Module);
            var node = module?.Transpiler.AST.FirstOrDefault(a => a is INamable && ((INamable)a).Name == descriptor.Name);
            if (node is null) return NotFound(descriptor);

            var mapper = new MermaidMapper(new Compiler.ASTGenerator(new List<IASTNode> { node }));
            mapper.Start().ToList();
            return Ok(mapper.ToString());
            
        }

        [HttpGet("/api/module/{module}")]
        public IActionResult GetModuleText(string module)
        {
            var m = ProjectContext.Instance?.FindModule(module);
            if (m is null) return NotFound();

            return Ok(m.Code);
        }

        [HttpPost("/api/module/{module}")]
        public async Task<IActionResult> SaveModuleTextAsync(string module)
        {
            var m = ProjectContext.Instance?.FindModule(module);


            if (m is null)
            {
                return await Task.Run(NotFound);
            }
            else { 
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var code = await reader.ReadToEndAsync();
                    m.SaveCode(code);
                    var result = Ok(m.Generator.Code);
                    reader.Close();
                    reader.Dispose();
                    return result;
                }
            }
        }

        [HttpGet("/api/module/{module}/errors")]
        public IActionResult GetModuleErrors(string module)
        {
            var m = ProjectContext.Instance?.FindModule(module);
            if (m is null) return NotFound();
            return Ok(m.Generator.Errors);
        }

    }

}
