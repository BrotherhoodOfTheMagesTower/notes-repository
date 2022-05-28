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
                var dirRepo = new DirectoryRepository(ctx);
                var toBeDeleted = dirRepo.GetMainDirectoriesWhichShouldBeRemovedFromDb();
                if (toBeDeleted.Count > 0)
                {
                    Console.WriteLine($"Attempting to delete {toBeDeleted.Count} directories from db");
                    var result = dirRepo.DeleteMany(toBeDeleted);
                    if (result)
                    {
                        Console.WriteLine($"Successfully removed {toBeDeleted.Count} directories from db");
                        return Task.CompletedTask;
                    }
                    else
                    {
                        Console.WriteLine($"Couldn't remove {toBeDeleted.Count} directories from db");
                        return Task.CompletedTask;
                    }
                }
                Console.WriteLine("There were no directories to be deleted");
                return Task.CompletedTask;
            }
            Console.WriteLine("Couldn't get an ApplicationDbContext...");
            return Task.CompletedTask;
        }
    }
}
