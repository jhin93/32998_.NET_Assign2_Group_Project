namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // .Read() 예시: 한 글자를 입력받아 유니코드 값 출력
            int ch = Console.Read();
            Console.WriteLine("입력한 문자의 유니코드 값: " + ch);

            // .ReadLine() 예시: 한 줄을 입력받아 그대로 출력
            string line = Console.ReadLine();
            Console.WriteLine("입력한 문자열: " + line);

            // .ReadKey() 예시: 아무 키나 누르면 해당 키 정보 출력
            Console.WriteLine("아무 키나 누르세요.");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            Console.WriteLine("\n누른 키: " + keyInfo.Key);

            // .Write() 예시: 줄바꿈 없이 출력
            Console.Write("줄바꿈 없이 출력합니다.");

            // .WriteLine() 예시: 줄바꿈과 함께 출력
            Console.WriteLine("줄바꿈과 함께 출력합니다.");
            
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
