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
            model.SaveWeight(@"C:\Users\Nikola\Desktop\AI_NN\AI_Proj\AI_Project.Service\Keras\Data\test.h5");
            var predictedValue = model.Predict(np.array(GetPredictorVariables(trainingOptions.PredictorVariablesTest, trainingOptions.PredictorVariablesTest.Count, trainingOptions.PredictorVariablesTest[0].Count))).astype(np.float32);
            results.PredictedValues = ToList(predictedValue);
            return results;
        }

        public ANNResults Predict(ANNTrainingOptions trainingOptions)
        {
            ANNResults results = new ANNResults();
            BaseModel newModel = BaseModel.ModelFromJson(File.ReadAllText(@"C:\Users\Nikola\Desktop\AI_NN\AI_Proj\AI_Project.Service\Keras\Data\test.json"));
            newModel.LoadWeight(@"C:\Users\Nikola\Desktop\AI_NN\AI_Proj\AI_Project.Service\Keras\Data\test.h5");
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

        private void ExportToJson(BaseModel model)
        {
            string jsonModel = model.ToJson();
            string path = @"C:\Users\Nikola\Desktop\AI_NN\AI_Proj\AI_Project.Service\Keras\Data\test.json";

            using(var tw = new StreamWriter(path, true))
            {
                tw.Write(jsonModel);
                tw.Close();
            }
        }
    }
}
