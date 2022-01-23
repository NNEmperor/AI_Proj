using AI_Project.Domain.Entities;
using AI_Project.Service.Keras.AnnExecute;
using AI_Project.Service.Keras.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Project.Service.Keras
{
    public class Predictor
    {
        private const float frac = 0.9f;

        public async Task Train(List<Weather> weathers)
        {
            var trainingData = GetTrainingData(weathers);
            var predictorData = trainingData.Item1;
            var predictedData = trainingData.Item2;

            int cnt = predictedData.Count;
            int trainCnt = (int)Math.Round((cnt * frac), 0);
            var trainingOptions = new ANNTrainingOptions();

            trainingOptions.PredictorVariablesTraining = predictorData.Take(trainCnt).ToList();
            trainingOptions.PredictorVariablesTest = predictorData.Skip(trainCnt).ToList();


            trainingOptions.PredictedVariablesTraining = predictedData.Take(trainCnt).ToList();
            List<float> predictedVariablesTest = predictedData.Skip(trainCnt).ToList();
            ANNExecutor annExecutor = new ANNExecutor();
            var results = annExecutor.Run(trainingOptions);

            List<float> l1 = new List<float>();
            List<float> l2 = new List<float>();
            foreach (var item in results.PredictedValues)
            {
                l1.Add(item * (ServiceClass.loadMax - ServiceClass.loadMin) + ServiceClass.loadMin);
            }
            foreach (var item in predictedVariablesTest)
            {
                l2.Add(item * (ServiceClass.loadMax - ServiceClass.loadMin) + ServiceClass.loadMin);
            }

            double sqrDeviation = GetSquareDeviation(results.PredictedValues, predictedVariablesTest);
            float sum = 0;
            for (int i = 0; i < results.PredictedValues.Count; i++)
            {
                sum += Math.Abs((l2[i] - l1[i]) / (l2[i]));
            }

            float s = (sum / results.PredictedValues.Count) * 100;

            Console.WriteLine("SQR Deviation: " + sqrDeviation.ToString());
        }

        public async Task<Dictionary<DateTime, float>> Predict(List<Weather> weathers)
        {
            var returnValue = new Dictionary<DateTime, float>();

            var trainingData = GetTrainingData(weathers);
            var predictorData = trainingData.Item1;
            var predictedData = trainingData.Item2;

            var trainingOptions = new ANNTrainingOptions();

            trainingOptions.PredictorVariablesTest = predictorData;
            //List<float> predictedVariablesTest = predictedData;

            ANNExecutor annExecutor = new ANNExecutor();
            var results = annExecutor.Predict(trainingOptions);

            //List<float> l1 = new List<float>();
            //List<float> l2 = new List<float>();
            //foreach (var item in results.PredictedValues)
            //{
            //    l1.Add(item * (ServiceClass.loadMax - ServiceClass.loadMin) + ServiceClass.loadMin);
            //}
            //foreach (var item in predictedVariablesTest)
            //{
            //    l2.Add(item * (ServiceClass.loadMax - ServiceClass.loadMin) + ServiceClass.loadMin);
            //}

            //double sqrDeviation = GetSquareDeviation(results.PredictedValues, predictedVariablesTest);
            //float sum = 0;
            //for (int i = 0; i < results.PredictedValues.Count; i++)
            //{
            //    sum += Math.Abs((l2[i] - l1[i]) / (l2[i]));
            //}

            //float s = (sum / results.PredictedValues.Count) * 100;

            //Console.WriteLine("SQR Deviation: " + sqrDeviation.ToString());

            //return s.ToString();

            int i = 0;
            foreach(Weather w in weathers)
            {
                returnValue.Add(w.DateTimeOfMeasurement, results.PredictedValues[i]);
                i++;
            }

            return returnValue;
        }

        private Tuple<List<List<float>>, List<float>> GetTrainingData(List<Weather> weathers)
        {
            List<List<float>> predictorData = new List<List<float>>();
            List<float> predictedData = new List<float>();

            foreach (Weather w in weathers)
            {
                List<float> rowValues = new List<float>();

                rowValues.Add(w.Cloudiness);
                rowValues.Add(w.Temperature);
                rowValues.Add(w.WindSpeed);
                rowValues.Add(w.Humidity);
                rowValues.Add(w.MonthOfTheYear);
                rowValues.Add(w.DayOfTheWeek);
                rowValues.Add(w.TimeOfDay);

                predictedData.Add(w.ElectricSpending);
                predictorData.Add(rowValues);
            }
            return new Tuple<List<List<float>>, List<float>>(predictorData, predictedData);
        }

        public double GetSquareDeviation(List<float> l1, List<float> l2)
        {
            if (l1.Count != l2.Count)
            {
                throw new Exception("Different lenghts");
            }
            List<double> deviations = new List<double>();
            for (int i = 0; i < l1.Count; i++)
            {
                deviations.Add(Math.Pow((float)(new decimal(l1[i])) - (float)(new decimal(l2[i])), 2));
            }
            return Math.Sqrt(deviations.Average());
        }

    }
}
