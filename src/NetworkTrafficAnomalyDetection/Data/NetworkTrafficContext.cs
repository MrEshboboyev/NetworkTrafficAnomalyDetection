using Microsoft.EntityFrameworkCore;
using NetworkTrafficAnomalyDetection.Models;

namespace NetworkTrafficAnomalyDetection.Data;

public class NetworkTrafficContext(DbContextOptions<NetworkTrafficContext> options) : DbContext(options)
{
    public DbSet<NetworkTraffic> NetworkTraffics { get; set; }
}
