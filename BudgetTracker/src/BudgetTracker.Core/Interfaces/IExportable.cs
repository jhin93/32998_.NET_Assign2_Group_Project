namespace BudgetTracker.Core.Interfaces;

/// <summary>
/// Interface for entities that can be exported to various formats
/// </summary>
public interface IExportable
{
    /// <summary>
    /// Exports the entity to CSV format
    /// </summary>
    /// <returns>CSV representation of the entity</returns>
    string ToCsv();

    /// <summary>
    /// Exports the entity to JSON format
    /// </summary>
    /// <returns>JSON representation of the entity</returns>
    string ToJson();

    /// <summary>
    /// Gets the CSV header row for this entity type
    /// </summary>
    /// <returns>CSV header string</returns>
    string GetCsvHeader();
}

/// <summary>
/// Interface for services that can export collections of data
/// </summary>
/// <typeparam name="T">The type of data to export</typeparam>
public interface IExportService<T> where T : class
{
    /// <summary>
    /// Exports a collection to CSV format
    /// </summary>
    /// <param name="items">Items to export</param>
    /// <returns>CSV string with header and data</returns>
    string ExportToCsv(IEnumerable<T> items);

    /// <summary>
    /// Exports a collection to JSON format
    /// </summary>
    /// <param name="items">Items to export</param>
    /// <returns>JSON array string</returns>
    string ExportToJson(IEnumerable<T> items);

    /// <summary>
    /// Exports a collection to a file
    /// </summary>
    /// <param name="items">Items to export</param>
    /// <param name="filePath">Path to save the file</param>
    /// <param name="format">Export format (csv, json, etc.)</param>
    void ExportToFile(IEnumerable<T> items, string filePath, string format);
}
