﻿using AI_Project.Domain.Entities;
using AI_Project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Project.Persistence.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly DataDbContext _dataContext;
        public WeatherRepository(DataDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void AddData(IEnumerable<Weather> data)
        {
            foreach (var item in data)
            {
                try
                {
                    _dataContext.Data.Add(item);

                }
                catch (Exception e)
                {
                    string r = e.Message;
                }
            }
            _dataContext.SaveChanges();

        }

        public IEnumerable<Weather> GetData()
        {
            return _dataContext.Data.ToList();
        }

        public IEnumerable<Weather> GetDataForPrediction(DateTime startDate, DateTime endDate)
        {
            return _dataContext.Data.Where(data =>/* data.ElectricSpending == -1 &&*/
            data.DateTimeOfMeasurement >= startDate &&
            data.DateTimeOfMeasurement <= endDate)
                .ToList();
        }
    }
}
