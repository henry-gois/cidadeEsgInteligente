using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Esg.Api.Data;

public sealed class EsgDbContextFactory : IDesignTimeDbContextFactory<EsgDbContext>
{
    public EsgDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EsgDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EsgEnergyDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new EsgDbContext(options);
    }
}
