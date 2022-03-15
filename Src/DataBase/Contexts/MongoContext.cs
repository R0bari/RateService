﻿using DataBase.Extensions;
using DataBase.Models;
using Domain.Contexts;
using Domain.Models.Rates;
using Domain.Models.Rates.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace DataBase.Contexts;

public class MongoContext : IContext
{
    private readonly IGridFSBucket _gridFs;
    private readonly IMongoCollection<RateForDateMongo> _ratesForDate;

    private const string ConnectionString =
        "mongodb://localhost:27017/CurrencyView?readPreference=primary&appname=MongoDB%20Compass&directConnection=true&ssl=false";

    public MongoContext()
    {
        var connection = new MongoUrlBuilder(ConnectionString);
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase(connection.DatabaseName);
        _gridFs = new GridFSBucket(database);
        _ratesForDate = database.GetCollection<RateForDateMongo>("RatesForDate");
    }

    public async Task<RateForDate> GetRateForDate(CurrencyCodesEnum code, DateTime date)
    {
        var filter =
            Builders<RateForDateMongo>.Filter.Eq(r => r.Code, code)
            &
            Builders<RateForDateMongo>.Filter.Eq(r => r.Date, date.Date.PrepareForMongo());
        var result = await _ratesForDate
            .Find(filter)
            .Limit(1)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (result is null)
        {
            return RateForDate.Empty;
        }
        
        return result.Adapt<RateForDate>() with {Date = result.Date.ToLocalTime()};
    }

    public async Task<DateTime> GetMostRecentDate()
    {
        var result = await _ratesForDate
            .Find(new BsonDocument())
            .SortByDescending(r => r.Date)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        return result?.Date.ToLocalTime() ?? new DateTime(1999, 12, 31);
    }

    public async Task<int> InsertRateForDate(RateForDate rateForDate)
    {
        var rateToInsert = rateForDate.Adapt<RateForDateMongo>();
        var result = await _ratesForDate
            .BulkWriteAsync(new WriteModel<RateForDateMongo>[]
            {
                new InsertOneModel<RateForDateMongo>(rateToInsert)
            })
            .ConfigureAwait(false);

        return result.IsAcknowledged ? 1 : -1;
    }

    public async Task<int> InsertRateForDateList(IEnumerable<RateForDate> ratesForDate)
    {
        var ratesList = ratesForDate.ToArray();
        if (!ratesList.Any())
        {
            return -1;
        }

        var models = ratesList
            .Select(rate => rate.Adapt<RateForDateMongo>())
            .Select(rateToInsert => new InsertOneModel<RateForDateMongo>(rateToInsert));
        var result = await _ratesForDate
            .BulkWriteAsync(models)
            .ConfigureAwait(false);

        return result.IsAcknowledged ? 1 : -1;
    }

    public async Task<int> DeleteRate(RateForDate rate)
    {
        var (currencyCodesEnum, _, dateTime) = rate;
        var filter = Builders<RateForDateMongo>.Filter.Eq(r => r.Date, dateTime.Date.PrepareForMongo())
                     &
                     Builders<RateForDateMongo>.Filter.Eq(r => r.Code, currencyCodesEnum);
        var result = await _ratesForDate
            .DeleteOneAsync(filter)
            .ConfigureAwait(false);

        return result.IsAcknowledged ? 1 : -1;
    }

    public async Task<int> DeleteAllRates(bool confirm = false)
    {
        if (!confirm)
        {
            return -1;
        }

        var filter = Builders<RateForDateMongo>.Filter.Empty;
        var deletionResult = await _ratesForDate.DeleteManyAsync(filter);
        return deletionResult.IsAcknowledged ? 1 : -1;
    }
}