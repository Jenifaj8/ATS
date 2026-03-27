using ATS.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Data;

public class AtsDbContext : IdentityDbContext<ApplicationUser>
{
    public AtsDbContext(DbContextOptions<AtsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Assessment> Assessments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Assessment>()
            .Property(a => a.DueDate)
            .HasColumnType("timestamp without time zone");
    }
}