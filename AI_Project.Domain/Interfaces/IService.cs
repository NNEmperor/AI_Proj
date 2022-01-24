using AI_Project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI_Project.Domain.Interfaces
{
    public interface IService
    {
        Task ImportExcel(ExcelModel filemodel, bool isPredictData);
        Task StartTraining();
        Task<List<ReturnModel>> PredictLoad(DateTime startDate, DateTime endDate);
        Task ExportToCSV();
    }
}
