using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpropagationNeuronNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                

                ReadMNIST read = new ReadMNIST("D:\\Source\\C++\\BackpropagationNeuronNetwork\\BackpropagationNeuronNetwork\\bin\\Debug");
                DataSet ds = read.LoadDataMNIST();
                //Backpropagation bnn = new Backpropagation (784, 200, 10);

                //printds(ds);
                
                Backpropagation bnn = new Backpropagation("RecogCharacter.xml");

                //test(bnn,ds);
                testTextFile(bnn);

                double learnRate = 0.05;  // learning rate - controls the maginitude of the increase in the change in weights.
                double momentum = 0.05; // momentum - to discourage oscillation.
                Console.WriteLine("Setting learning rate = " + learnRate.ToString("F2") + " and momentum = " + momentum.ToString("F2"));

                int maxEpochs = 500;
                double errorThresh = 1.0;
                Console.WriteLine("\nSetting max epochs = " + maxEpochs + " and error threshold = " + errorThresh.ToString("F6"));

                int epoch = 0;
                double error = double.MaxValue;
                Console.WriteLine("\nBeginning training using back-propagation\n");

                double[] yValues; // outputs
                while (epoch < maxEpochs) // train
                {
                    error = 0.0;
                    for (int i = 0; i < 50000; i++)
                    {
                        yValues = bnn.ComputeOutputs(ds.DataTrain[i].input);
                        error += Backpropagation.Error(ds.DataTrain[i].output, yValues);
                        
                        bnn.UpdateWeights(ds.DataTrain[i].output,learnRate, momentum);
                        
                    }
                    
                    bnn.Save("RecogCharacter.xml");
                    
                    Console.WriteLine("epoch = "+ epoch + " error = " + error);
                    //if (error < errorThresh)
                    //{
                    //    Console.WriteLine("Found weights and bias values that meet the error criterion at epoch " + epoch);
                    //    break;
                    //}
                    
                    ++epoch;
                } // train loop

                bnn.Save("RecogCharacter.xml");
                
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal: " + ex.Message);
                Console.ReadLine();
            }

        }
        private static void printds(DataSet ds)
        {
            for (int k = 0; k < 20; k++)
            {


                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        Console.Write(" " + ds.DataTrain[k].input[i * 28 + j]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("\n\n");
                int index = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (ds.DataTrain[k].output[i] == 1)
                    {
                        index = i;
                        break;
                    }
                }
                Console.WriteLine(index);
            }
            
        }
        private static void printInput(DataSet ds)
        {
            for (int i = 0; i < ds.DataTrain[0].input.Length; i++)
            {
                if (i % 28 == 0) Console.Write("\n");
                Console.Write(ds.DataTrain[0].input[i]);
                
            }
            Console.Write("\n");
            for (int i = 0; i < ds.DataTrain[0].output.Length; i++)
            {
                if (i % 28 == 0) Console.Write("\n");
                Console.Write(ds.DataTrain[0].output[i]);
            }
        }
        private static void test(Backpropagation bnn, DataSet ds)
        {
            int count = 0;
            double[] yValues;
            for (int i = 0; i < 10000; i++)
            {
                yValues = bnn.ComputeOutputs(ds.DataTest[i].input);
                bool check = false;
                int select = 0;
                for (int j = 1; j < 10; j++)
                {
                    if (Math.Abs(1 - yValues[j]) < Math.Abs(1 - yValues[select]))
                    {
                        select = j;
                    }
                }
                if (ds.DataTest[i].output[select] != 1)
                {
                    int tResult = 0;
                    Console.WriteLine("test case " + i + " is no ");
                    for (int k = 0; k < 10; k++)
                    {
                        Console.WriteLine("   "+yValues[k]);
                        if (ds.DataTest[i].output[k] == 1)
                        {
                            tResult = k;
                        }
                    }
                    Console.WriteLine("       target result = " + tResult);
                }
                else
                {
                    count++;
                    Console.WriteLine("test case " + i + "is yes \n");
                    for (int k = 0; k < 28; k++)
                    {
                        for (int j = 0; j < 28; j++)
                        {
                            Console.Write(ds.DataTest[i].input[k * 28 + j] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("count = " + count);
            double result = count / 10000;
            Console.WriteLine("rate success = " + result);
                
        }

        private static void testTextFile(Backpropagation bnn)
        {
            double[] input = new double[784];
            using (TextReader reader = File.OpenText("write.txt"))
            {
                for (int i = 0; i < 784; i++)
                {
                    input[i] = double.Parse(reader.ReadLine());
                }
                double[] output = bnn.ComputeOutputs(input);
                
            }
        }
    }
}
