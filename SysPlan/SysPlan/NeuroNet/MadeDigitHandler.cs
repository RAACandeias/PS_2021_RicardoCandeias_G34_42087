using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPlan
{
    class MadeDigitHandler
    {
        public static double ConvertBrightnessToIdxVal(Bitmap img, int width, int height)
        {
            double pixelVal = (Convert.ToDouble(img.GetPixel(width, height).GetBrightness()) - 1);
            if (pixelVal != 0.0) pixelVal *= -1;
            return pixelVal*255.0;
        }

        public static Matrix<double> LoadImgMatrix(string imgDirectory, int label, int index)
        {
            Bitmap img = new Bitmap(imgDirectory + @"\" + label + ".t" + index + ".png");
            Matrix<double> img_idx = Matrix<double>.Build.Dense(img.Height, img.Width);
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    img_idx[j, i] = ConvertBrightnessToIdxVal(img, j, i);
                }
            }
            return img_idx;
        }

        public static void SaveJointIdx(Matrix<double>[] mA, string systemImgPath, int eachDigitQuantity)
        {
            FileStream imgFile = new FileStream(systemImgPath + @"\formatedTestImgs.idx3-ubyte", FileMode.Create);
            FileStream imgLabelFile = new FileStream(systemImgPath + @"\formatedTestImgsLabels.idx1-ubyte", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(imgFile);
            BinaryWriter bwL = new BinaryWriter(imgLabelFile);
            bw.Write(MNISTHandler.ReverseBytes(2051));
            bw.Write(MNISTHandler.ReverseBytes(eachDigitQuantity*10));
            bw.Write(MNISTHandler.ReverseBytes(28));
            bw.Write(MNISTHandler.ReverseBytes(28));
            bwL.Write(MNISTHandler.ReverseBytes(2049));
            bwL.Write(MNISTHandler.ReverseBytes(eachDigitQuantity*10));
            foreach (Matrix<double> m in mA)
            {
                for (int i = 0; i < m.ColumnCount; i++)
                {
                    for (int j = 0; j < m.RowCount; j++)
                    {
                        bw.Write(Convert.ToByte(m.At(j, i)));
                    }
                }
            }
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < eachDigitQuantity; y++)
                {
                    int index = y + (x * eachDigitQuantity);                                      
                    bwL.Write(Convert.ToByte(x));                   
                }
            }
            bw.Close();
            imgFile.Close();
            bwL.Close();
            imgLabelFile.Close();
        }

        public static double[] LoadImgPixelArray(string imgDirectory, int label, int index)
        {
            Bitmap img = new Bitmap(imgDirectory + @"\" + label + ".t" + index + ".png");
            double[] imgA = new double[img.Height*img.Width];
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {                    
                    imgA[j + (i * img.Height)] = ConvertBrightnessToIdxVal(img, i, j);                   
                }
            }
            return imgA;
        }

        public static void SaveImgIdx(Matrix<double> img, string imgDirectory, int label, int index)
        {
            FileStream imgFile = new FileStream(imgDirectory + @"\"+label+".t"+index+".idx2-ubyte", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(imgFile);
            bw.Write(MNISTHandler.ReverseBytes(2049));
            bw.Write(MNISTHandler.ReverseBytes(img.RowCount));
            bw.Write(MNISTHandler.ReverseBytes(img.ColumnCount));
            for (int i = 0; i < img.ColumnCount; i++)
            {
                for (int j = 0; j < img.RowCount; j++)
                {
                    bw.Write(img[j, i]);
                }
            }
            bw.Close();
            imgFile.Close();
        }
    }
}
