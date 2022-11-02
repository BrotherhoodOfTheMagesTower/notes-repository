using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Repositories;
using NotesRepository.Services;
using SeleniumTests.Constants;
using SeleniumTests.Infrastructure.Builders;
using System;

namespace SeleniumTests.Extensions;

public static class SeederExtensions
{
    /// <summary>
    /// Creates user accounts
    /// </summary>
    /// <param name="ur">the UserRepository object</param>
    /// <param name="usersCount">Amount of accounts to be created</param>
    /// <returns>A collection with user Tags (user ID and user email)</returns>
    public static async Task<IReadOnlyCollection<Tag>> CreateUsers(this UserRepository ur, int usersCount)
    {
        var userTags = new List<Tag>();

        for (int i = 0; i < usersCount; i++)
        {
            var id = Guid.NewGuid();
            var email = Faker.Internet.Email();
            var user = new ApplicationUserBuilder()
                .WithId(id.ToString())
                .WithFirstName(Faker.Name.First())
                .WithLastName(Faker.Name.Last())
                .WithEmail(email)
                .Build();
            user.HashPassword();
            await ur.AddUserAsync(user);
            userTags.Add(new Tag(id, email));
        }

        return userTags;
    }

    /// <summary>
    /// Creates directories. Additionaly for each user will be a Default and Bin directory created.
    /// </summary>
    /// <param name="ds">the DirectoryService object</param>
    /// <param name="directoriesPerAccountCount">Amount of directories per account to be created</param>
    /// <param name="users">Users, for which directories will be created</param>
    /// <param name="ur">the UserRepository object</param>
    /// <returns>A collection with directories Tags (directory ID and directory name)</returns>
    public static async Task<IReadOnlyCollection<Tag>> CreateDirectories(this DirectoryService ds, int directoriesPerAccountCount, IReadOnlyCollection<Tag> users,
        UserRepository ur)
    {
        var directoriesTags = new List<Tag>();

        foreach (var user in users)
        {
            directoriesTags.AddRange(await ds.CreateDefaultDirectoryAndBin(user.Id, ur, directoriesTags));

            var userObj = await ur.GetUserByIdAsync(user.Id.ToString());

            for (int i = 0; i < directoriesPerAccountCount; i++)
            {
                var id = Guid.NewGuid();
                var name = Faker.Address.City();
                var directory = new DirectoryBuilder()
                    .WithDirectoryId(id)
                    .WithName(name)
                    .WithUser(userObj!)
                    .Build();
                await ds.AddDirectoryAsync(directory);
                directoriesTags.Add(new Tag(id, name));
            }
        }

        return directoriesTags;
    }

    /// <summary>
    /// Creates notes for the Default directory
    /// </summary>
    /// <param name="ns">the NoteService object</param>
    /// <param name="notesPerAccountCount">Amount of notes per each user to be created</param>
    /// <param name="users">Users, for which directories will be created</param>
    /// <param name="ur">the UserRepository object</param>
    /// <param name="ds">the DirectoryService object</param>
    /// <returns>A collection with notes Tags (note ID and note title)</returns>
    public static async Task<IReadOnlyCollection<Tag>> CreateNotesForDefaultDirectory(this NoteService ns, int notesPerAccountCount, IReadOnlyCollection<Tag> users,
        UserRepository ur, DirectoryService ds)
    {
        var random = new Random();
        var noteTags = new List<Tag>();

        foreach (var user in users)
        {
            var userObj = await ur.GetUserByIdAsync(user.Id.ToString());

            for (int i = 0; i < notesPerAccountCount; i++)
            {
                var id = Guid.NewGuid();
                var title = SeederData.NoteTitle(i + 1);
                var dir = await ds.GetDirectoryByNameAsync("Default", user.Id.ToString());
                var note = new NoteBuilder()
                    .WithNoteId(id)
                    .WithTitle(title)
                    .WithContent(Faker.Lorem.Sentence())
                    .WithCreatedAt(new DateTime(2019, random.Next(1, 12), random.Next(1, 28), random.Next(0, 12), random.Next(0, 58), 0))
                    .WithEditedAt(new DateTime(2021, random.Next(1, 12), random.Next(1, 28), random.Next(0, 12), random.Next(0, 58), 0))
                    .WithOwner(userObj!)
                    .WithDirectory(dir!)
                    .Build();
                await ns.AddNoteAsync(note);
                noteTags.Add(new Tag(id, title));
            }
        }

        return noteTags;
    }

    /// <summary>
    /// Creates events
    /// </summary>
    /// <param name="es">the EventService object</param>
    /// <param name="eventsPerAccountCount">Amount of events per user to be created</param>
    /// <param name="users">Users, for which events will be created</param>
    /// <param name="ur">the UserRepository object</param>
    /// <returns>A collection with event Tags (event ID and event content)</returns>
    public static async Task<IReadOnlyCollection<Tag>> CreateEvents(this EventService es, int eventsPerAccountCount, IReadOnlyCollection<Tag> users,
        UserRepository ur)
    {
        var random = new Random();
        var eventTags = new List<Tag>();

        foreach (var user in users)
        {
            var userObj = await ur.GetUserByIdAsync(user.Id.ToString());

            for (int i = 0; i < eventsPerAccountCount; i++)
            {
                var id = Guid.NewGuid();
                var startAt = DateTime.Now.AddDays(random.Next(minValue: 2, maxValue: 14));
                var _event = new EventBuilder()
                    .WithEventId(id)
                    .WithContent(SeederData.EventContent(i + 1))
                    .WithStartAt(startAt)
                    .WithEndAt(startAt.AddDays(2).AddHours(random.Next(minValue: 1, maxValue: 3)))
                    .WithUser(userObj!)
                    .Build();
                await es.AddAsync(_event);
                eventTags.Add(new Tag(id, SeederData.EventContent(i + 1)));
            }
        }

        return eventTags;
    }

