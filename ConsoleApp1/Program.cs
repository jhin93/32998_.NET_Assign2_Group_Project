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
                Calculator calc = new Calculator();
                int result = calc.Add(3, 5);
                number = result;
                Console.WriteLine(number);
            }
            else
            {
                Console.WriteLine("Enter only numbers.");
            }
        }
    }
}
