using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Query.GetGratitudeRandom;

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
        /// Get a gratitude sentence by Id, optionally specify a name and a signature.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="signature"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(GratitudeResponse), 200)]
        public async Task<IActionResult> GetById(
            [FromRoute, Required] int id,
            [FromQuery, DefaultValue("Alice")] string name,
            [FromQuery, DefaultValue("Bob")] string signature,
            CancellationToken cancellationToken)
        {
            var gratitude = await Mediator.Send(null, cancellationToken);
            return Ok();
        }

        [HttpGet, Route("random")]
        public async Task<IActionResult> GetRandom(
            [FromQuery, DefaultValue("Alice")] string name,
            [FromQuery, DefaultValue("Bob")] string signature,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomQuery(name, signature), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }
        
        [HttpGet, Route("random/basic")]
        public async Task<IActionResult> GetRandomNamed(
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomQuery(), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }

        [HttpGet, Route("random/named")]
        public async Task<IActionResult> GetRandomNamed(
            [FromQuery, DefaultValue("Alice")] string name,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomQuery(name, ""), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }

        [HttpGet, Route("random/signed")]
        public async Task<IActionResult> GetRandomSigned(
            [FromQuery, DefaultValue("Bob")] string signature,
            CancellationToken cancellationToken)
        {
            var (id, text) = await Mediator.Send(new GetGratitudeRandomQuery("", signature), cancellationToken);

            return Ok(new GratitudeResponse(id, text));
        }
    }
}