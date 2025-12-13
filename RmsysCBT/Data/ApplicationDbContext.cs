using Microsoft.EntityFrameworkCore;
using RMSYSCBT.Models.Entities;

namespace RMSYSCBT.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
}
