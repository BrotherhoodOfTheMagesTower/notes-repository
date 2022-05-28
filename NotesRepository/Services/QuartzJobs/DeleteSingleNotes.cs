using NotesRepository.Data;
using NotesRepository.Repositories;
using Quartz;

namespace NotesRepository.Services.QuartzJobs;

[DisallowConcurrentExecution]
public class DeleteSingleNotes : IJob
{
    private readonly IServiceProvider _container;

    public DeleteSingleNotes(IServiceProvider container)
    {
        _container = container;
    }
    
    public DeleteSingleNotes()
    {
    }

    public Task Execute(IJobExecutionContext context)
    {
        using (var ctx = _container.GetService<ApplicationDbContext>())
        {
            if(ctx is not null)
            {
                var noteRepo = new NoteRepository(ctx);
                var toBeDeleted = noteRepo.GetAllSingleNotesWhichShouldBeRemovedFromDb();
                if(toBeDeleted.Count > 0)
                {
                    Console.WriteLine($"Attempting to delete {toBeDeleted.Count} single notes from db");
                    var result = noteRepo.DeleteMany(toBeDeleted);
                    if(result)
                    {
                        Console.WriteLine($"Successfully removed {toBeDeleted.Count} single notes from db");
                        return Task.CompletedTask;
                    }
                    else
                    {
                        Console.WriteLine($"Couldn't remove {toBeDeleted.Count} single notes from db");
                        return Task.CompletedTask;
                    }
                }
                Console.WriteLine("There were no individual notes to be deleted");
                return Task.CompletedTask;
            }
            Console.WriteLine("Couldn't get an ApplicationDbContext...");
            return Task.CompletedTask;
        }
    }
}
