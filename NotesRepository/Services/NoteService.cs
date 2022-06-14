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
        public async Task<ICollection<Note>> GetAllNotesForParticularDirectoryAsync(Guid directoryId)
            => await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);

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

        public async Task<ICollection<Note>> GetRecentlyEditedOrCreatedNotesAsync(string userId, int count = 10)
            => await _nr.GetRecentlyEditedOrCreatedNotesAsync(userId, count);

        public async Task<List<Note>> SearchNotesByTitleAndContentAsync(string searchText, string userId)
            => await _nr.SearchNoteByTitleAndContentAsync(searchText, userId);

        public async Task<bool> AddNoteAsync(Note note)
            => await _nr.AddAsync(note);

        public async Task<bool> AddNotesAsync(ICollection<Note> notes)
            => await _nr.AddManyAsync(notes);

        public async Task<bool> UpdateNoteAsync(Note note)
            => await _nr.UpdateAsync(note);

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

        public bool DeleteNote(Note note)
        {
            var imagesAttachedToNote = _ir.GetAllNoteImages(note.NoteId);
            if (imagesAttachedToNote != null)
            {
                foreach (var image in imagesAttachedToNote)
                {
                    _azureHelper.DeleteImageFromAzureNotAsync(image.Name, containerName);
                    _ir.Delete(image);
                }
            }
            return _nr.Delete(note);
        }

        public async Task<bool> DeleteNoteByIdAsync(Guid noteId)
        {
            var imagesAttachedToNote = await _ir.GetAllNoteImagesAsync(noteId);
            if (imagesAttachedToNote != null)
            {
                foreach (var image in imagesAttachedToNote)
                {
                    await _azureHelper.DeleteImageFromAzure(image.Name, containerName);
                    await _ir.DeleteAsync(image);
                }
            }
            return await _nr.DeleteByIdAsync(noteId);
        }

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

        public async Task<bool> SetNoteAsCurrentlyEditedAsync(Guid noteId)
            => await _nr.MarkNoteAsCurrentlyEditedAsync(noteId);
        
        public async Task<bool> SetNoteAsCurrentlyNotEditedAsync(Guid noteId)
            => await _nr.MarkNoteAsCurrentlyNotEditedAsync(noteId);
        
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
            if(note is not null)
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
            if(note is not null)
            {
                var isMarkedAsNotDeleted = await _nr.MarkNoteAsNotDeletedAsync(noteId);
                var dir = await _dr.GetByIdAsync(directoryId);
                if(dir is not null)
                {
                    var isMovedToNewDir = await ChangeNoteDirectoryAsync(noteId, directoryId);
                    if (isMarkedAsNotDeleted && isMovedToNewDir)
                        return true;
                }
            }
            return false;
        }

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
        
        public async Task<bool> UnpinNoteAsync(Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                note.IsPinned = false;
                return await _nr.UpdateAsync(note);
            }
            return false;
        }
        
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

        public async Task<bool> CheckIfTheNoteTitleExistsForParticularUser(string title, string userId)
        {
            var directory = await _nr.GetNoteByTitleAsync(title, userId);
            if (directory == null)
                return false;
            else return true;
        }

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
                    title = "SavedFromDraft_" + note.CreatedAt.Year + "-" + note.CreatedAt.Month + "-" + note.CreatedAt.Day + "-" + counter;
                    counter++;
                }
            }
            note.Title = title;
            var newNote = note;
            return newNote;
        }
    }
}
