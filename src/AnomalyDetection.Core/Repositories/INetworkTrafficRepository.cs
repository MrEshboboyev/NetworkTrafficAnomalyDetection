using AnomalyDetection.Core.Entities;

namespace AnomalyDetection.Core.Repositories;

public interface INetworkTrafficRepository
{
    Task<IEnumerable<NetworkTrafficLog>> GetTrainingDataAsync();
    Task<IEnumerable<NetworkTrafficLog>> GetTestDataAsync();
}
