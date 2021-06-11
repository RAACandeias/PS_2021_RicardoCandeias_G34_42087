using System;


namespace NeuroNetwork
{
    class App
    {
        public static void Run(int eachDigitQuantity)
        {
            while (true)
            {                
                Console.WriteLine("Operações:");
                Console.WriteLine("1-> Refresh test images;");
                Console.WriteLine("2-> Load existing neuralNet and test;");
                Console.WriteLine("3-> Create new neuralNet and train;");
                Console.WriteLine("4-> Simulate with test images;");
                Console.WriteLine("0-> Exit;");
                int choice = 0;
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());                    
                }
                catch(Exception)
                {
                    Console.WriteLine("Invalid input!");
                }
                Console.Clear();
                switch (choice)
                {
                    case 0:
                        return;                        
                    case 1:
                        CheckTestImages.RefreshTestImages(eachDigitQuantity);
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case 2:
                        LoadExistingNN.LoadExistingNNAndTest(eachDigitQuantity);
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case 3:
                        CreateNewNN.CreateNewNNAndTrain(eachDigitQuantity);
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case 4:
                        SimTestImages.SimulateLoop(eachDigitQuantity);                        
                        Console.Clear();
                        break;
                    default:                       
                        Console.WriteLine("Invalid input!");
                        break;
                }
            }
        }
    }
}
