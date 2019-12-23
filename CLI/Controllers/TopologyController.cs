using System;
using CLI.Models;
using Microsoft.AspNetCore.Mvc;
using Project;

namespace CLI.Controllers
{
    public class TopologyController: ControllerBase
    {

        [HttpGet("/api/topology")]
        public IActionResult GetTopology()
        {
            var topology = ProjectContext.Instance?.GetTopology(true);
            if (topology is null) return NoContent();
            else return Ok(topology);
        }

        [HttpGet("/api/topology/modules")]
        public IActionResult GetTopologyModules()
        {
            var topology = ProjectContext.Instance?.GetTopology(false);
            if (topology is null) return NoContent();
            else return Ok(topology);
        }
    }
}
