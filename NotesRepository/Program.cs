using NotesRepository.Data;
using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using Microsoft.AspNetCore.Components.Authorization;
using NotesRepository.Areas.Identity;
using Blazored.Toast;
using NotesRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast;
using NotesRepository.Repositories;
using NotesRepository.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<CollaboratorsNotesRepository>();
builder.Services.AddScoped<DirectoryRepository>();
builder.Services.AddScoped<ImageRepository>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<NoteService>();
builder.Services.AddBlazoredToast();

builder.Services.AddSingleton<ViewOptionService>();


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

app.Run();
