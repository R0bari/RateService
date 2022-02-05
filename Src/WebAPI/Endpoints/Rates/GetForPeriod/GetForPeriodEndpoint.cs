﻿using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using CommandLayer.Queries.Rates.GetForPeriodQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RateGetters.Rates.Models;
using RateGetters.Rates.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Endpoints.Rates.GetForPeriod
{
    public class GetForPeriodEndpoint : EndpointBaseAsync
        .WithRequest<GetForPeriodSpecification>
        .WithResult<PeriodRateList>
    {
        private readonly IMediator _mediator;

        public GetForPeriodEndpoint(IMediator mediator) => _mediator = mediator;

        [HttpGet("rates/period")]
        [SwaggerOperation(
            Summary = "Get currency rate for period",
            Description = "Get currency rate for period",
            OperationId = "Rate.GetForPeriod",
            Tags = new[] {"RateEndpoint"})]
        public override async Task<PeriodRateList> HandleAsync(
            [FromQuery] GetForPeriodSpecification specification,
            CancellationToken cancellationToken = default) =>
            await _mediator.Send(
                    new GetForPeriodQuery(specification),
                    cancellationToken)
                .ConfigureAwait(false);
    }
}