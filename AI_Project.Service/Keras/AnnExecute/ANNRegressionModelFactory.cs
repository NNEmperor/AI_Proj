using AI_Project.Service.Keras.Options;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Project.Service.Keras.AnnExecute
{
    public class ANNRegressionModelFactory
    {
        private ANNTrainingOptions trainingOptions;

        public ANNRegressionModelFactory(ANNTrainingOptions trainingOptions)
        {
            this.trainingOptions = trainingOptions;
        }

        public BaseModel GetModel()
        {
            var ann = new ANNRegression(this.trainingOptions);
            var model = ann.GetSequentialModel((NDarray)trainingOptions.NdPredictorData, (NDarray)trainingOptions.NdPredictedData);
            return model;
        }
    }
}
