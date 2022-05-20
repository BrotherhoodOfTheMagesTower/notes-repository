using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Repositories;
using NotesRepository.Services;
using NotesRepository.Services.Azure;
using NotesRepository.Services.QuartzJobs;
using Quartz;
using Radzen;
using Plk.Blazor.DragDrop;


var builder = WebApplication.CreateBuilder(args);

#if DEBUG
var connectionString = builder.Configuration.GetConnectionString("LocalApplicationDbContextConnection");
#else
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection");
#endif
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services
    .AddAuthentication()
        .AddGoogle(opts =>
        {
            opts.ClientId = "1024151888164-a4ckjd6fc4kdpecff7cp7u4lhacqgbsh.apps.googleusercontent.com";
            opts.ClientSecret = "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";
            opts.SignInScheme = IdentityConstants.ExternalScheme;
        });

builder.Services.AddScoped<DeleteSingleNotes>();
builder.Services.AddScoped<DeleteDirectories>();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Create "keys" for jobs
    var notes = new JobKey("DeleteSingleNotes");
    var directories = new JobKey("DeleteDirectories");

    // Register jobs with the DI container
    q.AddJob<DeleteSingleNotes>(opts => opts.WithIdentity(notes).StoreDurably(true));
    q.AddJob<DeleteDirectories>(opts => opts.WithIdentity(directories).StoreDurably(true));

    // Create triggers for jobs
    q.AddTrigger(opts => opts
        .ForJob(notes)
        .WithIdentity("DeleteSingleNotes")
        .WithCronSchedule("0 0 12 ? * *") // run every day at noon
        .StartNow());

    q.AddTrigger(opts => opts
        .ForJob(directories)
        .WithIdentity("DeleteDirectories")
        .WithCronSchedule("0 0 12 ? * *") // run every day at noon
        .StartNow());
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddControllers();
builder.Services.AddLocalization();
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<CollaboratorsNotesRepository>();
builder.Services.AddScoped<DirectoryRepository>();
builder.Services.AddScoped<ImageRepository>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AzureStorageHelper>();

builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<CollaboratorsNotesService>();
builder.Services.AddScoped<Flags>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

builder.Services.AddSingleton<ViewOptionService>();
builder.Services.AddScoped<DialogService>();

builder.Services.AddBlazorDragDrop();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    //app.UseMigrationsEndPoint();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var serviceProvider = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope().ServiceProvider;
serviceProvider!.GetService<ApplicationDbContext>()!.Database.Migrate();

serviceProvider!.SeedDefaultEntities();
serviceProvider!.SeedCollaboratorsWithSharedNotes();

app.Run();