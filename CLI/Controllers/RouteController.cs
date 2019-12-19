using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CLI.Controllers
{
    public class MonitorController : ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _provider;

        public MonitorController(IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
        }

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var rs = _provider.ActionDescriptors.Items;
            var routes = rs.Select(x => (
                Controller: x.RouteValues["controller"],
                Action: x.RouteValues["action"],
                Url: $"{x.RouteValues["controller"]}/{x.RouteValues["action"]}"
            )).ToList();
            return Ok(routes);
        }
    }
}
