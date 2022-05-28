using NotesRepository.Data;
using NotesRepository.Repositories;
using Quartz;

namespace NotesRepository.Services.QuartzJobs;

public class AddEventReminder : IJob
{
    private readonly IServiceProvider _container;

    public AddEventReminder(IServiceProvider container)
    {
        _container = container;
    }

    public AddEventReminder()
    {
    }

    public Task Execute(IJobExecutionContext context)
    {
        using (var ctx = _container.GetService<ApplicationDbContext>())
        {
            if (ctx is not null)
            {
                var eventRepo = new EventRepository(ctx);
                var _event = eventRepo.GetByIdAsync(Guid.Parse(context.JobDetail.Key.Name));
                if (_event is not null)
                {
                    Console.WriteLine($"Attempting to send e-mail for EventId: {Guid.Parse(context.JobDetail.Key.Name)}.");
                    //TODO: Implement SendGrid
                }
                Console.WriteLine($"Couldn't get the event (ID: {Guid.Parse(context.JobDetail.Key.Name)} from the database.");
                return Task.CompletedTask;
            }
            Console.WriteLine("Couldn't get an ApplicationDbContext...");
            return Task.CompletedTask;
        }
    }
}
