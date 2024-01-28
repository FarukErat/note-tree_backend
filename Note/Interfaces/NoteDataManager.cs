using NoteTree.Notes.Models;

namespace NoteTree.Notes.Interfaces;

public interface INoteDataManager
{
    Task<string> SaveNotesOfUser(NoteRecord noteRecord);
    Task<List<Note>> GetNotesOfUser(string userId);
}