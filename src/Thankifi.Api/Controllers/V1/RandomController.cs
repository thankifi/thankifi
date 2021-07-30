using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1;
using Thankifi.Api.Model.V1.Requests.Gratitude;
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
        /// Retrieve a random gratitude, optionally specify a subject, a signature, flavours, categories and languages. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveRandom))]
        [ProducesResponseType(typeof(GratitudeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveRandom([FromQuery] RetrieveRandomQueryParameters gratitudeByIdQuery, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}