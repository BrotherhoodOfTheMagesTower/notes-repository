using NotesRepository.Data;
using NotesRepository.Repositories;
using Quartz;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NotesRepository.Services.QuartzJobs;
public class AddEventReminder : IJob
{
    private readonly IServiceProvider _container;
    public AddEventReminder(IServiceProvider container)
    {
        _container = container;
    }

    public Task Execute(IJobExecutionContext context)
    {
        using (var ctx = _container.GetService<ApplicationDbContext>())
        {
            if (ctx is not null)
            {
                var eventRepo = new EventRepository(ctx);
                var _event = eventRepo.GetById(Guid.Parse(context.JobDetail.Key.Name));
                if (_event is not null)
                {
                    Console.WriteLine($"Attempting to send e-mail for EventId: {_event.NoteId} for time: {_event.ReminderAt}");
                    var key = _container.GetRequiredService<IConfiguration>()["sendgrid-api-key"];
                    var sendGridClient = new SendGridClient(key);
                    var sendGridMessage = new SendGridMessage();
                    sendGridMessage.SetFrom("brotherhoodofthemagestower@gmail.com", "Notes Repository");
                    sendGridMessage.AddTo(_event.User.Email);
                    sendGridMessage.SetTemplateId("d-9b9cb70e0cd74df7a395f459310ae845");
                    sendGridMessage.SetTemplateData(new
                    {
                        email = _event.User.Email,
                        url = "https://notesrepository.azurewebsites.net/calendar",
                        startAt = _event.StartAt.ToString("HH:mm dddd, dd MMMM yyyy")
                    });

                    var response = sendGridClient.SendEmailAsync(sendGridMessage);
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        Console.WriteLine($"E-mail was successfuly sent!");
                    }
                    return Task.CompletedTask;
                }
                else
                {
                    Console.WriteLine($"Couldn't get the event (ID: {Guid.Parse(context.JobDetail.Key.Name)} from the database.");
                    return Task.CompletedTask;
                }
                
            }
            Console.WriteLine("Couldn't get an ApplicationDbContext...");
            return Task.CompletedTask;
        }
    }
}
