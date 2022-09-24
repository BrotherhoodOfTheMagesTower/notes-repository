using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotesRepository.Data;

public static class DbOptionsFactory
{
    static DbOptionsFactory()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = config.GetSection("ConnectionStrings")["LocalConnectionString"];

        DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer(connectionString)
            .Options;
    }

    public static DbContextOptions<ApplicationDbContext> DbContextOptions { get; }

}