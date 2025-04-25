using AnomalyDetection.Core.Entities;

namespace AnomalyDetection.Core.Repositories;

public interface IModelRepository
{
    Task<MLModel> GetByIdAsync(int id);
    Task<IEnumerable<MLModel>> GetAllAsync();
    Task AddAsync(MLModel model);
    Task UpdateAsync(MLModel model);
    Task DeleteAsync(int id);
}
