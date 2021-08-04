using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.Internal;
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
        private readonly IFilterService _filterService;

        public FlavourController(IFilterService filterService)
        {
            _filterService = filterService;
        }

        /// <summary>
        /// Retrieve a list of all the available flavours. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllFlavours))]
        [ProducesResponseType(typeof(IEnumerable<FlavourViewModel>), StatusCodes.Status200OK)]
        public IActionResult RetrieveAllFlavours(CancellationToken cancellationToken)
        {
            
            return Ok(_filterService.GetAvailableFilterIdentifiers().Select(identifier => new FlavourViewModel
            {
                Flavour = identifier,
                Text = string.Empty
            }));
        }

    }
}