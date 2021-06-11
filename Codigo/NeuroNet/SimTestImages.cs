using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroNetwork
{
    class SimTestImages
    {
        public static void SimulateLoop(int eachDigitQuantity)
        {            
            string formatedTestImgs = @"..\..\data\formated test images";
            string savePath = @"..\..\data\neuro net";
            NeuralNetwork nn = NeuralNetwork.LoadNeuroNet(784, 200, 10, 0.1, savePath);
            MNISTHandler mh = new MNISTHandler(null, formatedTestImgs, eachDigitQuantity);
            mh.LoadHandMadeData(eachDigitQuantity);
            Random r = new Random();
            while (true)
            {
                Console.Write("Write a digit (-1 to Exit): ");
                int digit = -1;
                try
                {
                    digit = Convert.ToInt32(Console.ReadLine());
                    if (digit == -1) return;
                    if (digit < 0 || digit > 9) throw new InvalidOperationException();
                    else
                    {
                        int index = r.Next(0, eachDigitQuantity - 1);
                        MNISTimage mi = mh.ReadHandMade(digit, index);
                        double[] res = nn.Query(mi.pixelsA);
                        int dgt = MNISTHandler.IndexOfMaxValue(res);
                        Console.WriteLine("     Number read of " + digit + "t" + index + ": " + dgt);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input!");
                }                                           
            }
        }
    }
}
