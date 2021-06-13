using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace NeuroNetwork
{
    public class NeuralNetwork
    {
           
        public int inputNodesNum;
        public int hiddenNodesNum;
        public int outputNodesNum;
        public double learningRate;     
        public Matrix<double> weights_inputXhidden;
        public Matrix<double> weights_hiddenXoutput;

        public Matrix<double> ActivationFunction(Matrix<double> matrix)
        {
            return matrix.Map(SpecialFunctions.Logistic); //this is sigmoid function
        }

        public NeuralNetwork(int inputNodesNum, int hiddenNodesNum, int outputNodesNum, double learningRate)
        {
            this.inputNodesNum = inputNodesNum;
            this.hiddenNodesNum = hiddenNodesNum;
            this.outputNodesNum = outputNodesNum;
            this.learningRate = learningRate;
            
            Normal normalDist = new Normal(0.0, Math.Pow(hiddenNodesNum, -0.5));
            weights_inputXhidden = Matrix<double>.Build.Dense(hiddenNodesNum, inputNodesNum, (i, j) => normalDist.Sample());

            normalDist = new Normal(0.0, Math.Pow(outputNodesNum, -0.5));
            weights_hiddenXoutput = Matrix<double>.Build.Dense(outputNodesNum, hiddenNodesNum, (i, j) => normalDist.Sample());
            
        }
        public NeuralNetwork(int inputNodesNum, int hiddenNodesNum, int outputNodesNum, double learningRate, double[] wIxH, double[] wHxO)
        {
            this.inputNodesNum = inputNodesNum;
            this.hiddenNodesNum = hiddenNodesNum;
            this.outputNodesNum = outputNodesNum;
            this.learningRate = learningRate;
            
            weights_inputXhidden = Matrix<double>.Build.Dense(hiddenNodesNum, inputNodesNum, wIxH);            
            weights_hiddenXoutput = Matrix<double>.Build.Dense(outputNodesNum, hiddenNodesNum, wHxO);
        }
        public void Train(double[] inputArray, double[] targetArray)
        {   
            //To multiply matrix: m1(x,y).m2(y,z)=m3(x,z)            
            Matrix<double> inputsM = CreateMatrix.Dense(1, inputArray.Length, inputArray);//inputsM is 1x784 
            Matrix<double> targetM = CreateMatrix.Dense(1, targetArray.Length, targetArray);// targetM is 1x10
            //weights_inputXhidden is 100x784
            //weights_hiddenXoutput is 10x100
            Matrix<double> hidden_inputs = weights_inputXhidden.TransposeAndMultiply(inputsM);//hidden_inputs is (100x784).(1x784.T)= 100x1          
            Matrix<double> hidden_outputs = ActivationFunction(hidden_inputs);//hidden_outputs is 100x1           
            Matrix<double> final_inputs = weights_hiddenXoutput.Multiply(hidden_outputs);//final_inputs is (10x100).(100x1)= 10x1         
            Matrix<double> final_outputs = ActivationFunction(final_inputs);//final_outputs is 10x1
            Matrix<double> output_errors = targetM.Transpose() - final_outputs;//output_errors is (1x10.T)-(10x1)= 10x1         
            Matrix<double> hidden_errors = weights_hiddenXoutput.TransposeThisAndMultiply(output_errors);//hidden_errors is (10x100.T).(10x1)= 100x1

            Matrix<double> Aux = output_errors.PointwiseMultiply(final_outputs).PointwiseMultiply(1 - final_outputs);//Aux is (10x1)*(10x1)*(1 - 10x1)= 10x1  
            // Aux is delta
            //weights_hiddenXoutput is 10x100 = (10x100) + learningRate * [(10x1).(100x1.T)=(10x100)]
            weights_hiddenXoutput += learningRate * Aux.Multiply(hidden_outputs.Transpose());
            
            Aux = hidden_errors.PointwiseMultiply(hidden_outputs).PointwiseMultiply(1 - hidden_outputs);//Aux is (100x1)*(100x1)*(1 - 100x1)= 100x1  
            // Aux is delta
            //weights_inputXhidden is 100x784 = (100x784) + learningRate * (100x1).(1x784)=(100x784)
            weights_inputXhidden += learningRate * Aux.Multiply(inputsM);
        }
        public double[] Query(double[] inputArray)
        {   //To multiply matrix: m1(x,y).m2(y,z)=m3(x,z)
            //MNISTHandler.PrintArray(inputArray);
            //Console.ReadKey();
            //inputsM is 1x784
            Matrix<double> inputsM = CreateMatrix.Dense(1, inputArray.Length, inputArray);
            //weights_ixh is 100x784 needs transpose for (1x784).(784x100)=(1x100)
            Matrix<double> hidden_inputs = inputsM.TransposeAndMultiply(weights_inputXhidden);
            //hidden_in is 1x100 hidden_out is 1x100
            Matrix<double> hidden_outputs = ActivationFunction(hidden_inputs);
            //weights_hxo is 10x100 needs transpose for (1x100).(100x10)=(1x10)
            Matrix<double> final_inputs = hidden_outputs.TransposeAndMultiply(weights_hiddenXoutput);
            //final_in is 1x10
            Matrix<double> final_outputs = ActivationFunction(final_inputs);
            //final_out is 1x10
            //Console.WriteLine(final_outputs.ToString());
            double[] output = new double[10];
            for(int i = 0; i < 10; i++)
            {
                output[i] = final_outputs[0, i];
            }
            //MNISTHandler.PrintArray(output);
            return output;   
        }
        private static void SaveMatrix(int rows, int cols, double[,] matrix, BinaryWriter bw)
        {
            bw.Write(MNISTHandler.ReverseBytes(2050));
            bw.Write(MNISTHandler.ReverseBytes(rows));
            bw.Write(MNISTHandler.ReverseBytes(cols));
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    bw.Write(matrix[j, i]);
                }
            }
            bw.Close();
        }
        public static void SaveNeuroNet(string path, NeuralNetwork nn)
        {
            FileStream wIxH = new FileStream(path + @"\weights_inputXhidden.idx2-ubyte", FileMode.Create);
            BinaryWriter bw_wIxH = new BinaryWriter(wIxH);
            SaveMatrix(nn.hiddenNodesNum, nn.inputNodesNum, nn.weights_inputXhidden.ToArray(), bw_wIxH);
            wIxH.Close();

            FileStream wHxO = new FileStream(path + @"\weights_hiddenXoutput.idx2-ubyte", FileMode.Create);
            BinaryWriter bw_wHxO = new BinaryWriter(wHxO);
            SaveMatrix(nn.outputNodesNum, nn.hiddenNodesNum, nn.weights_hiddenXoutput.ToArray(), bw_wHxO);
            bw_wHxO.Close();
        }
        private static double[] LoadMatrixA(int rows, int cols, BinaryReader br)
        {
            int magic1 = MNISTHandler.ReverseBytes(br.ReadInt32()); //stored as big endian need to convert to Intel format
            if (magic1 != 2050) throw new Exception("Stored NeuroNet is invalid! Magic number 1 does not match.");

            int numRows = MNISTHandler.ReverseBytes(br.ReadInt32());
            if (numRows != rows) throw new Exception("Stored NeuroNet is invalid! wIxH number of rows does not match.");

            int numCols = MNISTHandler.ReverseBytes(br.ReadInt32());
            if (numCols != cols) throw new Exception("Stored NeuroNet is invalid! wIxH number of colums does not match.");

            double[] m = new double[rows * cols];

            for (int i = 0; i < rows * cols; ++i)
            {
                m[i] = br.ReadDouble();
            }

            return m;
        }
        public static NeuralNetwork LoadNeuroNet(int inputNumb, int hiddenNumb, int outputNumb, double learningRate, string path)
        {
            FileStream wIxH = new FileStream(path + @"\weights_inputXhidden.idx2-ubyte", FileMode.Open);
            BinaryReader br_wIxH = new BinaryReader(wIxH);
            double[] m1 = LoadMatrixA(hiddenNumb, inputNumb, br_wIxH);

            FileStream wHxO = new FileStream(path + @"\weights_hiddenXoutput.idx2-ubyte", FileMode.Open);
            BinaryReader br_wHxO = new BinaryReader(wHxO);
            double[] m2 = LoadMatrixA(outputNumb, hiddenNumb, br_wHxO);

            return new NeuralNetwork(inputNumb, hiddenNumb, outputNumb, learningRate, m1, m2);
        }
    }
}
