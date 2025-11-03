using Dal.DbModels;

using Microsoft.EntityFrameworkCore;

namespace Dal.Ef;

public class DefaultDbContext(DbContextOptions<DefaultDbContext> options) : DbContext(options)
{
    public virtual DbSet<Settings> Settings { get; set; }
    public virtual DbSet<User> DomainUsers { get; set; }
}