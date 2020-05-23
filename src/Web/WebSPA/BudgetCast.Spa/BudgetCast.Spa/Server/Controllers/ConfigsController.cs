using BudgetCast.Spa.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BudgetCast.Spa.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigsController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public ConfigsController(IOptionsSnapshot<AppSettings> settings)
        {
            _settings = settings;
        }

        [HttpGet("endpoints")]
        public IActionResult EndpointsConfiguration()
        {
            return Json(_settings.Value);
        }
    }
}
