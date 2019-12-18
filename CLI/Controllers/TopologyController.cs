using System;
using CLI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CLI.Controllers
{
    public class TopologyController: ControllerBase
    {

        [HttpGet("/api/topology")]
        public IActionResult GetTopology()
        {
            var topology = Project.Current?.GetTopology(true);
            if (topology is null) return NoContent();
            else return Ok(topology);
        }

        [HttpGet("/api/topology/modules")]
        public IActionResult GetTopologyModules()
        {
            var topology = Project.Current?.GetTopology(false);
            if (topology is null) return NoContent();
            else return Ok(topology);
        }
    }
}
