using MemberDataIntegration.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberDataIntegration.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Member> Members { get; set; }
}
