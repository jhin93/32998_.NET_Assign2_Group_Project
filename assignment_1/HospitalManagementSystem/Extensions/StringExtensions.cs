namespace HospitalManagementSystem.Extensions
{
    /// <summary>
    /// Provides extension methods for string validation
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if a string represents a valid ID (5-8 digits)
        /// </summary>
        /// <param name="value">The string to validate</param>
        /// <returns>True if the string is a valid ID, false otherwise</returns>
        public static bool IsValidId(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return int.TryParse(value, out int id) && id >= 10000 && id <= 99999999;
        }
    }
}