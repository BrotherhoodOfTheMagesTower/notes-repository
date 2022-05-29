using NotesRepository.Data;
using NotesRepository.Repositories;
using Quartz;

namespace NotesRepository.Services.QuartzJobs;

[DisallowConcurrentExecution]
public class DeleteDirectories : IJob
{
    private readonly IServiceProvider _container;

    public DeleteDirectories(IServiceProvider container)
    {
        _container = container;
    }

    public DeleteDirectories()
    {
    }

    public Task Execute(IJobExecutionContext context)
    {
        using (var ctx = _container.GetService<ApplicationDbContext>())
        {
            if (ctx is not null)
            {
                var _dr = new DirectoryRepository(ctx);
                var _nr = new NoteRepository(ctx);
                var _ur = new UserRepository(ctx);
                var _ds = new DirectoryService(_nr, _dr, _ur);


                var result = _ds.RemoveDirectoriesSubdirectoriesAndNotesFromBinAndDb();
                if (result == true)
                    Console.WriteLine($"Attempting to delete directories from db");
                else
                Console.WriteLine("There were no directories to be deleted");
                return Task.CompletedTask;
            }
            Console.WriteLine("Couldn't get an ApplicationDbContext...");
            return Task.CompletedTask;
        }
    }
}
