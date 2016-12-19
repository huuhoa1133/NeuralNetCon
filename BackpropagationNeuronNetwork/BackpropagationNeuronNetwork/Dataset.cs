using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpropagationNeuronNetwork
{
    public class DataPoint
    {
        public const int inputSize = 784;
        public const int outputSize = 10;
        public double[] input;
        public double[] output;

        //method
        public DataPoint()
        {
            input = new double[inputSize];
            output = new double[outputSize];

            //init output 
            for (int i = 0; i < outputSize; i++)
            {
                output[i] = 0;
            }
        }

    }
    public class DataSet
    {
        public List<DataPoint> DataTrain;
        public List<DataPoint> DataTest;


        //method
        public DataSet()
        {
            DataTrain = new List<DataPoint>();
            DataTest = new List<DataPoint>();
        }
    }
}
