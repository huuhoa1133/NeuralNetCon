using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public class DataPoint
{
    public DataPoint() {
        input = new double[784];
        output = new double[14];
    }
    public DataPoint(double[] inputs, double[] outputs)
    {
        Load(inputs, outputs);
    }
    public DataPoint(XmlElement Elem)
    {
        Load(Elem);
    }
    public void Load(double[] inputs, double[] outputs)
    {
        this.input = new double[inputs.Length];
        this.output = new double[outputs.Length];

        Array.Copy(inputs, input, input.Length);
        Array.Copy(outputs, output, output.Length);
    }
    public void Load(XmlElement elm)
    {
        XmlNode nType;
        int lIn, lOut, i;
        nType = elm.SelectSingleNode("Input");
        lIn = nType.ChildNodes.Count;
        input = new double[lIn];

        foreach (XmlNode node in nType.ChildNodes)
        {
            XmlElement Node = (XmlElement)node;
            double val;
            int.TryParse(Node.GetAttribute("Index"), out i);
            double.TryParse(Node.InnerText, out val);

            input[i] = val;
        }

        nType = elm.SelectSingleNode("Output");
        lOut = nType.ChildNodes.Count;
        output = new double[lOut];

        foreach (XmlNode node in nType.ChildNodes)
        {
            XmlElement Node = (XmlElement)node;
            double val;
            int.TryParse(Node.GetAttribute("Index"), out i);
            double.TryParse(Node.InnerText, out val);

            output[i] = val;
        }
    }
    public XmlElement ToXml(XmlDocument doc)
    {
        XmlElement nDataPoint, nType, node;
        int lIn = input.Length;
        int lOut = output.Length;
        nDataPoint = doc.CreateElement("DataPoint");
        nType = doc.CreateElement("Input");
        for (int i = 0; i < lIn; i++)
        {
            node = doc.CreateElement("Data");
            node.SetAttribute("Index", i.ToString());
            node.AppendChild(doc.CreateTextNode(input[i].ToString()));
            nType.AppendChild(node);
        }
        nDataPoint.AppendChild(nType);

        nType = doc.CreateElement("Output");
        for (int i = 0; i < lOut; i++)
        {
            node = doc.CreateElement("Data");
            node.SetAttribute("Index", i.ToString());
            node.AppendChild(doc.CreateTextNode(output[i].ToString()));
            nType.AppendChild(node);
        }
        nDataPoint.AppendChild(nType);
        return nDataPoint;
    }
    

    public double[] input, output;
    public int inputSzie { get { return input.Length; } }
    public int outputSize { get { return output.Length; } }
}

public class DataSet
{
    public DataSet() 
    { 
        Data = new List<DataPoint>();
        DataTrain = new List<DataPoint>();
        DataTest = new List<DataPoint>();
    }
    public XmlElement ToXml(XmlDocument doc)
    {
        XmlElement nDataSet; nDataSet = doc.CreateElement("DataSet");
        foreach (DataPoint d in Data)
        {
            nDataSet.AppendChild(d.ToXml(doc));
        }
        return nDataSet;
    }
    public void Load(XmlElement nDataSet)
    {
        foreach (XmlNode node in nDataSet.ChildNodes)
        {
            DataPoint d = new DataPoint((XmlElement)node);
            Data.Add(d);
        }
    }
    public void CoppyDataset(DataSet ds)
    {
        for (int i = 0; i < ds.Data.Count; i++)
        {
            this.DataTrain.Add(ds.Data[i]);
            this.DataTest.Add(ds.Data[i]);
        }
    }
    public List<DataPoint> Data;
    public List<DataPoint> DataTrain;
    public List<DataPoint> DataTest;
    public int Size { get { return Data.Count; } }
}