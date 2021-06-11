using System;


namespace NeuroNetwork
{
    public class MNISTimage
    {
        public int width; // 28
        public int height; // 28        
        public double[] pixelsA;
        public byte label; // '0' - '9'
        public double[] target ;       

        public void PrintTarget()
        {
            string space = ", ";
            Console.Write("{ ");
            for (int i = 0; i < target.Length; i++)
            {
                Console.Write(target[i]);

                if (i == 9) space = " ";
                Console.Write(space);
            }
            Console.WriteLine("}");
        }

        public MNISTimage(int width, int height, double[] pixelsA, byte label)
        {
            this.width = width;
            this.height = height;            
            this.label = label;
            target =  new double[] { 0.01d, 0.01d, 0.01d, 0.01d, 0.01d, 0.01d, 0.01d, 0.01d, 0.01d, 0.01d };
            target[Convert.ToInt32(label)] = 0.99d;
            this.pixelsA = new double[width*height];
            pixelsA.CopyTo(this.pixelsA, 0);
            
        }      
    }
}
