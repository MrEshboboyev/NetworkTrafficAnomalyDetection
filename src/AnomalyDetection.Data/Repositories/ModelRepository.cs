using AnomalyDetection.Core.Entities;
using AnomalyDetection.Core.Repositories;
using AnomalyDetection.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AnomalyDetection.Data.Repositories;

public class ModelRepository(ApplicationDbContext context) : IModelRepository
{
    public async Task<MLModel> GetByIdAsync(int id)
    {
        return await context.MLModels.FindAsync(id);
    }

    public async Task<IEnumerable<MLModel>> GetAllAsync()
    {
        return await context.MLModels.ToListAsync();
    }

    public async Task AddAsync(MLModel model)
    {
        await context.MLModels.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MLModel model)
    {
        context.MLModels.Update(model);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var model = await GetByIdAsync(id);
        if (model != null)
        {
            context.MLModels.Remove(model);
            await context.SaveChangesAsync();
        }
    }
}
