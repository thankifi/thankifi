using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Gratitude.Query.GetGratitude;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeById;

namespace TaaS.Api.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class ThanksController : ControllerBase
    {
        protected readonly ILogger<ThanksController> Logger;
        protected readonly IMediator Mediator;

        public ThanksController(ILogger<ThanksController> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        /// <summary>
        /// Get a random gratitude sentence, optionally specify a name, a signature and a category. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="category">Category to get the random gratitude from.</param>
        /// <param name="filters">Filter or filters to apply to the text. Filters available: shouting, mocking, leet.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Get(
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string? category,
            [FromQuery] string[]? filters,
            [FromQuery, DefaultValue("eng")] string language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeQuery
            {
                Name = name,
                Signature = signature,
                Language = language,
                Category = category,
                Filters = new List<string>(filters)
            }, cancellationToken);
            
            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a gratitude sentence by Id, optionally specify a name and a signature. Thanks!
        /// </summary>
        /// <param name="gratitudeId">Id of the gratitude sentence.</param>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="filters">Filter or filters to apply to the text. Filters available: shouting, mocking, leet.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("{gratitudeId:int}")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetById(
            [FromRoute, Required] int gratitudeId,
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string[]? filters,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetGratitudeByIdQuery
            {
                Id = gratitudeId,
                Name = name,
                Signature = signature,
                Filters = new List<string>(filters)
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a random gratitude filtered by category. Thanks!
        /// </summary>
        /// <param name="categoryName">Name/title of the category.</param>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="filters">Filter or filters to apply to the text. Filters available: shouting, mocking, leet.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("{categoryName}")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetByCategoryTitle(
            [FromRoute, Required] string categoryName,
            [FromQuery] string? name,
            [FromQuery] string? signature,
            [FromQuery] string[]? filters,
            [FromQuery, DefaultValue("eng")] string language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeQuery
            {
                Category = categoryName,
                Name = name,
                Signature = signature,
                Language = language,
                Filters = new List<string>(filters)
            }, cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitude Not Found.");
        }
    }
}
