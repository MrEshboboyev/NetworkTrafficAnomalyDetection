// AnomalyDetection.Web/Services/DataIngestion/ILogFileParser.cs
using AnomalyDetection.Core.Entities;

namespace AnomalyDetection.Web.Services.DataIngestion;

public interface ILogFileParser
{
    Task<IEnumerable<NetworkTrafficLog>> ParseAsync(Stream fileStream);
    bool CanParseExtension(string fileExtension);
}
