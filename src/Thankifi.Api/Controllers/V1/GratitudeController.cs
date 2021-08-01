using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Gratitude;
using Thankifi.Api.Model.V1.Responses;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class GratitudeController : ControllerBase
    {
        /// <summary>
        /// Retrieve a paginated list of gratitudes.
        /// Optionally specify a subject, a signature and flavours. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllGratitudes))]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveAllGratitudes([FromQuery] RetrieveAllGratitudesQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Retrieve a gratitude by id.
        /// Optionally specify a subject, a signature and flavours. Thanks!
        /// </summary>
        [HttpGet("{id:guid:required}",Name = nameof(RetrieveGratitudeById))]
        [ProducesResponseType(typeof(GratitudeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveGratitudeById([FromRoute] Guid id, [FromQuery] RetrieveGratitudeByIdQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a random gratitude repeated once instance per available flavour.
        /// Optionally specify a subject, a signature and flavours. Thanks!
        /// </summary>
        [HttpGet("{id:guid:required}/flavourful",Name = nameof(RetrieveGratitudeByIdFlavourful))]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveGratitudeByIdFlavourful([FromRoute] Guid id, [FromQuery] RetrieveGratitudeByIdFlavourfulQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
