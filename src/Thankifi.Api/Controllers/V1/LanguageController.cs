using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Language;
using Thankifi.Api.Model.V1.Responses;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class LanguageController : ControllerBase
    {
        /// <summary>
        /// Retrieve a paginated list of all the supported languages. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllLanguages))]
        [ProducesResponseType(typeof(IEnumerable<LanguageViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetrieveAllLanguages(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a detail view of a language and a paginated list of gratitudes for the specified language id.
        /// Optionally specify a subject, a signature, flavours and categories. Thanks!
        /// </summary>
        [HttpGet("{id:guid:required}", Name = nameof(RetrieveByLanguageId), Order = 1)]
        [ProducesResponseType(typeof(LanguageDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveByLanguageId([FromRoute] Guid id, [FromQuery] RetrieveByLanguageQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a detail view of a language and a paginated list of gratitudes for the specified language code.
        /// Optionally specify a subject, a signature, flavours and categories. Thanks!
        /// </summary>
        [HttpGet("{code:required}", Name = nameof(RetrieveByLanguageCode), Order = 2)]
        [ProducesResponseType(typeof(LanguageDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveByLanguageCode([FromRoute, Required] string code, [FromQuery] RetrieveByLanguageQueryParameters query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}