﻿using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using RateGetters.Rates.Interfaces;
using RateGetters.Rates.Models;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Endpoints.Rates.GetForDateEndpoint;

namespace WebAPI.Endpoints.Rates.GetForDate
{
    public class GetForDateEndpoint : EndpointBaseAsync
        .WithRequest<GetForDateRequest>
        .WithResult<ActionResult<RateForDate>>
    {
        private readonly IRateGetter _rateGetter;

        public GetForDateEndpoint(IRateGetter rateGetter) => _rateGetter = rateGetter;

        [HttpGet("rates/date")]
        [SwaggerOperation(
            Summary = "Get currency rate for date",
            Description = "Get currency rate for date",
            OperationId = "Rate.GetForDate",
            Tags = new[] {"RateEndpoint"})]
        public override async Task<ActionResult<RateForDate>> HandleAsync(
            [FromQuery] GetForDateRequest request,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(
                    () => new ActionResult<RateForDate>(
                        _rateGetter.GetRate(request.DateTime, request.Code)),
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}