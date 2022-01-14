using AI_Project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Project.Domain.Interfaces
{
    public interface IService
    {
        void ImportExcel(ExcelModel filemodel);
        void StartTraining();
    }
}
