using NotesRepository.Data;
using NotesRepository.Repositories;
using Quartz;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NotesRepository.Services.QuartzJobs;

public class CancelEventReminder : IJob
{
    private readonly IServiceProvider _container;

    public CancelEventReminder(IServiceProvider container)
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
                var eventId = context.JobDetail.Key.Name.Split('_')[1];
                var _event = eventRepo.GetById(Guid.Parse(eventId));
                if (_event is not null)
                {
                    Console.WriteLine($"Attempting to send cancellation e-mail for EventId: {_event.NoteId} at {DateTime.Now}");
                    var key = _container.GetRequiredService<IConfiguration>()["sendgrid-api-key"];
                    var sendGridClient = new SendGridClient(key);
                    var sendGridMessage = new SendGridMessage();
                    sendGridMessage.SetFrom("brotherhoodofthemagestower@gmail.com", "Notes Repository");
                    sendGridMessage.AddTo(_event.User.Email);
                    sendGridMessage.SetTemplateId("d-cc851c9dc1e04c4390ce804cbdcf1dcc");
                    sendGridMessage.SetTemplateData(new
                    {
                        eventTitle = _event.Content,
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
