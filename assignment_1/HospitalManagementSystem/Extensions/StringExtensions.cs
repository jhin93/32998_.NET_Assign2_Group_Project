namespace HospitalManagementSystem.Extensions
{
    // Extension methods for string validation
    public static class StringExtensions
    {
        // Extension method to check if string is valid ID (5-8 digits)
        public static bool IsValidId(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return int.TryParse(value, out int id) && id >= 10000 && id <= 99999999;
        }
    }
}