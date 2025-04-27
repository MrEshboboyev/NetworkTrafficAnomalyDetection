using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AnomalyDetection.Infrastructure.Data.Repositories;

public class TrafficLogRepository : IRepository<NetworkTrafficLog>
{
    private readonly AppDbContext _context;

    public TrafficLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NetworkTrafficLog> GetByIdAsync(int id)
    {
        return await _context.NetworkTrafficLogs.FindAsync(id);
    }

    public async Task<IEnumerable<NetworkTrafficLog>> GetAllAsync()
    {
        return await _context.NetworkTrafficLogs.ToListAsync();
    }

    public async Task<IEnumerable<NetworkTrafficLog>> FindAsync(Expression<Func<NetworkTrafficLog, bool>> predicate)
    {
        return await _context.NetworkTrafficLogs.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(NetworkTrafficLog entity)
    {
        await _context.NetworkTrafficLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<NetworkTrafficLog> entities)
    {
        await _context.NetworkTrafficLogs.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NetworkTrafficLog entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(NetworkTrafficLog entity)
    {
        _context.NetworkTrafficLogs.Remove(entity);
        await _context.SaveChangesAsync();
    }
}