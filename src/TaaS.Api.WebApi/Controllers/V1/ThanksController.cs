using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Query.GetGratitudeById;
using TaaS.Core.Domain.Query.GetGratitudeRandom;
using TaaS.Core.Domain.Query.GetGratitudeRandomByType;
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
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetById(
            [FromRoute, Required] int id,
            [FromQuery, DefaultValue("Alice")] string name = "Alice",
            [FromQuery, DefaultValue("Bob")] string signature = "Bob",
            CancellationToken cancellationToken = default)
        {
            var (_, text) = await Mediator.Send(new GetGratitudeByIdQuery(id, name, signature), cancellationToken);
            return Ok(new GratitudeResponse(id, text));
        }

        /// <summary>
        /// Get a random gratitude sentence, optionally specify a name and a signature. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="language">Language of the gratitude. Currently supported: "en", "es".</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        [HttpGet, Route("random")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetRandom(
            [FromQuery, DefaultValue("Alice")] string? name,
            [FromQuery, DefaultValue("Bob")] string? signature,
            [FromQuery, DefaultValue("en")] string? language,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomQuery(language, name, signature), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }

        /// <summary>
        /// Get a random gratitude basic sentence. No names, no signatures. Thanks!
        /// </summary>
        /// <param name="language">Language of the gratitude. Currently supported: "en", "es".</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        [HttpGet, Route("random/basic")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetRandomBasic(
            [FromQuery, DefaultValue("en")] string? language,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.Basic), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }

        /// <summary>
        /// Get a random gratitude named sentence. Required name, no signatures. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="language">Language of the gratitude. Currently supported: "en", "es".</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        [HttpGet, Route("random/named")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetRandomNamed(
            [FromQuery, Required] string name,
            [FromQuery, DefaultValue("en")] string? language,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.Named, name), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }

        /// <summary>
        /// Get a random gratitude signed sentence. No name, required signature. Thanks!
        /// </summary>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="language">Language of the gratitude. Currently supported: "en", "es".</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        [HttpGet, Route("random/signed")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetRandomSigned(
            [FromQuery, Required] string signature,
            [FromQuery, DefaultValue("en")] string? language,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.Signed, signature: signature), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }

        /// <summary>
        /// Get a random gratitude named and signed. Required name, required signature. Thanks!
        /// </summary>
        /// <param name="name">Name of the person receiving the expression of gratitude.</param>
        /// <param name="signature">Name of the person who expresses their gratitude.</param>
        /// <param name="language">Language of the gratitude. Currently supported: "en", "es".</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Gratitude sentence. Thanks!</response>
        [HttpGet, Route("random/namedAndSigned")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetRandomNamedAndSigned(
            [FromQuery, Required] string name,
            [FromQuery, Required] string signature,
            [FromQuery, DefaultValue("en")] string? language,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomByTypeQuery(GratitudeType.NamedAndSigned, name, signature), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }
    }
}