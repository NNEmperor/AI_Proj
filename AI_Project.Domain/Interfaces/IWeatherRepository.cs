using AI_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Project.Domain.Interfaces
{
    public interface IWeatherRepository
    {
        void AddData(IEnumerable<Weather> data);
        IEnumerable<Weather> GetData();
        IEnumerable<Weather> GetDataForPrediction(DateTime startDate, DateTime endDate);
    }
}
