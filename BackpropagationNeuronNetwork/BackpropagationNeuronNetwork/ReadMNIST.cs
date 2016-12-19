using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpropagationNeuronNetwork
{
    public class ReadMNIST
    {
        //fields
        private string filePath;//MNIST file path
        public DataSet Data;
        public static int testSize = 10000;
        public static int trainSize = 50000;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (!value.EndsWith(@"\"))
                {
                    filePath = value + @"\";
                }
                else
                {
                    filePath = value;
                }
            }
        }
        //constructor
        public ReadMNIST()
        {
            Data = new DataSet();
            filePath = "";

        }
        public ReadMNIST(string path)
        {
            Data = new DataSet();

            if (!path.EndsWith(@"\"))
            {
                filePath = path + @"\";
            }
            else
            {
                filePath = path;
            }
        }
        public ReadMNIST(DataSet ds, string path)
        {
            Data = ds;
            if (!path.EndsWith(@"\"))
            {
                filePath = path + @"\";
            }
            else
            {
                filePath = path;
            }
        }
        //method
        public DataSet LoadDataMNIST()
        {
            string testImagesPath = filePath + "t10k-images.idx3-ubyte";
            string testLabelsPath = filePath + "t10k-labels.idx1-ubyte";
            string trainingImagesPath = filePath + "train-images.idx3-ubyte";
            string trainingLabelsPath = filePath + "train-labels.idx1-ubyte";

            LoadData(Data.DataTrain, trainingImagesPath, trainingLabelsPath, trainSize);
            LoadData(Data.DataTest, testImagesPath, testLabelsPath, testSize);

            return Data;
        }
        private void LoadData(List<DataPoint> dp, string imagePath, string labelPath, int datasize)
        {
            try
            {
                FileStream fsLabels = new FileStream(labelPath, FileMode.Open);
                FileStream fsImages = new FileStream(imagePath, FileMode.Open);
                BinaryReader brLabels = new BinaryReader(fsLabels);
                BinaryReader brImages = new BinaryReader(fsImages);

                //parse image
                int magic1 = brImages.ReadInt32();
                int numImages = brImages.ReadInt32();
                int numRows = brImages.ReadInt32();
                int nubCols = brImages.ReadInt32();

                //parse labels
                int magic2 = brLabels.ReadInt32();
                int numLabels = brLabels.ReadInt32();

                DataPoint temp;
                for (int di = 0; di < datasize; di++)
                {
                    //read image
                    temp = new DataPoint();
                    for (int i = 0; i < 28; i++)
                    {
                        for (int j = 0; j < 28; j++)
                        {
                            var b = (byte)brImages.ReadByte();
                            if (b > 30)
                            {
                                temp.input[i * 28 + j] = 1;
                            }
                            else
                            {
                                temp.input[i * 28 + j] = 0;
                            }
                        }
                    }
                    //read label
                    string label = brLabels.ReadByte().ToString();
                    int ioutput;//lebel output
                    Int32.TryParse(label, out ioutput);
                    temp.output[ioutput] = 1;

                    dp.Add(temp);

                    //print image and label

                    //string outp = "";
                    //for (int k1 = 0; k1 < 28; k1++)
                    //{
                    //    for (int k2 = 0; k2 < 28; k2++)
                    //    {
                    //        if (temp.input[k1 * 28 + k2] == 1)
                    //        {
                    //            outp += ".";
                    //        }
                    //        else
                    //        {
                    //            outp += " ";
                    //        }
                    //    }
                    //    outp += "\n";
                    //}
                    //for (int k3 = 0; k3 < 10; k3++)
                    //{
                    //    outp += temp.output[k3] + " ";
                    //}
                    //Console.WriteLine(outp);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("parse image error");
            }
        }

    }
}
