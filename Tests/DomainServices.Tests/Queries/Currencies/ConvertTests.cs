using System;
using Domain.Models.Rates.Enums;
using DomainServices.Services.Converters;
using DomainServices.Services.Rates;
using DomainServices.Services.Rates.Interfaces;
using Xunit;

namespace DomainServices.Tests.Queries.Currencies;

public class ConvertTests
{
    private readonly IConverter _converter;
    private readonly IRateService _rateService;

    public ConvertTests()
    {
        _rateService = new CachedCbrRateService();
        _converter =
            new BaseConverter(_rateService);
    }

    [Fact]
    public async void TestConversations()
    {
        var now = DateTime.Today;
        var fromRate = await _rateService
            .GetRateAsync(now, CurrencyCodesEnum.Usd)
            .ConfigureAwait(false);

        var toRate = await _rateService
            .GetRateAsync(now, CurrencyCodesEnum.Eur)
            .ConfigureAwait(false);
        var expectedValue = 100 * fromRate.Value / toRate.Value;

        var actual = await _converter
            .Convert( new ConvertCurrencySpecification(
                CurrencyCodesEnum.Usd,
                CurrencyCodesEnum.Eur,
                100))
            .ConfigureAwait(false);

        Assert.Equal(expectedValue, actual);
    }
}
