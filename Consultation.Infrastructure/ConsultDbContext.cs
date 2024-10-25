using Consultation.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consultation.Infrastructure;

public class ConsultDbContext : DbContext
{
    public ConsultDbContext(DbContextOptions<ConsultDbContext> options) : base(options)
    {
    }
    
    public DbSet<Consult> Consults { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consult>(entity =>
        {
            entity.HasKey(consult => consult.Id);
            entity.Property(consult => consult.Id).ValueGeneratedNever();
            entity.Property(consult => consult.PatientName).IsRequired();
            entity.Property(consult => consult.PatientBirthDate).IsRequired();
            entity.Property(consult => consult.PatientSalutation).IsRequired();
            entity.Property(consult => consult.Status).IsRequired();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}

public static class ConsultDbContextExtensions
{
    public static IServiceCollection AddConsultDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ConsultDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Consults")));
        
        return services;
    }
}