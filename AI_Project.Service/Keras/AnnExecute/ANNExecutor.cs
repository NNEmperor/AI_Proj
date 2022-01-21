using AI_Project.Service.Keras.Options;
using AI_Project.Service.Keras.Results;
using Keras.Models;
using Newtonsoft.Json;
using Numpy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AI_Project.Service.Keras.AnnExecute
{
    public class ANNExecutor
    {
        public ANNResults Run(ANNTrainingOptions trainingOptions)
        {
            ANNResults results = new ANNResults();
            FillOptions(trainingOptions);
            ANNRegressionModelFactory factory = new ANNRegressionModelFactory(trainingOptions);
            var model = factory.GetModel();
            ExportToJson(model);
            BaseModel newModel = BaseModel.ModelFromJson(GetJson());
            var predictedValue = newModel.Predict(np.array(GetPredictorVariables(trainingOptions.PredictorVariablesTest, trainingOptions.PredictorVariablesTest.Count, trainingOptions.PredictorVariablesTest[0].Count))).astype(np.float32);
            results.PredictedValues = ToList(predictedValue);
            return results;
        }

        private List<float> ToList(NDarray arr)
        {
            List<float> list = new List<float>();
            for (int i = 0; i < arr.len; i++)
            {
                list.Add((float)arr[i][0]);
            }
            return list;
        }

        private void FillOptions(ANNTrainingOptions trainingOptions)
        {
            int inputDim = trainingOptions.PredictorVariablesTraining[0].Count;
            trainingOptions.InputDim = inputDim;
            float[] predictedData = trainingOptions.PredictedVariablesTraining.ToArray();
            float[,] predictorData = GetPredictorVariables(trainingOptions.PredictorVariablesTraining, trainingOptions.PredictorVariablesTraining.Count, trainingOptions.PredictorVariablesTraining[0].Count);
            trainingOptions.NdPredictedData = np.array(predictedData);
            trainingOptions.NdPredictorData = np.array(predictorData);
        }

        private float[,] GetPredictorVariables(List<List<float>> predictorVariablesList, int x, int y)
        {
            float[,] predVariables = new float[x, y];
            for (int i = 0; i < predictorVariablesList.Count; i++)
            {
                for (int j = 0; j < predictorVariablesList[i].Count; j++)
                {
                    predVariables[i, j] = predictorVariablesList[i][j];
                }
            }
            return predVariables;
        }

        private float[] ToArray(List<float> valuesList)
        {
            float[] arr = new float[valuesList.Count];
            for (int i = 0; i < valuesList.Count; i++)
            {
                arr[i] = valuesList[i];
            }
            return arr;
        }

        private void ExportToJson(BaseModel model)
        {
            string jsonModel = model.ToJson();
            string path = @"C:\Users\nikola.nikolic\Desktop\Faks\AI\AI_Project_NN\AI_Project.Service\Keras\Data\test.json";

            using(var tw = new StreamWriter(path, true))
            {
                tw.Write(jsonModel);
                tw.Close();
            }
        }

        private string GetJson()
        {
            string jsonString;
            string path = @"C:\Users\nikola.nikolic\Desktop\Faks\AI\AI_Project_NN\AI_Project.Service\Keras\Data\test.json";

            using (var tw = new StreamReader(path, true))
            {
                jsonString = tw.ReadLine();
                tw.Close();
            }

            return jsonString;
        }
    }
}
