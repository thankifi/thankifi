using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Gratitude;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Common.Pagination;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class GratitudeController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        
        public GratitudeController(IQueryBus queryBus, IMapper mapper)
        {
            _queryBus = queryBus;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Retrieve a paginated list of gratitudes.
        /// Optionally specify a subject, a signature and flavours. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllGratitudes))]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveAllGratitudes([FromQuery] RetrieveAllGratitudesQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveAll
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours
            }, cancellationToken);

            Response.Headers.AddPagination(result);

            var gratitudes = result.Select(_mapper.Map<GratitudeViewModel>);

            return Ok(gratitudes);
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
            var result = await _queryBus.Send(new RetrieveById
            {
                Id = id,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours
            }, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            var gratitude = _mapper.Map<GratitudeViewModel>(result);

            return Ok(gratitude);
        }

        /// <summary>
        /// Retrieve a random gratitude repeated once instance per available flavour.
        /// Optionally specify a subject, a signature and flavours. Thanks!
        /// </summary>
        [HttpGet("{id:guid:required}/flavourful",Name = nameof(RetrieveGratitudeByIdFlavourful))]
        [ProducesResponseType(typeof(GratitudeFlavourfulViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveGratitudeByIdFlavourful([FromRoute] Guid id, [FromQuery] RetrieveGratitudeByIdFlavourfulQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveByIdFlavourful
            {
                Id = id,
                Subject = query.Subject,
                Signature = query.Signature,
            }, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            var gratitude = _mapper.Map<GratitudeFlavourfulViewModel>(result);

            return Ok(gratitude);
        }
    }
}
