using MathNet.Numerics.LinearAlgebra;
using System;

namespace NeuroNetwork
{
    class CheckTestImages
    {       
        public static void RefreshTestImages(int eachDigitQuantity)
        {            
            string handMadeImgPath = @".\data\test images";
            string formatedTestImgs = @".\data\formated test images";
            string savePath = @".\data\neuro net";           

            Matrix<double>[] mA = new Matrix<double>[eachDigitQuantity*10];
            for(int digit = 0; digit < 10; digit++)
            {
                for(int index = 0; index < eachDigitQuantity; index++)
                {
                    Matrix<double> m = MadeDigitHandler.LoadImgMatrix(handMadeImgPath, digit, index);
                    mA[index + (digit * eachDigitQuantity)] = m;                   
                }
            }
            MadeDigitHandler.SaveJointIdx(mA, formatedTestImgs, eachDigitQuantity);
            //PrintMatrix(mA);                        
            NeuralNetwork nn = NeuralNetwork.LoadNeuroNet(784, 200, 10, 0.1, savePath);
            MNISTHandler mh = new MNISTHandler(null, formatedTestImgs, eachDigitQuantity);
            mh.LoadHandMadeData(eachDigitQuantity);
            mh.ReadHandMade(nn);
        }  
        private static void PrintMatrix(Matrix<double>[] mA)
        {
            for (int num = 0; num < 30; num++)
            {
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        Console.Write("|" + mA[num].At(j, i));
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
            }
        }
    }
}
