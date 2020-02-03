using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Category.Query.GetAllCategories;

namespace TaaS.Api.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        protected readonly ILogger<ThanksController> Logger;
        protected readonly IMediator Mediator;

        public CategoryController(ILogger<ThanksController> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }
        
        /// <summary>
        /// Get a list of all the categories available in TaaS.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Categories list.. Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), 200)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetAllCategoriesQuery(), cancellationToken);

            return Ok(CategoryViewModel.Parse(result));
        }
    }
}