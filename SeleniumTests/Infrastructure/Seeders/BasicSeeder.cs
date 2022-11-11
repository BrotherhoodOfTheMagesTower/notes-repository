using Microsoft.Extensions.Configuration;
using NotesRepository.Data;
using NotesRepository.Repositories;
using NotesRepository.Services;
using NotesRepository.Services.Azure;
using SeleniumTests.Extensions;

namespace SeleniumTests.Infrastructure.Seeders;

public static class BasicSeeder
{
    public static async Task<BasicSeedingReport> CreateEnvironment(BasicSeedingTask task)
    {
        if (task.ImagesPerAccountCount > 0 && task.NotesPerAccountCount == 0)
            throw new ArgumentException($"In order to create images {nameof(task.NotesPerAccountCount)} needs to be higher than 0.");
        
        if (task.AccountsCount % 2 != 0)
            throw new ArgumentException($"In order to create accounts {nameof(task.AccountsCount)} needs to be an even number");

        using var context = new ApplicationDbContext(DbOptionsFactory.DbContextOptions);

        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        (var ns, var ds, var es, var ims, var cns, var ur) = CreateServices(context, config);

        var users = await ur.CreateUsers(task.AccountsCount);
        var directories = await ds.CreateDirectories(task.DirectoriesPerAccountCount, users, ur);
        var notes = await ns.CreateNotesForDefaultDirectory(task.NotesPerAccountCount, users, ur, ds);
        var events = await es.CreateEvents(task.EventsPerAccountCount, users, ur);
        var images = await ims.CreateImages(task.ImagesPerAccountCount, users, ns, config);
        var collaborators = new List<Tuple<string, string>>();
        if (task.CreateCollaborators)
            collaborators = (await cns.CreateCollaborators(task.AccountsCount, users, ns, ur, ds)).ToList();


        return new BasicSeedingReport(users, directories, notes, events, images, collaborators);
    }

    public static async Task CleanEnvironment(BasicSeedingReport report)
    {
        var userIds = report.Users.Select(x => x.Id);
        using var context = new ApplicationDbContext(DbOptionsFactory.DbContextOptions);
        var ur = new UserRepository(context);
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var storageBaseUrl = config.GetSection("ConnectionStrings")["StorageBaseUrl"];
        var storageConnectionString = config.GetSection("ConnectionStrings")["StorageConnectionString"];
        var ns = new NoteService(new NoteRepository(context), ur, new EventRepository(context), new DirectoryRepository(context),
            new ImageRepository(context), new AzureStorageHelper(storageBaseUrl, storageConnectionString));

        foreach (var userId in userIds)
        {
            var notes = await ns.GetAllUserNotesByIdAsync(userId.ToString());
            foreach (var note in notes)
                await ns.DeleteNoteByIdAsync(note.NoteId);

            await ur.DeleteUserByIdAsync(userId.ToString());
        }
    }
    
    public static async Task RemoveUser(string userEmail)
    {
        using var context = new ApplicationDbContext(DbOptionsFactory.DbContextOptions);
        var ur = new UserRepository(context);
        var user = await ur.GetUserByEmailAsync(userEmail);
        if (user != null)
            await ur.DeleteUserByIdAsync(user.Id.ToString());
    }


    private static (NoteService, DirectoryService, EventService, ImageService, CollaboratorsNotesService, UserRepository) CreateServices(
        ApplicationDbContext context, IConfigurationRoot config)
    {
        var nr = new NoteRepository(context);
        var dr = new DirectoryRepository(context);
        var er = new EventRepository(context);
        var ir = new ImageRepository(context);
        var ur = new UserRepository(context);
        var cnr = new CollaboratorsNotesRepository(context);
        var ash = new AzureStorageHelper(config.GetSection("ConnectionStrings")["StorageBaseUrl"],
            config.GetSection("ConnectionStrings")["StorageConnectionString"]);

        var ns = new NoteService(nr, ur, er, dr, ir, ash);
        var ds = new DirectoryService(nr, dr, ur, ir, ash);
        var es = new EventService(er);
        var ims = new ImageService(ir, ash);
        var cns = new CollaboratorsNotesService(cnr, nr, ur);

        return (ns, ds, es, ims, cns, ur);

    }
}
