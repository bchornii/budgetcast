using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private static List<string> _catNames = new List<string>
        {            
            "Shoes & Close",
            "Car",
            "Food",
            "Healthy food",
            "Cafe & restaurants",
            "Entertaiment"            
        };

        [HttpGet("categories")]
        public IActionResult GetCategories(
            [FromQuery] string name, [FromQuery] int amount)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(_catNames.Take(amount).ToArray());
            }
            return Ok(_catNames
                .Where(n => n.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v)
                .Take(amount)
                .ToArray());
        }
    }
}
