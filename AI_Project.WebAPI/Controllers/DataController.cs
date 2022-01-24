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

        [HttpGet]
        [Route("Predict")]
        public async Task<ActionResult<string>> Predict(string startDate, string endDate)
        {
            string[] sDateStrings = startDate.Split('-');
            DateTime sDate = new DateTime(int.Parse(sDateStrings[0]), int.Parse(sDateStrings[1]), int.Parse(sDateStrings[2]), 0, 0, 0);

            string[] eDateStrings = endDate.Split('-');
            DateTime eDate = new DateTime(int.Parse(eDateStrings[0]), int.Parse(eDateStrings[1]), int.Parse(eDateStrings[2]), 0, 0, 0);

            List<ReturnModel> result = _dataService.PredictLoad(sDate, eDate);
            return Ok(result);
        }


        [HttpGet]
        [Route("Export")]
        public async Task<ActionResult> ExportToCSV()
        {
            await _dataService.ExportToCSV();
            return Ok();
        }
    }
}
