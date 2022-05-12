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
using Radzen;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection");
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

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

builder.Services.AddSingleton<ViewOptionService>();
builder.Services.AddScoped<DialogService>();

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

app.Run();