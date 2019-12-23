using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CLI.Models;
using Microsoft.AspNetCore.Mvc;
using Project;

namespace CLI.Controllers
{
    public class RemoteController : ControllerBase
    {
        [HttpGet("/api/remote/module/{module}")]
        public async Task<IActionResult> GetModuleText(string module)
        {
            var project = ProjectContext.Instance;
            if (project is null) return NotFound();
            
            var url = project.CarConfig?.Remote + "/api/module/" + module;
            if (url is null) return NotFound();

            var request = WebRequest.Create(url);
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return Ok(await reader.ReadToEndAsync());
            }
        }
    }
}
