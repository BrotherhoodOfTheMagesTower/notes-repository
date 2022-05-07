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


        public async Task<bool> MarkDirectorySubdirectoriesAndNotesAsDeleted(Guid directoryId)
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
                    var notes = await _nr.GetAllDirectoryNotesAsync(directoryId);
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