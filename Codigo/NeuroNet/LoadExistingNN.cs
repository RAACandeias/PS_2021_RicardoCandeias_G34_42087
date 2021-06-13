using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroNetwork
{
    class LoadExistingNN
    {
        public static void LoadExistingNNAndTest(int eachDigitQuantity)
        {          
            string savePath = @".\data\neuro net";
            string mnistDatasetPath = @".\data\dataset";                                                         
            NeuralNetwork nn = NeuralNetwork.LoadNeuroNet(784, 200, 10, 0.1, savePath);
            MNISTHandler mh = new MNISTHandler(mnistDatasetPath, null, eachDigitQuantity);
            mh.LoadTestData();
            int correct = mh.TestNeuroNet(nn);
            Console.WriteLine("Correct answers: " + correct + "/10,000");
        }
    }
}
