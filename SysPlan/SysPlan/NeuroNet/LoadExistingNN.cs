using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPlan
{
    class LoadExistingNN
    {
        public static void LoadExistingNNAndTest(int eachDigitQuantity)
        {          
            string savePath = @".\NeuroNetData\neuro net";
            string mnistDatasetPath = @".\NeuroNetData\dataset";                                                         
            NeuralNetwork nn = NeuralNetwork.LoadNeuroNet(784, 200, 10, 0.1, savePath);
            MNISTHandler mh = new MNISTHandler(mnistDatasetPath, null, eachDigitQuantity);
            mh.LoadTestData();
            int correct = mh.TestNeuroNet(nn);
            Console.WriteLine("Correct answers: " + correct + "/10,000");
        }
    }
}
