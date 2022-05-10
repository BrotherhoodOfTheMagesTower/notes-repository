using NotesRepository.Repositories;
using NotesRepository.Data.Models;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Services
{
    public class DirectoryService
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly DirectoryRepository _dr;

        private readonly NoteService _ns;

        public DirectoryService(DirectoryRepository directoryRepository)
        {
            _dr = directoryRepository;
        }

        public DirectoryService(NoteRepository noteRepository, DirectoryRepository directoryRepository, NoteService noteService)
        {
            _nr = noteRepository;
           // _ur = userRepository;
            _dr = directoryRepository;
            _ns = noteService;
        }

        public async Task<Directory?> GetDirectoryByIdAsync(Guid directoryId)
            => await _dr.GetByIdAsync(directoryId);

        public async Task<Directory?> GetDirectoryByNameAsync(string name, string userId)
            => await _dr.GetDirectoryByNameAsync(name, userId);

        public async Task<bool> AddDirectoryAsync(Directory directory)
            => await _dr.AddAsync(directory);

        public async Task<bool> DeleteDirectoryByIdAsync(Guid directoryId)
            => await _dr.DeleteByIdAsync(directoryId);

        public async Task<bool> DeleteManyDirectoriesAsync(ICollection<Directory> directories)
           => await _dr.DeleteManyAsync(directories);
        public async Task<bool> DeleteDirectoryAsync(Directory directory)
            => await _dr.DeleteAsync(directory);


        public async Task<Directory?> GetDefaultDirectoryForParticularUserAsync(string userId)
        
          =>  await _dr.GetDirectoryByNameAsync("Default", userId);

        public async Task<Directory?> GetBinForParticularUserAsync(string userId)

          => await _dr.GetDirectoryByNameAsync("Bin", userId);

        public async Task<ICollection<Directory>?> GetAllDirectoriesForParticularUserAsync(string userId)
        => await _dr.GetAllDirectoriesForParticularUserAsync(userId);

        public async Task<ICollection<Directory>?> GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(string userId)
        => await _dr.GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(userId);

        public async Task<ICollection<Directory>?> GetAllSubDirectoriesOfParticularDirectoryAsync(Guid directoryId)
        => await _dr.GetAllSubDirectoriesOfParticularDirectory(directoryId);

        public async Task<(ICollection<Directory>?, ICollection<Note>?)> GetAllSubDirectoriesAndNotesOfParticularDirectoryAsync(Guid directoryId)
        {
            ICollection<Directory>? directories = await _dr.GetAllSubDirectoriesOfParticularDirectory(directoryId);
            ICollection<Note>? notes = await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);
            return (directories, notes);
        }





        public async Task<bool> MarkDirectorySubdirectoriesAndNotesAsDeleted(Guid directoryId) //dodac bin jak nadrzedny
        {
            var directory = await _dr.GetByIdAsync(directoryId);

            if(directory == null)
            {
                return false;
            }

            var subDirectories = await _dr.GetAllSubDirectoriesOfParticularDirectory(directoryId);

            if (subDirectories != null)
            {
                if (subDirectories.Count > 0)
                {
                    foreach (var subdirectory in subDirectories)
                    {
                       
                       await MarkDirectorySubdirectoriesAndNotesAsDeleted(subdirectory.DirectoryId);
                    }
                }
            }       
            if (directory.Notes != null)
            {
                
                if (directory.Notes.Count > 0)
                {
                    var notes = await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);
                    foreach (var note in notes)
                    {
                        await _ns.MarkNoteAsDeletedAndStartTimerAsync(note.NoteId);
                    }
                }
            }

            await _dr.MarkDirectoryAsDeletedAsync(directoryId);
            return true;
        }
    }
}