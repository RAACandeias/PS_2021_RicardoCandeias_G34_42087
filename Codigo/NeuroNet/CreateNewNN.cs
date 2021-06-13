using MathNet.Numerics.LinearAlgebra;
using System;

namespace NeuroNetwork
{
    class CreateNewNN
    {
        public static void CreateNewNNAndTrain(int eachDigitQuantity)
        {          
            string savePath = @".\data\neuro net";
            string mnistDatasetPath = @".\data\dataset";
            MNISTHandler mh = new MNISTHandler(mnistDatasetPath, null, eachDigitQuantity);
            mh.LoadTrainData();
            NeuralNetwork nn = new NeuralNetwork(784, 200, 10, 0.1);
            int epochs = 5;
            Console.WriteLine("Begining " + epochs + " round cycle of training:");            
            for(int i = 0; i < epochs; i++)
            {
                mh.TrainNeuroNet(nn);
            }           
            mh.LoadTestData();
            int correct = mh.TestNeuroNet(nn);
            Console.WriteLine("Correct answers: " + correct + "/10,000");
            NeuralNetwork.SaveNeuroNet(savePath, nn);
        }
    }
}