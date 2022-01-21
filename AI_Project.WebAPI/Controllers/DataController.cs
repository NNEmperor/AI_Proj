using AI_Project.Domain.Interfaces;
using AI_Project.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Project.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IService _dataService;

        public DataController(IService dataService)
        {
            _dataService = dataService;
        }


        [HttpPost]
        public async Task<ActionResult<bool>> PostData([FromForm] ExcelModel file)
        {
            await _dataService.ImportExcel(file, false);
            return true;
        }

        [HttpPost]
        [Route("PostPredictData")]
        public async Task<ActionResult<bool>> PostPredictData([FromForm] ExcelModel file)
        {
            await _dataService.ImportExcel(file, true);
            return true;
        }

        //GET: api/Data
        [HttpGet]
        public async Task<ActionResult<bool>> GetData()
        {
            await _dataService.StartTraining();
            return Ok(true);
        }

        //[HttpGet]
        //public async Task<ActionResult<string>> Predict(DateTime startData, int numDays)
        //{

        //    return Ok("data");
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportToCSV(string data)
        //{

        //    return Ok();
        //}
    }
}
