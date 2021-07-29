using MathNet.Numerics.LinearAlgebra;
using System;
using System.IO;
using System.Linq;

namespace SysPlan
{
    public class MNISTHandler
    {
        private string pixelFileTrain = @"\train-images.idx3-ubyte";
        private string labelFileTrain = @"\train-labels.idx1-ubyte";
        private string pixelFileTest = @"\t10k-images.idx3-ubyte";
        private string labelFileTest = @"\t10k-labels.idx1-ubyte";
        private string pixelFileByHand = @"\formatedTestImgs.idx3-ubyte";
        private string labelFileByHand = @"\formatedTestImgsLabels.idx1-ubyte";
        private string datasetPath;
        private string handMadePath;
        private int eachDigitQuantity;
        public MNISTimage[] trainImages = null;
        public MNISTimage[] testImages = null;
        public MNISTimage[] handMadeImgs = null;

        public MNISTHandler(string datasetPath, string handMadePath, int eachDigitQuantity)
        {
            this.datasetPath = datasetPath;
            this.handMadePath = handMadePath;
            this.eachDigitQuantity = eachDigitQuantity;
        }        
        public MNISTimage ReadHandMade(int digit, int index)
        {
            return handMadeImgs[index + (digit * eachDigitQuantity)];
        }
        public void LoadTrainData()
        {
            Console.Write("Loading Train data set... ");
            trainImages = Load(60000, pixelFileTrain, labelFileTrain, datasetPath);
            Console.WriteLine("Done!");
        }        
        public void LoadTestData()
        {
            Console.Write("Loading Test data set... ");
            testImages = Load(10000, pixelFileTest, labelFileTest, datasetPath);
            Console.WriteLine("Done!");
        }
        public void LoadHandMadeData(int eachDigitQuantity)
        {
            Console.Write("Loading Hand made data set... ");
            handMadeImgs = Load(eachDigitQuantity*10, pixelFileByHand, labelFileByHand, handMadePath);
            Console.WriteLine("Done!");
        }
        public void TrainNeuroNet(NeuralNetwork nn)
        {
            Console.Write("Training neural network... ");
            foreach (MNISTimage img in trainImages)
            {
                nn.Train(img.pixelsA, img.target);
            }
            Console.WriteLine("Done!");
        }
        private MNISTimage[] Load(int numImages, string pixelFile, string labelFile, string dataPath)
        {            
            MNISTimage[] result = new MNISTimage[numImages];
            double[] pixelsA = new double[784];

            FileStream ifsPixels = new FileStream(dataPath + pixelFile, FileMode.Open);
            FileStream ifsLabels = new FileStream(dataPath + labelFile, FileMode.Open);
            BinaryReader brImages = new BinaryReader(ifsPixels);
            BinaryReader brLabels = new BinaryReader(ifsLabels);

            int magic1 = ReverseBytes(brImages.ReadInt32()); //stored as big endian need to convert to Intel format
            if (magic1 != 2051) throw new Exception("Training data is invalid! Magic number 1 does not match.");

            int imageCount = ReverseBytes(brImages.ReadInt32());
            if (imageCount != numImages) throw new Exception("Training data is invalid! Number of images does not match.");

            int numRows = ReverseBytes(brImages.ReadInt32());
            if (numRows != 28) throw new Exception("Training data is invalid! Number of rows does not match.");

            int numCols = ReverseBytes(brImages.ReadInt32());
            if (numCols != 28) throw new Exception("Training data is invalid! Number of colums does not match.");

            int magic2 = ReverseBytes(brLabels.ReadInt32());
            if (magic2 != 2049) throw new Exception("Label data is invalid! Magic number does not match.");

            int numLabels = ReverseBytes(brLabels.ReadInt32());
            if (numLabels != numImages) throw new Exception("Label data is invalid! Number of labels does not match.");

            for (int di = 0; di < numImages; ++di) //to store each image
            {
                for (int i = 0; i < 784; ++i)
                {
                    byte b = brImages.ReadByte();
                    double aux = Scale(0.01, 1.0, 255.0, Convert.ToDouble(b));
                    pixelsA[i] = aux;
                }
                byte lbl = brLabels.ReadByte(); // get the label
                MNISTimage dImage = new MNISTimage(28, 28, pixelsA, lbl);
                result[di] = dImage;
            }
            ifsPixels.Close(); brImages.Close();
            ifsLabels.Close(); brLabels.Close();           
            return result;
        }                
        public static void PrintArray(double[] array)
        {
            string space = ", ";
            Console.Write("{ ");
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i]);

                if (i == 9) space = " ";
                Console.Write(space);
            }
            Console.WriteLine("}");
        }
        public int TestNeuroNet(NeuralNetwork nn)
        {
            Console.Write("Testing NeuroNet... ");
            int correct = 0;
            foreach(MNISTimage img in testImages)
            {
                double[] response = nn.Query(img.pixelsA);
                int r = Convert.ToInt32(img.label);
                int g = IndexOfMaxValue(response);                
                if (r == g) correct++;
            }
            Console.WriteLine("Done!");
            return correct;
        }
        public int ReadHandMade(NeuralNetwork nn)
        {
            Console.Write("Reading Hand made set... ");
            int correct = 0;
            Console.WriteLine();
            foreach (MNISTimage img in handMadeImgs)
            {
                double[] response = nn.Query(img.pixelsA);
                int r = Convert.ToInt32(img.label);
                int g = IndexOfMaxValue(response);
                //PrintArray(response);
                Console.Write("label: "+r);
                Console.WriteLine(", guess: "+g);                
                if (r == g) correct++;
            }
            Console.WriteLine("Done!");
            return correct;
        }
        public static int ReverseBytes(int v)
        {
            byte[] intAsBytes = BitConverter.GetBytes(v);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }
        //To scale intput to 0.01-1.0 since the pixel values are 0-255
        public static double Scale(double lower, double higher, double maxVal, double val)
        {
            return ((val / maxVal) * (higher - lower)) + lower;
        }
        public static int IndexOfMaxValue(double[] array)
        {
            int indexOfMaxVal = 0;            
            for(int i = 1; i < array.Length; i++)
            {
                if (array[i] > array[indexOfMaxVal]) 
                { 
                    indexOfMaxVal = i;                     
                }
            }
            return indexOfMaxVal;
        }
    }   
}
