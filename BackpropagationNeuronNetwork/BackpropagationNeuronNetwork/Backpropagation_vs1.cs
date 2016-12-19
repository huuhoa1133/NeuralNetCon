using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BackpropagationNeuronNetwork
{
    #region TransferFunction
	public enum TransferFunction
    {
        None,
        Sigmoid,
        Tanh
    }
    public class TransferFunctions
    {
        public static double Evaluate(TransferFunction tFunc, double input)
        {
            switch (tFunc)
            {
                case TransferFunction.Sigmoid:
                    return sigmoid(input);
                case TransferFunction.Tanh:
                    return tanh(input);
                default: return 0.0;
            }
        }
        public static double EvaluateDerivative(TransferFunction tFunc, double input)
        {
            switch (tFunc)
            {
                case TransferFunction.Sigmoid:
                    return sigmoid_derivative(input);
                case TransferFunction.Tanh:
                    return tanh_derivative(input);
                default: return 0.0;
            }
        }
        private static double sigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }
        private static double sigmoid_derivative(double x)
        {
            return (1 - x) * x;
        }
        private static double tanh(double x)
        {
            if (x < -45.0) return -1.0;
            else if (x > 45.0) return 1.0;
            else return Math.Tanh(x);
        }
        private static double tanh_derivative(double x)
        {
            return (1 - x) * (1 + x);
        }
    }	 
	#endregion
    
    class Backpropagation_vs1
    {
        #region public data
        public string Name = "Default";
        private static Random rand = new Random();
        #endregion
        #region private data

        private int[] layerSize;
        private TransferFunction[] transferFunction;

        private double[][] layerOutput;//value neuron before transfer
        private double[][] layerInput;//value neuron after transfer
        private double[][] bias;
        private double[][] delta;
        private double[][] PreviousBiasDelta;

        private double[][][] weight;
        private double[][][] previousWeightDelta;

        private double[][] Gradient;
        private XmlDocument doc = null;
        private bool loaded = true;
        #endregion 
        #region Method
        public Backpropagation_vs1(int[] layerSize, TransferFunction[] transfer)
        {
            //validate the input data
            if (transfer.Length != layerSize.Length || transfer[0] != TransferFunction.None)
                throw new ArgumentException("cannot construction a network");
            //init layer network

            //coppy layer size
            this.layerSize = new int[layerSize.Length];
            for (int i = 0; i < layerSize.Length; i++)
            {
                this.layerSize[i] = layerSize[i];
            }
            //coppy transferFunction
            this.transferFunction = new TransferFunction[layerSize.Length];
            for (int i = 0; i < layerSize.Length; i++)
			{
                this.transferFunction[i] = transfer[i];
			}
            //init matrix
            layerOutput = new double[layerSize.Length - 1][];
            layerInput = new double[layerSize.Length - 1][];
            Gradient = new double[layerSize.Length - 1][];
            bias = new double[layerSize.Length - 1][];
            delta = new double[layerSize.Length - 1][];
            PreviousBiasDelta = new double[layerSize.Length - 1][];

            weight = new double[layerSize.Length - 1][][];
            previousWeightDelta = new double[layerSize.Length - 1][][];

            //fill matrix 2
            for (int i = 0; i < layerSize.Length-1; i++)
            {
                bias[i] = new double[layerSize[i + 1]];
                PreviousBiasDelta[i] = new double[layerSize[i + 1]];
                delta[i] = new double[layerSize[i + 1]];
                layerOutput[i] = new double[layerSize[i + 1]];
                layerInput[i] = new double[layerSize[i + 1]];
                Gradient[i] = new double[layerSize[i + 1]];

                weight[i] = new double[layerSize[i]][];
                previousWeightDelta[i] = new double[layerSize[i]][];
                for (int j = 0; j < layerSize[i]; j++)
                {
                    weight[i][j] = new double[layerSize[i + 1]];
                    previousWeightDelta[i][j] = new double[layerSize[i + 1]];
                }
            }
            //init the weight
            for (int i = 0; i < layerSize.Length-1; i++)
            {
                for (int j = 0; j < layerSize[i+1]; j++)
                {
                    bias[i][j] = random();
                    PreviousBiasDelta[i][j] = 0.0;
                    delta[i][j] = 0.0;
                }
                for (int k = 0; k < layerSize[i]; k++)
                {
                    for (int j = 0; j < layerSize[i+1]; j++)
                    {
                        weight[i][k][j] = random();
                        previousWeightDelta[i][k][j] = 0.0;
                    }
                }

            }
        }
        //public Backpropagation_vs1(String filePath);
        public double[] Run(double[] input)
        {
            if (input.Length != layerSize[0])
            {
                throw new ArgumentException("inputs data is not correct");
            }
      
            double[] output = new double[layerSize[layerSize.Length - 1]];

            for (int l = 0; l < layerSize.Length-1; l++)
            {
                for (int j = 0; j < layerSize[l+1]; j++)
                {
                    double sum = 0.0;
                    for (int i = 0; i < layerSize[l]; i++)
                    {
                        sum += weight[l][i][j] * (l == 0 ? input[i] : layerOutput[l-1][i]);
                    }
                    sum += bias[l][j];
                    layerInput[l][j] = sum;
                    layerOutput[l][j] = TransferFunctions.Evaluate(transferFunction[l+1], sum);
                }
            }

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = layerOutput[layerSize.Length-2][i];
            }
            return output;
        }
        public void Train(double[] tOutput, double learningRate, double momentum)
        {
            if (tOutput.Length != layerSize[layerSize.Length - 1])
            {
                throw new ArgumentException("inputs data is not correct");
            }
            //compute gradient ouput
            for (int i = 0; i < layerSize[layerSize.Length-1]; i++)
            {
                double derivative = TransferFunctions.EvaluateDerivative(transferFunction[layerSize.Length-1],layerOutput[layerSize.Length - 2][i]);
                Gradient[layerSize.Length - 2][i] = derivative * (tOutput[i] - layerOutput[layerSize.Length - 2][i]);
            }
            //compute gradient
            for (int i = layerSize.Length-2; i > 0; i--)
            {
                for (int j = 0; j < layerSize[i]; j++)
                {
                    double derivative = TransferFunctions.EvaluateDerivative(transferFunction[i], layerOutput[i - 1][j]);
                    double sum = 0.0;
                    for (int k = 0; k < layerSize[i+1]; k++)
                    {
                        sum += Gradient[i][k] * weight[i][j][k];
                    }
                    Gradient[i-1][j] = sum * derivative;
                }
            }

        }
        private double random()
        {
            double result =  (rand.NextDouble() * 0.01);
            return result;
        }
        #endregion
    }
}
