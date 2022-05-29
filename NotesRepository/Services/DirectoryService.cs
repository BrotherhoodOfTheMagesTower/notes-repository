using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Services
{
    public class DirectoryService
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly DirectoryRepository _dr;



        public DirectoryService(DirectoryRepository directoryRepository)
        {
            _dr = directoryRepository;
        }

        public DirectoryService(NoteRepository noteRepository, DirectoryRepository directoryRepository, UserRepository userRepository)
        {
            _nr = noteRepository;
            _ur = userRepository;
            _dr = directoryRepository;
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
          => await _dr.GetDirectoryByNameAsync("Default", userId);

        public async Task<Directory?> GetBinForParticularUserAsync(string userId)
          => await _dr.GetDirectoryByNameAsync("Bin", userId);

        public async Task<ICollection<Directory>?> GetAllDirectoriesForParticularUserAsync(string userId)
        => await _dr.GetAllDirectoriesForParticularUserAsync(userId);

        public async Task<ICollection<Directory>?> GetAllDirectoriesFromBinForParticularUserAsync(string userId)
        {
            var directory = await _dr.GetDirectoryByNameAsync("Bin", userId);
            if (directory != null)
            {
                return await _dr.GetAllSubDirectoriesOfParticularDirectoryAsync(directory.DirectoryId);
            }
            else return null;
        }

        public async Task<ICollection<Directory>?> GetAllNotDeletedDirectoriesForParticularUserAsync(string userId)
        => await _dr.GetAllNotDeletedDirectoriesForParticularUserAsync(userId);

        public async Task<ICollection<Directory>?> GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(string userId)
        => await _dr.GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(userId);

        public async Task<ICollection<Directory>?> GetAllSubDirectoriesOfParticularDirectoryAsync(Guid directoryId)
        => await _dr.GetAllSubDirectoriesOfParticularDirectoryAsync(directoryId);

        public ICollection<Directory>? GetAllSubDirectoriesOfParticularDirectorySync(Guid directoryId)
            => _dr.GetAllSubDirectoriesOfParticularDirectorySync(directoryId);

        public async Task<(ICollection<Directory>?, ICollection<Note>?)> GetAllSubDirectoriesAndNotesOfParticularDirectoryAsync(Guid directoryId)
        {
            ICollection<Directory>? directories = await _dr.GetAllSubDirectoriesOfParticularDirectoryAsync(directoryId);
            ICollection<Note>? notes = await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);
            return (directories, notes);
        }

        public async Task<bool> MoveDirectorySubdirectoriesAndNotesToBin(Guid directoryId)
        {
            var directory = await _dr.GetByIdAsync(directoryId);
            var bin = await _dr.GetDirectoryByNameAsync("Bin", directory.User.Id);

            var result = await MarkDirectorySubdirectoriesAndNotesAsDeleted(directoryId);

            if (result == true && bin != null)
            {
                await _dr.ChangeParentDirectoryForSubDirectory(directoryId, bin.DirectoryId);
                return true;
            }
            else return false;

        }

        public bool  RemoveDirectoriesSubdirectoriesAndNotesFromBinAndDb(int daysOld=30)
        {
            var directories = _dr.GetMainDirectoriesWhichShouldBeRemovedFromDb(daysOld).ToList();

            //var singleNotes = _nr.GetAllSingleNotesWhichShouldBeRemovedFromDb();
            //if(singleNotes != null)
            //    _nr.DeleteMany(singleNotes);

            if (directories != null)
            {
                foreach (var directory in directories)
                {
                   RemoveSubdirectoriesByDirectoryId(directory.DirectoryId);
                   _dr.DeleteByIdAsync(directory.DirectoryId);
                }
                return true;

            }
            else return false;
        }

        public bool RemoveSubdirectoriesByDirectoryId(Guid directoryId)
        {
            var subDirectories = _dr.GetAllSubDirectoriesOfParticularDirectoryAsync(directoryId).Result;

            if (subDirectories != null)
            {
                if (subDirectories.Count > 0)
                {
                    foreach (var subdirectory in subDirectories)
                    {

                        RemoveSubdirectoriesByDirectoryId(subdirectory.DirectoryId);
                        _dr.DeleteByIdAsync(subdirectory.DirectoryId);
                    }
                }
            }
            return true;

        }

        public async Task<bool> MarkDirectorySubdirectoriesAndNotesAsDeleted(Guid directoryId) //dodac bin jak nadrzedny
        {
            var directory = await _dr.GetByIdAsync(directoryId);

            if (directory == null)
            {
                return false;
            }

            var subDirectories = await _dr.GetAllSubDirectoriesOfParticularDirectoryAsync(directoryId);

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

            var notes = await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);
            if (notes != null)
            {
                if (directory.Notes.Count > 0)
                {
                    foreach (var note in notes)
                    {
                        await _nr.MarkNoteAsDeletedAsync(note.NoteId);
                    }
                }
            }
            await _dr.MarkDirectoryAsDeletedAsync(directoryId);


            return true;
        }
        public async Task<bool> UpdateAsync(Directory _directory)
            => await _dr.UpdateAsync(_directory);
    }
}