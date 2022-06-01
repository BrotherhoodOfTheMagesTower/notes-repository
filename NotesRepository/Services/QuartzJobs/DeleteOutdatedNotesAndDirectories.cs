using NotesRepository.Data;
using NotesRepository.Repositories;
using Quartz;

namespace NotesRepository.Services.QuartzJobs;

/// <summary>
/// This class is resposible for deleteing obsolete/outdated entities from the database. By default all notes/directories/subdirectories
/// that are older than 30 days will be removed from the database. 
/// </summary>
[DisallowConcurrentExecution]
public class DeleteOutdatedNotesAndDirectories : IJob
{
    private readonly IServiceProvider _container;

    public DeleteOutdatedNotesAndDirectories(IServiceProvider container)
    {
        _container = container;
    }

    public DeleteOutdatedNotesAndDirectories()
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

                var result = _ds.RemoveDirectoriesSubdirectoriesAndNotesFromBinAndDbByDate();
                if (result == true)
                    Console.WriteLine($"Successfully removed some directories/notes from bin");
                else
                Console.WriteLine($"There were no directories/notes to be deleted");
                return Task.CompletedTask;
            }
            Console.WriteLine("Couldn't get an ApplicationDbContext...");
            return Task.CompletedTask;
        }
    }
}
