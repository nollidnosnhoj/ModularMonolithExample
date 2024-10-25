using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Domain;

namespace Notes.Infrastructure;

public class NotesDbContext : DbContext
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }
    
    public DbSet<Note> Notes { get; set; }
    public DbSet<NoteData> NoteData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(note => note.Id);
            entity.Property(note => note.Id).ValueGeneratedNever();
            entity.Property(note => note.Content).IsRequired();
            entity.Property(note => note.Type).IsRequired();
            entity.Property(note => note.OwnerType).IsRequired();
            entity.Property(note => note.OwnerId).IsRequired();
            entity.HasMany(note => note.Data)
                .WithOne()
                .HasForeignKey(noteData => noteData.NoteId)
                .IsRequired();
        });
        
        modelBuilder.Entity<NoteData>(entity =>
        {
            entity.HasKey(note => note.Id);
            entity.Property(note => note.Id).ValueGeneratedNever();
            entity.Property(note => note.NoteId).IsRequired();
            entity.Property(note => note.Key).IsRequired();
            entity.Property(note => note.Value).IsRequired();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}

public static class NotesDbContextExtensions
{
    public static IServiceCollection AddNotesDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Notes")));
        
        return services;
    }
}