    /// <summary>
    /// Creates images
    /// </summary>
    /// <param name="ims">the ImageService object</param>
    /// <param name="imagesPerAccountCount">Amount of images per user that will be created</param>
    /// <param name="users">Users, for which directories will be created</param>
    /// <param name="ns">the NoteService object</param>
    /// <param name="config">Configuration</param>
    /// <returns>A collection with images Tags (image ID and image name)</returns>
    public static async Task<IReadOnlyCollection<Tag>> CreateImages(this ImageService ims, int imagesPerAccountCount, IReadOnlyCollection<Tag> users,
        NoteService ns, IConfigurationRoot config)
    {
        var random = new Random();
        var imagesTags = new List<Tag>();

        foreach (var user in users)
        {
            var noteObj = (await ns.GetAllUserNotesByIdAsync(user.Id.ToString())).First();

            for (int i = 0; i < imagesPerAccountCount; i++)
            {
                var id = Guid.NewGuid();
                var imgName = FileNames.defaultImages.ElementAt(random.Next(0, 2));
                var image = new ImageBuilder()
                    .WithImageId(id)
                    .WithName(imgName)
                    .WithNote(noteObj)
                    .Build();

                var filePath = $"{Directory.GetCurrentDirectory()}\\Infrastructure\\Images\\{imgName}";
                (_, var imgUrl) = await ims.AddImageFromPathAsync(image, filePath, config.GetSection("ConnectionStrings")["StorageConnectionString"]);

                var note = await ns.GetNoteByIdAsync(noteObj.NoteId);
                note!.Content += $"\n ![image]({imgUrl})";
                await ns.UpdateNoteAsync(noteObj);
                imagesTags.Add(new Tag(id, imgName));
            }
        }

        return imagesTags;
    }
    
    /// <summary>
    /// Creates collaborators
    /// </summary>
    /// <param name="cns">the CollaboratorsNotesService object</param>
    /// <param name="amountOfAccounts">Amount of accounts - for each pair will be a shared note created</param>
    /// <param name="users">Users, for which shared notes will be created</param>
    /// <param name="ns">The note service object</param>
    /// <returns>A collectiono of tuples, that contain a pair of user emails, that that have shared notes - Item1 is the Owner, Item2 is the collaborator</returns>
    public static async Task<IReadOnlyCollection<Tuple<string, string>>> CreateCollaborators(this CollaboratorsNotesService cns, int amountOfAccounts, IReadOnlyCollection<Tag> users,
        NoteService ns, UserRepository ur, DirectoryService ds)
    {
        var collaborators = new List<Tuple<string, string>>();
        for (int i = 0; i < users.Count; i = i + 2)
        {
            var userObj = await ur.GetUserByIdAsync(users.ElementAt(i).Id.ToString());
            var collaborator = await ur.GetUserByIdAsync(users.ElementAt(i + 1).Id.ToString());
            var title = SeederData.sharedNoteTitle;
            var dir = await ds.GetDirectoryByNameAsync("Default", users.ElementAt(i).Id.ToString());
            var noteId = Guid.NewGuid();
            var note = new NoteBuilder()
                .WithNoteId(noteId)
                .WithTitle(title)
                .WithContent(SeederData.sharedNoteContent)
                .WithCreatedAt(new DateTime(2019, 1, 12, 1, 0, 58, 0))
                .WithEditedAt(new DateTime(2021, 1, 12, 1, 0, 58, 0))
                .WithOwner(userObj!)
                .WithDirectory(dir!)
                .Build();
            await ns.AddNoteAsync(note);
            await cns.AddCollaboratorToNoteAsync(collaborator.Id.ToString(), noteId);
            collaborators.Add(new Tuple<string, string>(userObj.Email, collaborator.Email));
        }

        return collaborators;
    }

    private static async Task<List<Tag>> CreateDefaultDirectoryAndBin(this DirectoryService ds, Guid userId, UserRepository ur, List<Tag> directoriesTags)
    {
        var userObj = await ur.GetUserByIdAsync(userId.ToString());
        var defId = Guid.NewGuid();
        var binId = Guid.NewGuid();
        var defName = "Default";
        var binName = "Bin";
        var defaultDir = new DirectoryBuilder()
            .WithDirectoryId(defId)
            .WithName(defName)
            .WithUser(userObj!)
            .Build();
        var binDir = new DirectoryBuilder()
            .WithDirectoryId(binId)
            .WithName(binName)
            .WithUser(userObj!)
            .Build();
        await ds.AddDirectoryAsync(defaultDir);
        await ds.AddDirectoryAsync(binDir);
        directoriesTags.Add(new Tag(defId, defName));
        directoriesTags.Add(new Tag(binId, binName));

        return directoriesTags;
    }

    private static void HashPassword(this ApplicationUser user, string psswd = SeederData.password)
    {
        var password = new PasswordHasher<ApplicationUser>();
        var hashed = password.HashPassword(user, psswd);
        user.PasswordHash = hashed;
    }
}
