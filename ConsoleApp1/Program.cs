namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the numbers:");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out int number))
            {
                int test = SumAverageLargestSmallest(number);
                Console.WriteLine(test);
            }
            else
            {
                Console.WriteLine("Enter only numbers.");
            }
        }

        static int SumAverageLargestSmallest(int input)
        {
            Console.WriteLine("This is a test method.");
            return input;
        }
    }
}
