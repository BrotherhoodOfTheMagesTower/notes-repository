using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;

namespace NotesRepository.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    DbSet<ApplicationUser> Users;
    DbSet<Note> Notes;
    DbSet<NoteDirectory> NoteDirectories;
    DbSet<Image> Images;
    DbSet<Event> Events;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("User");
        builder.Entity<Note>().ToTable("Note");
        builder.Entity<NoteDirectory>().ToTable("Directory");
        builder.Entity<Image>().ToTable("Image");
        builder.Entity<Event>().ToTable("Event");

        builder.Entity<Note>()
            .HasOne(u => u.Owner)
            .WithMany(n => n.Notes)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Note>()
            .HasOne(d => d.Directory)
            .WithMany(n => n.Notes)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Note>()
            .HasMany(c => c.Collaborators);
            //.WithMany(s => s.SharedNotes)

        builder.Entity<Image>()
            .HasOne(n => n.Note)
            .WithMany(i => i.Images);

        builder.Entity<Event>()
            .HasOne(u => u.User)
            .WithMany(e => e.Events);

        builder.Entity<Event>()
            .HasOne(n => n.Note)
            .WithOne(e => e.Event)
            .HasForeignKey<Note>(n => n.Id);
    }
}
