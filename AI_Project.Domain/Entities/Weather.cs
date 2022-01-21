using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Project.Domain.Entities
{
    public class Weather
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTimeOfMeasurement { get; set; }
        public float Temperature { get; set; }
        public float Cloudiness { get; set; }
        public float Humidity { get; set; }
        public float WindSpeed { get; set; }
        public float ElectricSpending { get; set; }
        public float DayOfTheWeek { get; set; }
        public float MonthOfTheYear { get; set; }
        public float TimeOfDay { get; set; }
    }
}
