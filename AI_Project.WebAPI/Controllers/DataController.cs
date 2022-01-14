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
        public void PostData([FromForm] ExcelModel file)
        {
            _dataService.ImportExcel(file);
        }

        //GET: api/Data
        [HttpGet]
        public void GetData()
        {
            _dataService.StartTraining();
        }
    }
}
