using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        // GET: /version
        [HttpGet]
        public Response Get()
        {
            return new SuccessResponse(new {
                version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                runtime = PlatformServices.Default.Application.RuntimeFramework,
                src = "https://github.com/pvpgn/api.pvpgn.pro"
            });
        }
    }
}
