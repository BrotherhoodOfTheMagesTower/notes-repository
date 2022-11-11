using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services.Azure;

namespace NotesRepository.Services
{
    public class NoteService
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly EventRepository _er;
        private readonly DirectoryRepository _dr;
        private readonly ImageRepository _ir;
        private readonly string containerName = "imagecontainer";
        private AzureStorageHelper _azureHelper;

        public NoteService(NoteRepository noteRepository)
        {
            _nr = noteRepository;
        }

        public NoteService(NoteRepository noteRepository, UserRepository userRepository, EventRepository eventRepository, DirectoryRepository directoryRepository, ImageRepository imageRepository)
        {
            _nr = noteRepository;
            _ur = userRepository;
            _er = eventRepository;
            _dr = directoryRepository;
            _ir = imageRepository;
        }

        public NoteService(NoteRepository noteRepository, UserRepository userRepository, EventRepository eventRepository, DirectoryRepository directoryRepository, ImageRepository imageRepository, AzureStorageHelper azureStorageHelper)
        {
            _nr = noteRepository;
            _ur = userRepository;
            _er = eventRepository;
            _dr = directoryRepository;
            _ir = imageRepository;
            _azureHelper = azureStorageHelper;
        }

        public async Task<Note?> GetNoteByIdAsync(Guid noteId)
            => await _nr.GetByIdAsync(noteId);

        public Note? GetNoteById(Guid noteId)
            => _nr.GetByIdSync(noteId);

        public async Task<List<Note>> GetAllUserNotesByIdAsync(string userId)
            => (await _nr.GetAllUserNotesAsync(userId)).ToList();

        public async Task<List<Note>> GetAllUserNotesWithoutEventAsync(string userId)
            => (await _nr.GetAllUserNotesWithoutEventAsync(userId)).ToList();

        public async Task<Note?> GetNoteByTitleAsync(string title, string userId)
            => await _nr.GetNoteByTitleAsync(title, userId);

        /// <summary>
        /// Gets all notes from the database, that are assigned to specific directory 
        /// </summary>
        /// <param name="directoryId">The unique ID of Directory, which notes will be returned</param>
        /// <returns>A collection of notes assigned to particular directory, that are currently stored in the database</returns>
        public ICollection<Note> GetAllNotesForParticularDirectory(Guid directoryId)
            => _nr.GetAllNotesForParticularDirectory(directoryId);

        /// <summary>
        /// Gets all notes from the database, that are were moved to the bin by single delete.
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of notes from particular user that are were moved to the bin by single delete</returns>
        public async Task<ICollection<Note>> GetAllSingleNotesFromUserThatAreCurrentlyInRecycleBinAsync(string userId)
            => await _nr.GetAllNotesFromParticularUserThatAreCurrentlyInRecycleBinAsync(userId);

        /// <summary>
        /// Gets all notes from the database, that are were moved to the bin by single delete
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of notes from particular user that are were moved to the bin by single delete</returns>
        public async Task<ICollection<Note>> GetAllPinnedNotesFromUserAsync(string userId)
            => await _nr.GetAllPinnedNotesFromUserAsync(userId);

        /// <summary>
        /// Gets recently edited notes
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="count">Amount of notes to return</param>
        /// <returns>A collection of recently edited notes</returns>
        public async Task<ICollection<Note>> GetRecentlyEditedNotesAsync(string userId, int count = 10)
            => await _nr.GetRecentlyEditedNotesAsync(userId, count);


        /// <summary>
        /// Gets notes by title and content
        /// </summary>
        /// <param name="searchText">The phrase which should be searched</param>
        /// <param name="userId">ID of the user</param>
        /// <returns>A list with results (if any)</returns>
        public async Task<List<Note>> SearchNotesByTitleAndContentAsync(string searchText, string userId)
            => await _nr.SearchNoteByTitleAndContentAsync(searchText, userId);

        /// <summary>
        /// Adds a note entity to the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>True if successfully added; otherwise false</returns>
        public async Task<bool> AddNoteAsync(Note note)
            => await _nr.AddAsync(note);

        public async Task<bool> AddNotesAsync(ICollection<Note> notes)
            => await _nr.AddManyAsync(notes);

        /// <summary>
        /// Updates a note entity
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>True if successfully updated; otherwise false</returns>
        public async Task<bool> UpdateNoteAsync(Note note)
            => await _nr.UpdateAsync(note);

        /// <summary>
        /// Deletes a note entity from the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteNoteAsync(Note note)
        {
            var imagesAttachedToNote = await _ir.GetAllNoteImagesAsync(note.NoteId);    // podpięte zdjęcia pod daną notatkę
            if (imagesAttachedToNote != null) // Jeżeli jakieś są to usuwamy
            {
                foreach (var image in imagesAttachedToNote)
                {
                    await _azureHelper.DeleteImageFromAzure(image.Name, containerName); // usuwamy z Azure
                    await _ir.DeleteAsync(image);   // Usuwamy z naszej bazy
                }
            }
            return await _nr.DeleteAsync(note); // Jeżeli nie to usuwamy odrazu notatkę
        }

        /// <summary>
        /// Deletes a note entity from the database
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteNoteByIdAsync(Guid noteId)
        {
            var imagesAttachedToNote = await _ir.GetAllNoteImagesAsync(noteId);
            if (imagesAttachedToNote != null && imagesAttachedToNote.Any())
            {
                foreach (var image in imagesAttachedToNote)
                {
                    await _azureHelper.DeleteImageFromAzure(image.Name, containerName);
                    await _ir.DeleteAsync(image);
                }
            }
            return await _nr.DeleteByIdAsync(noteId);
        }

