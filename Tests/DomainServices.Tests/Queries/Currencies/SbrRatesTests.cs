using System;
using System.Collections.Generic;
using Domain.Models.Rates;
using Domain.Models.Rates.Enums;
using DomainServices.Services.Rates;
using DomainServices.Services.Rates.Interfaces;
using Xunit;

namespace DomainServices.Tests.Queries.Currencies;

public class SbrRatesTests
{
    private readonly IRateService _rateService;

    public SbrRatesTests() => _rateService = new CachedCbrRateService();

    [Theory]
    [InlineData(76.4849, CurrencyCodesEnum.Usd, 2022, 02, 03)]
    [InlineData(86.2826, CurrencyCodesEnum.Eur, 2022, 02, 03)]
    [InlineData(1, CurrencyCodesEnum.Rub, 2022, 02, 03)]
    public async void TestCurrenciesRates(
        decimal expectedValue,
        CurrencyCodesEnum code,
        int year,
        int month,
        int day)
    {
        var expected = new RateForDate(
            code,
            expectedValue,
            new DateTime(year, month, day));

        var actual = await _rateService
            .GetRateAsync(
                new DateTime(year, month, day),
                code)
            .ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void TestFailedCbrGetRateForDateResult()
    {
        var expected = RateForDate.Empty;

        var actual = await _rateService
            .GetRateAsync(
                new DateTime(1021, 06, 1),
                CurrencyCodesEnum.Eur)
            .ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void TestCbrGetRateForPeriodResult()
    {
        var expected = new RateForDateList(
            new List<RateForDate>
            {
                new(
                    CurrencyCodesEnum.Usd,
                    74.8926m,
                    new DateTime(2021, 12, 01)),
                new(
                    CurrencyCodesEnum.Usd,
                    73.9746m,
                    new DateTime(2021, 12, 02)),
                new(
                    CurrencyCodesEnum.Usd,
                    74.0637m,
                    new DateTime(2021, 12, 03)),
                new(
                    CurrencyCodesEnum.Usd,
                    73.7426m,
                    new DateTime(2021, 12, 04))
            });

        var actual = await _rateService
            .GetRatesForPeriodAsync(
                new DateTime(2021, 12, 01),
                new DateTime(2021, 12, 06),
                CurrencyCodesEnum.Usd)
            .ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void TestCbrGetRateForPeriodWithHolidays()
    {
        var expected = new RateForDateList(
            new List<RateForDate>
            {
                new(
                    CurrencyCodesEnum.Usd,
                    74.8926m,
                    new DateTime(2021, 12, 01)),
                new(
                    CurrencyCodesEnum.Usd,
                    73.9746m,
                    new DateTime(2021, 12, 02)),
                new(
                    CurrencyCodesEnum.Usd,
                    74.0637m,
                    new DateTime(2021, 12, 03)),
                new(
                    CurrencyCodesEnum.Usd,
                    73.7426m,
                    new DateTime(2021, 12, 04))
            });

        var actual = await _rateService
            .GetRatesForPeriodAsync(
                new DateTime(2021, 12, 01),
                new DateTime(2021, 12, 06),
                CurrencyCodesEnum.Usd)
            .ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }
}
