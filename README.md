# 32998_.NET_Application_Developments

### Class: car architecture blueprint
### Object: Mercedes, BMW, Audi

```C#
using System;

namespace StudentDemo
{
    // Class: Defines a blueprint for a Student
    public class Student
    {
        // Fields: Store data for the student
        public string Name;
        public int RollNumber;
        public double Grade;

        // Constructor: Initialize the student object
        public Student(string name, int rollNumber, double grade)
        {
            Name = name;
            RollNumber = rollNumber;
            Grade = grade;
        }

        // Method: Display student information
        public void DisplayInfo()
        {
            Console.WriteLine($"Student Name: {Name}, Roll Number: {RollNumber}, Grade: {Grade}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Object: Create an instance of the Student class
            Student student1 = new Student("Nabin Sharma", 1091, 95.5);

            // Object: Create another instance of the Student class
            Student student2 = new Student("John Doe", 1092, 88.0);

            // Call method to display information for each object
            Console.WriteLine("Student 1 Information:");
            student1.DisplayInfo();

            Console.WriteLine("\nStudent 2 Information:");
            student2.DisplayInfo();

            Console.ReadKey();
        }
    }
}
```

multi-dimensional array
```C#
using System;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[,] array = new double[2, 2]; // Declare a 2x2 multidimensional array

            // Explicitly assign values to match the JavaScript structure
            array[0, 0] = 10.0; // First row, first column
            array[0, 1] = 11.2; // First row, second column
            array[1, 0] = 38.1; // Second row, first column
            array[1, 1] = 0.0;  // Second row, second column

            // Display the array contents
            Console.WriteLine("Array contents:");
            Console.WriteLine($"[{array[0, 0]}, {array[0, 1]}]");
            Console.WriteLine($"[{array[1, 0]}, {array[1, 1]}]");
            Console.ReadKey();
        }
    }
}

// or

double[,] array = new double[,] { { 10.0, 11.2 }, { 38.1, 0.0 } };
```

pass parameter by reference
메소드로 원래 존재했던 변수를 변경하는 것. ref(참조)를 통해 원본 변수를 변경하는 것. 원본 변수에 초기값 필요
```C#
using System;

namespace ReferenceParameterDemo
{
    internal class Program
    {
        // Method: Swap the values of two variables by reference
        public static void SwapNumbers(ref int a, ref int b)
        {
            int temp = a; // Store the value of a in a temporary variable
            a = b;        // Assign b's value to a
            b = temp;     // Assign the temporary value to b
        }

        static void Main(string[] args)
        {
            int number1 = 10; // Initial value set (ref requires an initial value)
            int number2 = 20;

            Console.WriteLine("Before Swapping: number1 = {0}, number2 = {1}", number1, number2);
            SwapNumbers(ref number1, ref number2); // Pass by reference
            Console.WriteLine("After Swapping: number1 = {0}, number2 = {1}", number1, number2);

            Console.ReadKey();
        }
    }
}
```

pass parameter by output
out 키워드는 빈 변수에 값을 전달 (초기값 불필요, 메서드 내에서 반드시 설정).
```C#
using System;

namespace OutParameterDemo
{
    internal class Program
    {
        // Method: Accept user input and store it in output parameters
        public static void GetUserInput(out int number, out string name)
        {
            Console.Write("Enter a number: ");
            number = Convert.ToInt32(Console.ReadLine()); // Assign value to number (no initial value needed)

            Console.Write("Enter a name: ");
            name = Console.ReadLine(); // Assign value to name
        }

        static void Main(string[] args)
        {
            int userNumber; // No initial value
            string userName;

            GetUserInput(out userNumber, out userName); // Pass by output

            Console.WriteLine("Received values: number = {0}, name = {1}", userNumber, userName);

            Console.ReadKey();
        }
    }
}
```