        /// <summary>
        /// Deletes note entities from the database
        /// </summary>
        /// <param name="notes">A collection of note entities</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteNotesAsync(ICollection<Note> notes)
        {
            foreach (var note in notes)
            {
                var imagesAttachedToNote = await _ir.GetAllNoteImagesAsync(note.NoteId);

                if (imagesAttachedToNote != null)
                {
                    foreach (Image image in imagesAttachedToNote)
                        await _azureHelper.DeleteImageFromAzure(image.Name, containerName);
                    await _ir.DeleteManyAsync(imagesAttachedToNote);
                }

            }
            return await _nr.DeleteManyAsync(notes);
        }

        /// <summary>
        /// Marks the note as currently being edited
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully marked as currently edited; otherwise false</returns>
        public async Task<bool> SetNoteAsCurrentlyEditedAsync(Guid noteId)
            => await _nr.MarkNoteAsCurrentlyEditedAsync(noteId);

        /// <summary>
        /// Marks the note as currently not being edited
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully marked as currently not edited; otherwise false</returns>
        public async Task<bool> SetNoteAsCurrentlyNotEditedAsync(Guid noteId)
            => await _nr.MarkNoteAsCurrentlyNotEditedAsync(noteId);

        /// <summary>
        /// Sets the last edited time and user
        /// </summary>
        /// <param name="editedAt">Time at which it was edited</param>
        /// <param name="userId">ID of the user, which edited the note</param>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully set; otherwise false</returns>
        public async Task<bool> SetLastEditedTimeAndUserAsync(DateTime editedAt, string userId, Guid noteId)
        {
            var user = await _ur.GetUserByIdAsync(userId);
            var note = await _nr.GetByIdAsync(noteId);
            if (user is not null && note is not null)
            {
                note.EditedAt = editedAt;
                note.EditedBy = user;
                return await _nr.UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// Moves a single note to the bin (IsDeleted = true; DeletedAt = now)
        /// </summary>
        /// <param name="noteId">The note ID</param>
        /// <returns>true, if the note was successfuly moved to the bin and removed from the current directory; otherwise false</returns>
        public async Task<bool> MoveSingleNoteToBinAsync(Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                var isMarkedAsDeleted = await _nr.MarkNoteAsDeletedAsync(noteId);
                var bin = await _dr.GetDirectoryByNameAsync("Bin", note.Owner.Id);
                var isMovedToBin = await ChangeNoteDirectoryAsync(noteId, bin.DirectoryId);
                if (isMarkedAsDeleted && isMovedToBin)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Restores a note from the bin to the given directory
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="directoryId"></param>
        /// <returns>true, if the note was successfuly restored from the bin to the given directory</returns>
        public async Task<bool> RestoreASingleNoteFromTheBinAsync(Guid noteId, Guid directoryId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                var isMarkedAsNotDeleted = await _nr.MarkNoteAsNotDeletedAsync(noteId);
                var dir = await _dr.GetByIdAsync(directoryId);
                if (dir is not null)
                {
                    var isMovedToNewDir = await ChangeNoteDirectoryAsync(noteId, directoryId);
                    if (isMarkedAsNotDeleted && isMovedToNewDir)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Pins a note
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully pinned; otherwise false</returns>
        public async Task<bool> PinNoteAsync(Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                note.IsPinned = true;
                return await _nr.UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// Edits the note content
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <param name="newContent">The new content</param>
        /// <returns>True if successfully edited; otherwise false</returns>
        public async Task<bool> EditNoteContentAsync(Guid noteId, string newContent)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                note.Content = newContent;
                return await _nr.UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// Edits the note title
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <param name="newTitle">The new title</param>
        /// <returns>True if successfully edited; otherwise false</returns>
        public async Task<bool> EditNoteTitleAsync(Guid noteId, string newTitle)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                note.Title = newTitle;
                return await _nr.UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// Changes the note directory
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <param name="newDirectoryId">ID of the new directory</param>
        /// <returns>True if successfully changed; otherwise false</returns>
        public async Task<bool> ChangeNoteDirectoryAsync(Guid noteId, Guid newDirectoryId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            var directory = await _dr.GetByIdAsync(newDirectoryId);
            if (note is not null && directory is not null)
            {
                note.Directory = directory;
                return await _nr.UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// Checks if a note with particular title exists for given user
        /// </summary>
        /// <param name="title">Title of the note</param>
        /// <param name="userId">ID of the user</param>
        /// <returns>True if exists; otherwise false</returns>
        public async Task<bool> CheckIfTheNoteTitleExistsForParticularUser(string title, string userId)
        {
            var note = await _nr.GetNoteByTitleAsync(title, userId);
            if (note == null)
                return false;
            else return true;
        }


        /// <summary>
        /// Generates a random title from note saved from draft
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <param name="userId">ID of the user</param>
        /// <returns>The note entity</returns>
        public async Task<Note> GenerateTitleForNoteFromDraft(Note note, string userId)
        {
            var title = "SavedFromDraft_" + note.CreatedAt.Year + "-" + note.CreatedAt.Month + "-" + note.CreatedAt.Day;
            bool titleExists = true;
            int counter = 0;
            while (titleExists)
            {
                titleExists = await CheckIfTheNoteTitleExistsForParticularUser(title, userId);
                if (titleExists)
                {
                    title = "SavedFromDraft_" + note.CreatedAt.Year + "-" + note.CreatedAt.Month + "-" + note.CreatedAt.Day + "_" + counter;
                    counter++;
                }
            }
            note.Title = title;
            var newNote = note;
            return newNote;
        }
    }
}
