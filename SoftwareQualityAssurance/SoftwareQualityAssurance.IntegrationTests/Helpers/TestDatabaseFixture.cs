using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SoftwareQualityAssurance.Data;

namespace SoftwareQualityAssurance.IntegrationTests.Helpers;

public class TestDatabaseFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public TestDatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=MELIH\\SQLEXPRESS;Database=RealEstateManagementDb_Test;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

