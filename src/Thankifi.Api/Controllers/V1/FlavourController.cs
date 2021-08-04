using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Common.Filters.Abstractions;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class FlavourController : ControllerBase
    {
        private readonly IEnumerable<IFilter> _filters;

        public FlavourController(IEnumerable<IFilter> filters)
        {
            _filters = filters;
        }

        /// <summary>
        /// Retrieve a list of all the available flavours. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllFlavours))]
        [ProducesResponseType(typeof(IEnumerable<FlavourViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetrieveAllFlavours(CancellationToken cancellationToken)
        {
            
            return Ok(_filters.Select(f => new FlavourViewModel
            {
                Flavour = f.Identifier,
                Text = string.Empty
            }));
        }

    }
}