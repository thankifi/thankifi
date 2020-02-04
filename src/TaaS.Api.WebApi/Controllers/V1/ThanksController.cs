using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeById;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandom;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandomByType;
using TaaS.Core.Entity;

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
        /// Get a gratitude sentence by Id, optionally specify a name and a signature. Thanks!
        /// </summary>
        /// <param name="id">Id of the gratitude sentence.</param>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetById(
            [FromRoute, Required] int id,
            [FromQuery, DefaultValue("Alice")] string name = "Alice",
            [FromQuery, DefaultValue("Bob")] string signature = "Bob",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeByIdQuery(id, name, signature), cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a random gratitude sentence, optionally specify a name and a signature. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("random")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandom(
            [FromQuery, DefaultValue("Alice")] string? name = "Alice",
            [FromQuery, DefaultValue("Bob")] string? signature = "Bob",
            [FromQuery, DefaultValue("eng")] string? language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeRandomQuery(language, name, signature), cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a random gratitude basic sentence. No names, no signature. Thanks!
        /// </summary>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("random/basic")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandomBasic(
            [FromQuery, DefaultValue("eng")] string? language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.Basic, language), cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a random gratitude named sentence. Required name, no signature. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("random/named")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandomNamed(
            [FromQuery, Required] string name,
            [FromQuery, DefaultValue("eng")] string? language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.Named, name), cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }
            
            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a random gratitude signed sentence. No name, required signature. Thanks!
        /// </summary>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("random/signed")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandomSigned(
            [FromQuery, Required] string signature,
            [FromQuery, DefaultValue("eng")] string? language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.Signed, language, signature: signature),
                cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitude Not Found.");
        }

        /// <summary>
        /// Get a random gratitude named and signed. Required name, required signature. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="language">Language of the gratitude.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        /// <response code="404">Gratitude not found! Thanks!</response>
        [HttpGet, Route("random/namedAndSigned")]
        [ProducesResponseType(typeof(GratitudeViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetRandomNamedAndSigned(
            [FromQuery, Required] string name,
            [FromQuery, Required] string signature,
            [FromQuery, DefaultValue("eng")] string? language = "eng",
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.NamedAndSigned, language, name, signature),
                cancellationToken);

            if (result != null)
            {
                return Ok(GratitudeViewModel.Parse(result));
            }

            return NotFound("Gratitude Not Found.");
        }
    }
}