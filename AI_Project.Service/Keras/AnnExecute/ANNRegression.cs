using AI_Project.Service.Keras.Options;
using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Project.Service.Keras.AnnExecute
{
    public class ANNRegression : ANNBase
    {
        public ANNRegression(ANNTrainingOptions trainingOptions) : base()
        {
            FillOptions(trainingOptions);
        }
        private void FillOptions(ANNTrainingOptions trainingOptions)
        {
            this.BatchSize = trainingOptions.BatchSize;
            this.EpochNumber = trainingOptions.EpochNumber;
            this.CostFunction = trainingOptions.CostFunction;
            this.Optimizer = trainingOptions.Optimizer;
            this.KernelInitializer = trainingOptions.KernelInitializer;
            this.ActivationFunction = trainingOptions.ActivationFunction;
            this.NumberOfHiddenLayers = trainingOptions.NumberOfHiddenLayers;
            this.NumberOfNeuronsInFirstHiddenLayer = trainingOptions.NumberOfNeuronsInFirstHiddenLayer;
            this.NumberOfNeuronsInOtherHiddenLayers = trainingOptions.NumberOfNeuronsInOtherHiddenLayers;
            this.Verbose = trainingOptions.Verbose;
            this.InputDim = trainingOptions.InputDim;
        }


        public Sequential GetSequentialModel(NDarray ndPredictorData, NDarray ndPredictedData)
        {

            var modelTraining = GetSequentialModel();
            modelTraining.Compile(this.Optimizer, this.CostFunction, new string[] { "accuracy" });
            ndPredictorData = np.asarray(ndPredictorData).astype(np.float32);
            ndPredictedData = np.asarray(ndPredictedData).astype(np.float32);
            //try
            //{
            modelTraining.Fit(ndPredictorData, ndPredictedData, epochs: this.EpochNumber, batch_size: this.BatchSize, verbose: this.Verbose);

            //}
            //catch(Exception e)
            //{
            //    string s = e.Message;
            //}
            return modelTraining;
        }

        private Sequential GetSequentialModel()
        {
            var modelTraining = new Sequential();
            if (this.NumberOfHiddenLayers > 0)
                modelTraining.Add(new Dense(this.NumberOfNeuronsInFirstHiddenLayer, input_dim: this.InputDim, kernel_initializer: this.KernelInitializer, activation: this.ActivationFunction));
            if (this.NumberOfHiddenLayers > 1)
            {
                for (int i = 0; i <= this.NumberOfHiddenLayers - 1; i++)
                {
                    modelTraining.Add(new Dense(this.NumberOfNeuronsInOtherHiddenLayers, kernel_initializer: this.KernelInitializer, activation: this.ActivationFunction));
                }
            }
            modelTraining.Add(new Dense(1, kernel_initializer: this.KernelInitializer));
            return modelTraining;
        }
    }
}
