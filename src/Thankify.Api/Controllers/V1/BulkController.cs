using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById;
using Thankify.Core.Domain.Gratitude.Query.GetBulkGratitude;
using Thankify.Api.Model.V1;

namespace Thankify.Api.Controllers.V1
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

        /// <summary>
        /// Get many random gratitude sentences, optionally specify a name, a signature and a category. Thanks!
        /// Note: Some of them may be repeated.
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="category">Category to get the random gratitude from.</param>
        /// <param name="filters">Filter or filters to apply to the text. Filters available: shouting, mocking, leet.</param>
        /// <param name="quantity">How many gratitude sentences to retrieve. Max 50 per call.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Multiple gratitude sentences. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandomBulk(
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string? category,
            [FromQuery] string[]? filters,
            [FromQuery, DefaultValue(5)] int? quantity,
            [FromQuery, DefaultValue("eng")] string? language,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetBulkGratitudeQuery
            {
                Name = name,
                Signature = signature,
                Category = category,
                Filters = new List<string>(filters),
                Language = language,
                Quantity = quantity ?? 5
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitudes Not Found.");
        }

        /// <summary>
        /// Get a random gratitude sentence, optionally specify a name, a signature and a category, multiple times each with a different filter. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="category">Category to get the random gratitude from.</param>
        /// <param name="different">If true every every gratitude will be different, one for each filter.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Multiple gratitude sentences. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("allfilters")]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetBulkRandomWithAllFilters(
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string? category,
            [FromQuery, DefaultValue(false)] bool? different,
            [FromQuery, DefaultValue("eng")] string? language,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetBulkAllFiltersGratitudeQuery
            {
                Name = name,
                Signature = signature,
                Category = category,
                Different = different ?? false,
                Language = language
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitudes Not Found.");
        }

        /// <summary>
        /// Get a gratitude sentence by Id, optionally specify a name and a signature, multiple times each with a different filter. Thanks!
        /// </summary>
        /// <param name="gratitudeId">Id of the gratitude sentence.</param>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Multiple gratitude sentences. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("allfilters/{gratitudeId:int}")]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetBulkByIdWithAllFilters(
            [FromRoute, Required] int gratitudeId,
            [FromQuery] string? name,
            [FromQuery] string? signature,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetBulkAllFiltersGratitudeByIdQuery
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