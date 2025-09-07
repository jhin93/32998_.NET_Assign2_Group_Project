namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int ch = Console.Read();
            Console.WriteLine("unicode values of input: " + ch);

            string line = Console.ReadLine();
            Console.WriteLine("input values: " + line);

            Console.WriteLine("press any key.");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            Console.WriteLine("\n pressed key: " + keyInfo.Key);

            Console.Write("print without changing the line.");

            Console.WriteLine("print with changing the line.");
            
            if (int.TryParse(line, out int number))
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
