﻿using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;

namespace NotesRepository.Services
{
    public class CollaboratorsNotesService
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly CollaboratorsNotesRepository _cnr;

        public CollaboratorsNotesService(CollaboratorsNotesRepository collaboratorsNotesRepository, NoteRepository noteRepository, UserRepository userRepository)
        {
            _cnr = collaboratorsNotesRepository;
            _nr = noteRepository;
            _ur = userRepository;
        }

        /// <summary>
        /// Gets all collaborators for particular note
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <returns>A collection with user entities, which are added as collaborators to the given note</returns>
        public async Task<ICollection<ApplicationUser>> GetAllCollaboratorsForNoteAsync(Guid noteId)
            => await _cnr.GetAllCollaboratorsForNote(noteId);

        /// <summary>
        /// Gets all notes, which were shared with given user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with note entites</returns>
        public async Task<ICollection<Note>> GetAllSharedNotesForUserAsync(string userId)
            => await _cnr.GetAllSharedNotesForUser(userId);

        /// <summary>
        /// Adds a collaborator to particular note
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully added; otherwise false</returns>
        public async Task<bool> AddCollaboratorToNoteAsync(string userId, Guid noteId)
        {
            var user = await _ur.GetUserByIdAsync(userId);
            var note = await _nr.GetByIdAsync(noteId);
            if (user is not null && note is not null)
            {
                return await _cnr.AddCollaboratorToNoteAsync(new CollaboratorsNotes(user, note));
            }
            return false;
        }

        /// <summary>
        /// Adds collaborators to the given note
        /// </summary>
        /// <param name="userIds">IDs of users</param>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully added; otherwise false</returns>
        public async Task<bool> AddCollaboratorsToNoteAsync(ICollection<string> userIds, Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if(note is not null)
            {
                var list = new List<CollaboratorsNotes>();
                foreach (var item in userIds)
                {
                    var user = await _ur.GetUserByIdAsync(item);
                    if (user is not null)
                    {
                        list.Add(new CollaboratorsNotes(user, note));
                    }
                    else
                        return false;
                }
                return await _cnr.AddCollaboratorsToNoteAsync(list);
            }
            return false;
        }

        /// <summary>
        /// Deletes a collaborator from particular note
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteCollaboratorFromNoteAsync(string userId, Guid noteId)
            => await _cnr.DeleteCollaboratorFromNoteAsync(noteId, userId);

        /// <summary>
        /// Deletes collaborators from particular note
        /// </summary>
        /// <param name="userIds">IDs of users</param>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteCollaboratorsFromNoteAsync(ICollection<string> userIds, Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                foreach (var item in userIds)
                {
                    var user = await _ur.GetUserByIdAsync(item);
                    if (user is not null)
                    {
                        var result = await _cnr.DeleteCollaboratorFromNoteAsync(note.NoteId, user.Id);
                        if (!result)
                            return false;
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
