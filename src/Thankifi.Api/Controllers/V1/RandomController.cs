using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Random;
using Thankifi.Api.Model.V1.Responses;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class RandomController : ControllerBase
    {
        /// <summary>
        /// Retrieve a random gratitude.
        /// Optionally specify a subject, a signature, flavours, categories and languages. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveRandom))]
        [ProducesResponseType(typeof(GratitudeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveRandom([FromQuery] RetrieveRandomQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Retrieve a random gratitude repeated once instance per available flavour.
        /// Optionally specify a subject, a signature, categories and languages. Thanks!
        /// </summary>
        [HttpGet("flavourful",Name = nameof(RetrieveRandomFlavourful))]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveRandomFlavourful([FromQuery] RetrieveRandomFlavourfulQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}