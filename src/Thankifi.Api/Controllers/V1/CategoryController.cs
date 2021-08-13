using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Category;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Common.Pagination;
using Thankifi.Core.Domain.Contract.Category.Queries;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        
        public CategoryController(IQueryBus queryBus, IMapper mapper)
        {
            _queryBus = queryBus;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieve a paginated list of all the available categories. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllCategories))]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetrieveAllCategories([FromQuery] QueryStringParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveAll
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }, cancellationToken);

            Response.Headers.AddPagination(result);

            var categories = result.Select(_mapper.Map<CategoryViewModel>);

            return Ok(categories);
        }

        /// <summary>
        /// Retrieve a detail view of a category and a paginated list of gratitudes for the specified category id.
        /// Optionally specify a subject, a signature, flavours and languages. Thanks!
        /// </summary>
        [HttpGet("{id:guid:required}", Name = nameof(RetrieveByCategoryId), Order = 1)]
        [ProducesResponseType(typeof(CategoryDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveByCategoryId([FromRoute] Guid id, [FromQuery] RetrieveByCategoryQueryParameters query,
            CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveById
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Id = id,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours,
                Languages = query.Languages
            }, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }
            
            Response.Headers.AddPagination(result.Gratitudes);

            var category= _mapper.Map<CategoryDetailViewModel>(result);

            return Ok(category);
        }

        /// <summary>
        /// Retrieve a detail view of a category and a paginated list of gratitudes for the specified category slug.
        /// Optionally specify a subject, a signature, flavours and languages. Thanks!
        /// </summary>
        [HttpGet("{slug:required}", Name = nameof(RetrieveByCategorySlug), Order = 2)]
        [ProducesResponseType(typeof(CategoryDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveByCategorySlug([FromRoute] string slug, [FromQuery] RetrieveByCategoryQueryParameters query,
            CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveBySlug
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Slug = slug,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours,
                Languages = query.Languages
            }, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }
            
            Response.Headers.AddPagination(result.Gratitudes);

            var category= _mapper.Map<CategoryDetailViewModel>(result);

            return Ok(category);
        }
    }
}