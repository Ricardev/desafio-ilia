using Infra.Data.ControlePonto.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infra.Data.ControlePonto;

public class ControlePontoContext : DbContext
{
    public ControlePontoContext() {}

    public ControlePontoContext(DbContextOptions<ControlePontoContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RegistroMapping());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            ConfigDatabase(optionsBuilder);
        base.OnConfiguring(optionsBuilder);
    }
    
    private void ConfigDatabase(DbContextOptionsBuilder optionsBuilder)
    { 
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, false)
            .Build();
        
        optionsBuilder.UseNpgsql(configuration.GetSection("DatabaseConnection")["ConnectionString"]!);
    }
}