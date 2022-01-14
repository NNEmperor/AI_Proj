using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Project.Service.Keras.Results 
{ 
    public class ANNResults
    {
        private List<float> predictedValue;
        public List<float> PredictedValues { get => predictedValue; set => predictedValue = value; }
    }
}
