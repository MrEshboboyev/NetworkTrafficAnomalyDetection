using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AnomalyDetection.Infrastructure.Data.Repositories;

public class TrafficLogRepository(AppDbContext context) : IRepository<NetworkTrafficLog>
{
    public async Task<NetworkTrafficLog> GetByIdAsync(int id)
    {
        return await context.NetworkTrafficLogs.FindAsync(id);
    }

    public async Task<IEnumerable<NetworkTrafficLog>> GetAllAsync()
    {
        return await context.NetworkTrafficLogs.ToListAsync();
    }

    public async Task<IEnumerable<NetworkTrafficLog>> FindAsync(Expression<Func<NetworkTrafficLog, bool>> predicate)
    {
        return await context.NetworkTrafficLogs.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(NetworkTrafficLog entity)
    {
        await context.NetworkTrafficLogs.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<NetworkTrafficLog> entities)
    {
        await context.NetworkTrafficLogs.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NetworkTrafficLog entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(NetworkTrafficLog entity)
    {
        context.NetworkTrafficLogs.Remove(entity);
        await context.SaveChangesAsync();
    }
}
