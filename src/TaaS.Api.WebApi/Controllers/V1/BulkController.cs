using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude;
using TaaS.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById;
using TaaS.Core.Domain.Gratitude.Query.GetBulkGratitude;

namespace TaaS.Api.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class BulkController : ControllerBase
    {
        protected readonly ILogger<BulkController> Logger;
        protected readonly IMediator Mediator;

        public BulkController(ILogger<BulkController> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandomBulk(
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string? category,
            [FromQuery] string[]? filters,
            [FromQuery, DefaultValue(5)] int quantity = 5,
            [FromQuery, DefaultValue("eng")] string language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetBulkGratitudeQuery
            {
                Name = name,
                Signature = signature,
                Category = category,
                Filters = new List<string>(filters),
                Language = language,
                Quantity = quantity
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitudes Not Found.");
        }
        
        [HttpGet, Route("allfilters")]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetBulkRandomWithAllFilters(
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string? category,
            [FromQuery, DefaultValue(false)] bool different = false,
            [FromQuery, DefaultValue("eng")] string language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetBulkAllFiltersGratitudeQuery
            {
                Name = name,
                Signature = signature,
                Category = category,
                Different = different,
                Language = language
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitudes Not Found.");
        }

        [HttpGet, Route("allfilters/{gratitudeId:int}")]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetBulkByIdWithAllFilters(
            [FromRoute, Required] int gratitudeId,
            [FromQuery] string? name,
            [FromQuery] string? signature,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetBulkAllFiltersGratitudeByIdQuery()
            {
                Id = gratitudeId,
                Name = name,
                Signature = signature
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitudes Not Found.");
        }
    }
}