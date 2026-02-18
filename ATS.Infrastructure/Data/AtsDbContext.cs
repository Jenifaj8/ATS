using ATS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Data;

public class AtsDbContext : DbContext
{
    public AtsDbContext(DbContextOptions<AtsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Assessment> Assessments { get; set; }
}