// AnomalyDetection.Web/Services/DataIngestion/CsvLogParser.cs
using System.Globalization;

namespace AnomalyDetection.Web.Services.DataIngestion;

public class CsvLogParser : ILogFileParser
{
    public bool CanParseExtension(string fileExtension)
    {
        return fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<IEnumerable<NetworkTrafficLog>> ParseAsync(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Register mapping for CSV columns to NetworkTrafficLog properties
        csv.Context.RegisterClassMap<NetworkTrafficLogMap>();

        // Read all records
        var records = csv.GetRecords<NetworkTrafficLog>().ToList();
        return records;
    }
}
