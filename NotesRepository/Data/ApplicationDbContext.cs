using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Directory> Directories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<CollaboratorsNotes> CollaboratorsNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("User");
        builder.Entity<Note>().ToTable("Note");
        builder.Entity<Directory>().ToTable("Directory");
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

        builder.Entity<Directory>()
            .HasOne(u => u.User)
            .WithMany(d => d.Directories)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Directory>()
            .HasMany(s => s.SubDirectories)
            .WithOne(p => p.ParentDir)
            .HasForeignKey("SubDirectoryId");


        // collaborators
        builder.Entity<CollaboratorsNotes>()
            .HasKey(t => new { t.ApplicationUserId, t.NoteId });
        builder.Entity<CollaboratorsNotes>()
            .HasOne(cn => cn.SharedNote)
            .WithMany(c => c.CollaboratorsNotes)
            .HasForeignKey(n => n.NoteId);
        builder.Entity<CollaboratorsNotes>()
            .HasOne(cn => cn.Collaborator)
            .WithMany(c => c.CollaboratorsNotes)
            .HasForeignKey(n => n.ApplicationUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Image>()
            .HasOne(n => n.Note)
            .WithMany(i => i.Images);

        builder.Entity<Event>()
            .HasOne(u => u.User)
            .WithMany(e => e.Events);

        builder.Entity<Event>()
            .HasOne(n => n.Note)
            .WithOne(e => e.Event)
            .IsRequired(false)
            .HasForeignKey<Event>(n => n.EventId);
    }
}
