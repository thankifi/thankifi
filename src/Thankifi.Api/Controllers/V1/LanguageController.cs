using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Language;
using Thankifi.Api.Model.V1.Responses;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Language.Queries;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        
        public LanguageController(IQueryBus queryBus, IMapper mapper)
        {
            _queryBus = queryBus;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Retrieve a paginated list of all the supported languages. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllLanguages))]
        [ProducesResponseType(typeof(IEnumerable<LanguageViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetrieveAllLanguages([FromQuery] QueryStringParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveAll
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }, cancellationToken);

            Response.Headers.AddPagination(result);

            var languages = result.Select(_mapper.Map<LanguageViewModel>);

            return Ok(languages);
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
            var result = await _queryBus.Send(new RetrieveById
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Id = id,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours,
                Categories = query.Categories
            }, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }
            
            Response.Headers.AddPagination(result.Gratitudes);

            var language = _mapper.Map<LanguageDetailViewModel>(result);

            return Ok(language);
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
            var result = await _queryBus.Send(new RetrieveByCode
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Code = code,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours,
                Categories = query.Categories
            }, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }
            
            Response.Headers.AddPagination(result.Gratitudes);

            var language = _mapper.Map<LanguageDetailViewModel>(result);

            return Ok(language);
        }
    }
}