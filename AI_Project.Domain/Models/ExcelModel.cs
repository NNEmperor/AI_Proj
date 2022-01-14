using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Project.Domain.Models
{
    public class ExcelModel
    {
        public string FileName { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
