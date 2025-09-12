namespace HospitalManagementSystem
{
    // Static utility class for common functionality
    public static class Utils
    {
        private static Random random = new Random();
        private static HashSet<int> usedIds = new HashSet<int>();
        private static readonly string IdFile = Path.Combine("Data", "usedIds.txt");

        // Static constructor to load used IDs
        static Utils()
        {
            LoadUsedIds();
        }

        // Generate unique ID (5-8 digits)
        public static int GenerateId()
        {
            int id;
            do
            {
                id = random.Next(10000, 99999999); // 5 to 8 digits
            } while (usedIds.Contains(id));

            usedIds.Add(id);
            SaveUsedIds();
            return id;
        }

        // Load used IDs from file
        private static void LoadUsedIds()
        {
            try
            {
                if (File.Exists(IdFile))
                {
                    var lines = File.ReadAllLines(IdFile);
                    foreach (var line in lines)
                    {
                        if (int.TryParse(line, out int id))
                        {
                            usedIds.Add(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading used IDs: {ex.Message}");
            }
        }

        // Save used IDs to file
        private static void SaveUsedIds()
        {
            try
            {
                // Ensure directory exists
                string? directory = Path.GetDirectoryName(IdFile);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var lines = new List<string>();
                foreach (var id in usedIds)
                {
                    lines.Add(id.ToString());
                }
                File.WriteAllLines(IdFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving used IDs: {ex.Message}");
            }
        }


        // Mask password input
        public static string GetMaskedPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Ignore any key that isn't a character or backspace
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        // Safe console readline that never returns null
        public static string ReadLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }
    }
}