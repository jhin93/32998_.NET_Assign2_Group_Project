namespace HospitalManagementSystem.Extensions
{
    // Extension methods for string manipulation
    public static class StringExtensions
    {
        // Extension method to truncate string
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        // Extension method to check if string is valid ID
        public static bool IsValidId(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return int.TryParse(value, out int id) && id >= 10000 && id <= 99999999;
        }

        // Extension method to capitalize first letter
        public static string CapitalizeFirst(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }

        // Extension method to format as title
        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var words = value.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = words[i].CapitalizeFirst();
                }
            }
            return string.Join(" ", words);
        }
    }
}