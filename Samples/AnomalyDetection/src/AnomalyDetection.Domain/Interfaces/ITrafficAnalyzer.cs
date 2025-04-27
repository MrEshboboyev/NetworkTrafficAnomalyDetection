using AnomalyDetection.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetection.Domain.Interfaces;

public interface ITrafficAnalyzer
{
    Task<IEnumerable<NetworkTrafficLog>> AnalyzeTrafficAsync(IEnumerable<NetworkTrafficLog> logs);
    Task<double> CalculateAnomalyScoreAsync(NetworkTrafficLog log);
    Task<bool> IsAnomalousAsync(NetworkTrafficLog log, double threshold = 0.8);
}