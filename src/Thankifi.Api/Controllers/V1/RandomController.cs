using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Random;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class RandomController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        
        public RandomController(IQueryBus queryBus, IMapper mapper)
        {
            _queryBus = queryBus;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Retrieve a random gratitude.
        /// Optionally specify a subject, a signature, flavours, categories and languages. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveRandom))]
        [ProducesResponseType(typeof(GratitudeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveRandom([FromQuery] RetrieveRandomQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveRandom
            {
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours,
                Categories = query.Categories,
                Languages = query.Languages
            }, cancellationToken);

            var gratitude = _mapper.Map<GratitudeViewModel>(result);

            return Ok(gratitude);
        }
        
        /// <summary>
        /// Retrieve random gratitudes in bulk up to 50 per call.
        /// Optionally specify a subject, a signature, flavours, categories and languages. Thanks!
        /// </summary>
        [HttpGet("bulk", Name = nameof(RetrieveRandomBulk))]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveRandomBulk([FromQuery] RetrieveRandomBulkQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveRandomBulk()
            {
                Quantity = query.Quantity,
                Subject = query.Subject,
                Signature = query.Signature,
                Flavours = query.Flavours,
                Categories = query.Categories,
                Languages = query.Languages
            }, cancellationToken);

            var gratitudes = result.Select(_mapper.Map<GratitudeViewModel>);

            return Ok(gratitudes);
        }
        
        /// <summary>
        /// Retrieve a random gratitude repeated once instance per available flavour.
        /// Optionally specify a subject, a signature, categories and languages. Thanks!
        /// </summary>
        [HttpGet("flavourful", Name = nameof(RetrieveRandomFlavourful))]
        [ProducesResponseType(typeof(IEnumerable<GratitudeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveRandomFlavourful([FromQuery] RetrieveRandomFlavourfulQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await _queryBus.Send(new RetrieveRandomFlavourful()
            {
                Subject = query.Subject,
                Signature = query.Signature,
                Categories = query.Categories,
                Languages = query.Languages
            }, cancellationToken);

            var gratitude = _mapper.Map<GratitudeFlavourfulViewModel>(result);

            return Ok(gratitude);
        }
    }
}