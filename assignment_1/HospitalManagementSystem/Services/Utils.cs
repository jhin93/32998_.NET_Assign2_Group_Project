namespace HospitalManagementSystem
{
    /// <summary>
    /// Provides utility methods for common functionality
    /// </summary>
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

        /// <summary>
        /// Generates a unique 5-digit ID
        /// </summary>
        /// <returns>A unique integer ID</returns>
        public static int GenerateId()
        {
            int id;
            do
            {
                id = random.Next(10000, 99999); // 5 digits to match existing format
            } while (usedIds.Contains(id));

            usedIds.Add(id);
            SaveUsedIds();
            return id;
        }

        // Load used IDs from file and existing data files
        private static void LoadUsedIds()
        {
            try
            {
                // If usedIds.txt exists, load from it
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
                else
                {
                    // Only load from data files if usedIds.txt doesn't exist
                    LoadExistingIds();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading used IDs: {ex.Message}");
            }
        }
        
        // Load existing IDs from all data files
        private static void LoadExistingIds()
        {
            try
            {
                // Load patient IDs
                string patientsFile = Path.Combine("Data", "patients.txt");
                if (File.Exists(patientsFile))
                {
                    var lines = File.ReadAllLines(patientsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                            {
                                usedIds.Add(id);
                            }
                        }
                    }
                }
                
                // Load doctor IDs
                string doctorsFile = Path.Combine("Data", "doctors.txt");
                if (File.Exists(doctorsFile))
                {
                    var lines = File.ReadAllLines(doctorsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                            {
                                usedIds.Add(id);
                            }
                        }
                    }
                }
                
                // Load admin IDs
                string adminsFile = Path.Combine("Data", "admins.txt");
                if (File.Exists(adminsFile))
                {
                    var lines = File.ReadAllLines(adminsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                            {
                                usedIds.Add(id);
                            }
                        }
                    }
                }
                
                // Load appointment IDs
                string appointmentsFile = Path.Combine("Data", "appointments.txt");
                if (File.Exists(appointmentsFile))
                {
                    var lines = File.ReadAllLines(appointmentsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                            {
                                usedIds.Add(id);
                            }
                        }
                    }
                }
                
                // Only save if usedIds.txt doesn't exist yet
                if (!File.Exists(IdFile) && usedIds.Count > 0)
                {
                    SaveUsedIds();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading existing IDs: {ex.Message}");
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


        /// <summary>
        /// Gets password input with masking for security
        /// </summary>
        /// <returns>The entered password</returns>
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

        /// <summary>
        /// Reads a line from console input, never returns null
        /// </summary>
        /// <returns>The input string or empty string if null</returns>
        public static string ReadLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }
    }
}