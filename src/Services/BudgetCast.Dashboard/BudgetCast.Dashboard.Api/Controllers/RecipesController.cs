using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BudgetCast.Dashboard.Api.Infrastructure.Extensions;
using BudgetCast.Dashboard.Api.ViewModels;
using BudgetCast.Dashboard.Api.ViewModels.Receipt;
using BudgetCast.Dashboard.Commands.Command;
using MediatR;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        private static List<string> _catNames = new List<string>
        {            
            "Shoes & Close",
            "Car",
            "Food",
            "Healthy food",
            "Cafe & restaurants",
            "Entertaiment"            
        };

        public RecipesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

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

        [HttpPost("addBasic")]
        public async Task<IActionResult> AddBasicReceipt(
            [FromBody] AddBasicReceiptViewModel model)
        {
            var cmd = _mapper.Map<CreateBasicReceiptCommand>(model);
            cmd.UserId = User.Identity.Name;
            var result = await _mediator.Send(cmd);

            return result.ToHttpActionResult();
        }
    }
}
