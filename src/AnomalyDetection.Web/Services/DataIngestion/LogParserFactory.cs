// AnomalyDetection.Web/Services/DataIngestion/LogParserFactory.cs
namespace AnomalyDetection.Web.Services.DataIngestion;

public class LogParserFactory(IEnumerable<ILogFileParser> parsers)
{
    public ILogFileParser GetParser(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        var parser = parsers.FirstOrDefault(p => p.CanParseExtension(extension));

        return parser 
            ?? throw new ArgumentException($"No parser available for file extension: {extension}");
    }
}
