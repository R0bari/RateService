﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Domain.Models.Rates;
using DomainServices.Queries.Currencies.Rates.GetForDate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Endpoints.Rates
{
    public class GetForDateEndpoint : EndpointBaseAsync
        .WithRequest<GetForDateSpecification>
        .WithResult<RateForDate>
    {
        private readonly IMediator _mediator;
        public GetForDateEndpoint(IMediator mediator) =>
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet("rates/date")]
        [SwaggerOperation(
            Summary = "Get currency rate for date",
            Description = "Get currency rate for date",
            OperationId = "Rate.GetForDate",
            Tags = new[] {"RateEndpoint"})]
        public override async Task<RateForDate> HandleAsync(
            [FromQuery] GetForDateSpecification specification,
            CancellationToken cancellationToken = default) =>
            await _mediator
                .Send(new GetForDateQuery(specification), cancellationToken)
                .ConfigureAwait(false);
    }
}