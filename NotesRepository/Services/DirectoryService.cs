using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services.Azure;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Services
{
    public class DirectoryService
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly ImageRepository _ir;
        private readonly DirectoryRepository _dr;
        private readonly string containerName = "imagecontainer";
        private AzureStorageHelper _azureHelper;

        public DirectoryService(NoteRepository noteRepository, DirectoryRepository directoryRepository, UserRepository userRepository, ImageRepository imageRepository)
        {
            _nr = noteRepository;
            _ur = userRepository;
            _dr = directoryRepository;
            _ir = imageRepository;
        }

        /// <summary>
        /// Gets the directory entity
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>The directory entity</returns>
        public async Task<Directory?> GetDirectoryByIdAsync(Guid directoryId)
            => await _dr.GetByIdAsync(directoryId);

        /// <summary>
        /// Gets the directory entity
        /// </summary>
        /// <param name="name">Name of the directory</param>
        /// <param name="userId">ID of the user</param>
        /// <returns>The directory entity</returns>
        public async Task<Directory?> GetDirectoryByNameAsync(string name, string userId)
            => await _dr.GetDirectoryByNameAsync(name, userId);

        /// <summary>
        /// Adds the directory entity to database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>True if successfully added; otherwise false</returns>
        public async Task<bool> AddDirectoryAsync(Directory directory)
            => await _dr.AddAsync(directory);

        /// <summary>
        /// Gets the bin directory entity for particular user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>The directory entity</returns>
        public async Task<Directory?> GetBinForParticularUserAsync(string userId)
          => await _dr.GetDirectoryByNameAsync("Bin", userId);

        /// <summary>
        /// Gets all directories for particular user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with directories</returns>
        public async Task<ICollection<Directory>?> GetAllDirectoriesForParticularUserAsync(string userId)
        => await _dr.GetAllDirectoriesForParticularUserAsync(userId);

        /// <summary>
        /// Gets all directories for particular user, which are not deleted
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with directories</returns>
        public async Task<ICollection<Directory>?> GetAllNotDeletedDirectoriesForParticularUserAsync(string userId)
        => await _dr.GetAllNotDeletedDirectoriesForParticularUserAsync(userId);

        /// <summary>
        /// Gets all directories without parent directory for particular user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with directories</returns>
        public async Task<ICollection<Directory>?> GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(string userId)
        => await _dr.GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(userId);

        /// <summary>
        /// Gets all directories without parent directory for particular user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with directories</returns>
        public ICollection<Directory>? GetAllDirectoriesWithoutParentDirectoryForParticularUserSync(string userId)
        => _dr.GetAllDirectoriesWithoutParentDirectoryForParticularUserSync(userId);

        /// <summary>
        /// Gets all subdirectories of particular directory
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>A collection with subdirectories</returns>
        public ICollection<Directory>? GetAllSubDirectoriesOfParticularDirectorySync(Guid directoryId)
            => _dr.GetAllSubDirectoriesOfParticularDirectorySync(directoryId);

        /// <summary>
        /// Changes the parent directory for given subdirectory
        /// </summary>
        /// <param name="subDirectoryId">ID of the subdirectory</param>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>True if successfully changed; otherwise false</returns>
        public async Task<bool> ChangeParentDirectoryForSubDirectoryAsync(Guid subDirectoryId, Guid directoryId)
            => await _dr.ChangeParentDirectoryForSubDirectoryAsync(subDirectoryId, directoryId);

        /// <summary>
        /// Removes directories with notes and subdirectories from bin
        /// </summary>
        /// <param name="daysOld">Amount of days old</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public bool CascadeRemoveDirectoriesWithStructureOfSubdirectoriesAndNotesFromBinAndDbByDaysSync(int daysOld = 30)
        {
            var directories = _dr.GetMainDirectoriesWhichShouldBeRemovedFromDbSync(daysOld).ToList();

            var singleNotes = _nr.GetAllSingleNotesWhichShouldBeRemovedFromDb(daysOld);
            if (singleNotes.Count > 0)
            {
                DeleteImagesFromNotesFromTheListSync(singleNotes);
                _nr.DeleteMany(singleNotes);
            }

            if (directories.Count > 0)
            {
                foreach (var directory in directories)
                {
                    var subDirectoryNotes = _nr.GetAllNotesForParticularDirectory(directory.DirectoryId);
                    DeleteImagesFromNotesFromTheListSync(subDirectoryNotes);
                    RemoveSubdirectoriesByDirectoryId(directory.DirectoryId);
                    _dr.DeleteByIdSync(directory.DirectoryId);
                }
                return true;

            }
            else return false;
        }

        /// <summary>
        /// Removes directory with notes and subdirectories from bin
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> CascadeRemoveDirectoryWithStructureOfSubdirectoriesAndNotesFromBinAsync(Guid directoryId)
        {
            var directory = await _dr.GetByIdAsync(directoryId);
            if (directory != null)
            {
                RemoveSubdirectoriesByDirectoryId(directory.DirectoryId);
                _dr.DeleteByIdSync(directory.DirectoryId);
                return true;
            }
            return false;

        }

        /// <summary>
        /// Removes subdirectories for the given directory
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public bool RemoveSubdirectoriesByDirectoryId(Guid directoryId)
        {
            var subDirectories = _dr.GetAllSubDirectoriesOfParticularDirectorySync(directoryId);

            if(subDirectories != null)
            {
                if (subDirectories.Count > 0)
                {
                    foreach (var subdirectory in subDirectories)
                    {
                        var subDirectoryNotes = _nr.GetAllNotesForParticularDirectory(subdirectory.DirectoryId);
                        DeleteImagesFromNotesFromTheListSync(subDirectoryNotes);
                        RemoveSubdirectoriesByDirectoryId(subdirectory.DirectoryId);
                        _dr.DeleteByIdSync(subdirectory.DirectoryId);
                    }
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Deletes images from given notes
        /// </summary>
        /// <param name="notesList">A collection of notes</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public bool DeleteImagesFromNotesFromTheListSync(ICollection<Note> notesList)
        {
            if (notesList != null)
            {
                if (notesList.Count > 0)
                {
                    foreach (var note in notesList)
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
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves directory to the bin
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>True if successfully moved; otherwise false</returns>
        public async Task<bool> MoveDirectoryWithStructureOfSubdirectoriesAndNotesToBinAsync(Guid directoryId)
        {
            var directory = await _dr.GetByIdAsync(directoryId);
            var bin = await _dr.GetDirectoryByNameAsync("Bin", directory.User.Id);

            if (directory != null && bin != null)
            {
                var result = await CascadeMarkDirectoryWithStructureOfSubdirectoriesAndNotesAsDeletedAsync(directoryId);
                if (result == true)
                {
                    await _dr.ChangeParentDirectoryForSubDirectoryAsync(directoryId, bin.DirectoryId);
                    return true;
                }
                else return false;
            }
            else return false;

        }

        /// <summary>
        /// Restores directory with notes & subdirectories to given directory
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <param name="parentDirectoryId">ID of the parent directory</param>
        /// <returns>True if successfully restored; otherwise false</returns>
        public async Task<bool> CascadeRestoreDirectoryWithStructureOfSubdirectoriesAndNotesFromBinToDirectoryAsync(Guid directoryId, Guid parentDirectoryId)
        {
            var directory = await _dr.GetByIdAsync(directoryId);
            var parentDirectory = await _dr.GetByIdAsync(parentDirectoryId);

            if (directory != null && parentDirectory != null)
            {
                var result = await CascadeMarkDirectoryWithStructureOfSubdirectoriesAndNotesAsNotDeletedAsync(directoryId);
                if (result == true)
                {
                    await _dr.ChangeParentDirectoryForSubDirectoryAsync(directoryId, parentDirectoryId);
                    return true;
                }
                else return false;
            }
            else return false;

        }

        /// <summary>
        /// Marks directory, subdirectories & notes as deleted
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>True if successfully marked; otherwise false</returns>
        public async Task<bool> CascadeMarkDirectoryWithStructureOfSubdirectoriesAndNotesAsDeletedAsync(Guid directoryId)
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

                        await CascadeMarkDirectoryWithStructureOfSubdirectoriesAndNotesAsDeletedAsync(subdirectory.DirectoryId);
                    }
                }
            }

            var notes = await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);
            if (notes != null)
            {
                if (notes.Count > 0)
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

        /// <summary>
        /// Marks directory, subdirectories & notes as NOT deleted
        /// </summary>
        /// <param name="directoryId">ID of the directory</param>
        /// <returns>True if successfully marked; otherwise false</returns>
        public async Task<bool> CascadeMarkDirectoryWithStructureOfSubdirectoriesAndNotesAsNotDeletedAsync(Guid directoryId)
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

                        await CascadeMarkDirectoryWithStructureOfSubdirectoriesAndNotesAsNotDeletedAsync(subdirectory.DirectoryId);
                    }
                }
            }

            var notes = await _nr.GetAllNotesForParticularDirectoryAsync(directoryId);
            if (notes != null)
            {
                if (notes.Count > 0)
                {
                    foreach (var note in notes)
                    {
                        await _nr.MarkNoteAsNotDeletedAsync(note.NoteId);
                    }
                }
            }
            await _dr.MarkDirectoryAsNotDeletedAsync(directoryId);


            return true;
        }

        /// <summary>
        /// Updates the directory entity
        /// </summary>
        /// <param name="_directory">The directory entity</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Directory _directory)
            => await _dr.UpdateAsync(_directory);

        /// <summary>
        /// Checks if a directory entity with given title exists for particular user
        /// </summary>
        /// <param name="title">Title of the directory</param>
        /// <param name="userId">ID of the user</param>
        /// <returns>True if exists; otherwise false</returns>
        public async Task<bool> CheckIfTheFolderTitleExistsForParticularUserAsync(string title, string userId)
        {
            var directory = await _dr.GetDirectoryByNameAsync(title, userId);
            if (directory == null)
                return false;
            else return true;
        }
    }
}