﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateGetters.Infrastructure.Extensions;
using RateGetters.Rates.Models;
using RateGetters.Rates.Models.Enums;
using RateGetters.Rates.Services.Interfaces;

namespace RateGetters.Rates.Services
{
    public record CbrRateService : IRateService
    {
        private const string CbrLinkForSingle = "http://www.cbr.ru/scripts/XML_daily.asp";
        private const string CbrLinkForPeriod = "http://www.cbr.ru/scripts/XML_dynamic.asp";

        private readonly RateForDate _emptyRateForDate =
            new(
                new Rate(CurrencyCodesEnum.None, 0m),
                DateTime.MinValue);

        public CbrRateService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<RateForDate> GetRateAsync(DateTime dateTime, CurrencyCodesEnum code)
        {
            if (code == CurrencyCodesEnum.Rub)
            {
                return new RateForDate(
                    new Rate(CurrencyCodesEnum.Rub, 1),
                    dateTime);
            }
            
            var ds = new DataSet();
            ds.ReadXml($"{CbrLinkForSingle}?date_req={dateTime:dd/MM/yyyy}");

            var currencyRows = ds.Tables["Valute"]?.Rows;
            if (currencyRows is null)
            {
                return await Task.FromResult(_emptyRateForDate);
            }

            var requiredRow = currencyRows
                .Cast<DataRow>()
                .FirstOrDefault(row => row["CharCode"].ToString() == code.ToString().ToUpper());

            return await Task.FromResult(
                requiredRow is not null
                    ? new RateForDate(
                        new Rate(code, Convert.ToDecimal(requiredRow["Value"].ToString())),
                        dateTime)
                    : _emptyRateForDate);
        }

        public async Task<PeriodRateList> GetRatesForPeriodAsync(DateTime first, DateTime second,
            CurrencyCodesEnum code)
        {
            var ds = new DataSet();

            ds.ReadXml($"{CbrLinkForPeriod}" +
                       $"?date_req1={(first < second ? first : second):dd/MM/yyyy}" +
                       $"&date_req2={(first > second ? first : second):dd/MM/yyyy}" +
                       $"&VAL_NM_RQ={code.Description()}");

            var currency = ds.Tables["Record"];
            return await Task.FromResult(currency?.Rows is null
                ? new PeriodRateList(new List<RateForDate>())
                : PeriodRateList.Prepare(currency, code));
        }
    }
}