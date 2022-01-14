using AI_Project.Domain.Entities;
using AI_Project.Domain.Interfaces;
using AI_Project.Domain.Models;
using AI_Project.Service.Keras;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AI_Project.Service
{
    public class ServiceClass : IService
    {
        private float _lastCloudnes = 100;
        private readonly IWeatherRepository _weatherRepository;

        public static float loadMax;
        public static float loadMin;
        private float tempMax;
        private float tempMin;
        private float humidityMax;
        private float humidityMin;
        private float windSpeedMax;
        private float windSpeedMin;

        public ServiceClass(IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }


        public void ImportExcel(ExcelModel filemodel)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", filemodel.FileName);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                filemodel.FormFile.CopyTo(stream);
            }

            FileInfo existingFile = new FileInfo(path);
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            Dictionary<DateTime, Weather> data = new Dictionary<DateTime, Weather>(20);

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count

                for (int row = 2; row <= rowCount; row++) // skip first row - headers
                {
                    Weather entity = CellToEntity(worksheet, row);
                    if (entity != null)
                        data[entity.DateTimeOfMeasurement] = entity;
                    else
                        continue;
                }
            }

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[6];
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count
                for (int row = 2; row <= rowCount; row++)
                {
                    if (row == 14)
                    {
                        int a = 0;
                    }
                    string date = worksheet.Cells[row, 1].Value.ToString().Split(' ')[0];
                    string time = "";
                    string[] t = worksheet.Cells[row, 2].Value.ToString().Split(' ');
                    if (t[2] == "AM" && t[1] == "12:00:00")
                    {
                        time = "00:00";
                    }
                    else
                    {
                        if (t[1].Split(':')[0].Length == 1)
                        {
                            time = worksheet.Cells[row, 2].Value.ToString().Split(' ')[1].Substring(0, 4);
                            time = "0" + time;
                        }
                        else
                        {
                            time = worksheet.Cells[row, 2].Value.ToString().Split(' ')[1].Substring(0, 5);
                        }
                        if (t[2] == "PM" && time.Split(':')[0] != "12")
                        {
                            int temp = Int32.Parse(time.Split(':')[0]);
                            temp += 12;
                            time = $"{temp}:00";
                        }

                    }

                    float load = float.Parse(worksheet.Cells[row, 4].Value.ToString(), CultureInfo.InvariantCulture);
                    string day = date.Split('/')[1];
                    string month = date.Split('/')[0];
                    string year = date.Split('/')[2];
                    if (Int32.Parse(day) < 10)
                    {
                        day = $"0{day}";
                    }
                    if (Int32.Parse(month) < 10)
                    {
                        month = $"0{month}";
                    }
                    string dateAndTime = $"{month}/{day}/{year} {time}";
                    DateTime dateTime = DateTime.ParseExact(dateAndTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                    if (data.ContainsKey(dateTime))
                    {
                        data[dateTime].ElectricSpending = load;
                    }

                }
            }
            _weatherRepository.AddData(data.Values);
        }

        private Weather CellToEntity(ExcelWorksheet worksheet, int row)
        {
            Weather entity = new Weather();

            if (worksheet.Cells[row, 1].Value != null)
            {
                DateTime time = DateTime.ParseExact(worksheet.Cells[row, 1].Value.ToString(), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                entity.DateTimeOfMeasurement = time;
            }
            else
            {
                return null;
            }

            if (worksheet.Cells[row, 2].Value != null)
            {
                entity.Temperature = float.Parse(worksheet.Cells[row, 2].Value?.ToString());
            }
            else
            {
                return null;
            }

            if (worksheet.Cells[row, 6].Value != null)
            {
                entity.Humidity = float.Parse(worksheet.Cells[row, 6].Value?.ToString());
            }
            else
            {
                return null;
            }

            if (worksheet.Cells[row, 8].Value != null)
            {
                entity.WindSpeed = float.Parse(worksheet.Cells[row, 8].Value?.ToString());
            }
            else
            {
                return null;
            }
            string cell = worksheet.Cells[row, 11].Value?.ToString();

            if (cell == null || cell == "")
            {
                entity.Cloudiness = _lastCloudnes;
            }
            else if (cell == "no clouds")
            {
                entity.Cloudiness = 0;
                _lastCloudnes = entity.Cloudiness;
            }
            else
            {
                string resultString = Regex.Match(cell, @"\d+").Value;
                try
                {
                    entity.Cloudiness = float.Parse(resultString);
                    _lastCloudnes = entity.Cloudiness;
                }
                catch
                {
                    return null;
                }

            }

            entity.DayOfTheWeek = ((float)entity.DateTimeOfMeasurement.DayOfWeek);
            entity.MonthOfTheYear = entity.DateTimeOfMeasurement.Month;

            return entity;
        }

        public void StartTraining()
        {
            Predictor predictor = new Predictor();
            predictor.Predict(GetScaledData());
        }

        private List<Weather> GetScaledData()
        {
            List<Weather> normalValues = _weatherRepository.GetData().ToList();
            List<Weather> scaledValues = new List<Weather>();

            loadMax = normalValues.Max(w => w.ElectricSpending);
            loadMin = normalValues.Min(w => w.ElectricSpending);
            tempMax = normalValues.Max(w => w.Temperature);
            tempMin = normalValues.Min(w => w.Temperature);
            humidityMax = normalValues.Max(w => w.Humidity);
            humidityMin = normalValues.Min(w => w.Humidity);
            windSpeedMax = normalValues.Max(w => w.WindSpeed);
            windSpeedMin = normalValues.Min(w => w.WindSpeed);

            foreach (Weather weather in normalValues)
            {
                Weather scaledWeather = new Weather
                {
                    DateTimeOfMeasurement = weather.DateTimeOfMeasurement,
                    Cloudiness = weather.Cloudiness / 100,

                    ElectricSpending = (weather.ElectricSpending - loadMin) / (loadMax - loadMin),
                    Temperature = (weather.Temperature - tempMin) / (tempMax - tempMin),
                    Humidity = (weather.Humidity - humidityMin) / (humidityMax - humidityMin),
                    WindSpeed = (weather.WindSpeed - windSpeedMin) / (windSpeedMax - windSpeedMin),

                    DayOfTheWeek = weather.DayOfTheWeek / 6,
                    MonthOfTheYear = (weather.MonthOfTheYear - 1) / 11
                };


                scaledValues.Add(scaledWeather);
            }

            return scaledValues;
        }
    }
}
