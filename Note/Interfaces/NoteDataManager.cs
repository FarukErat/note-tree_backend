using Workout.Notes.Models;

namespace Workout.Notes.Interfaces;

public interface INoteDataManager
{
    Task<string?> SaveNotesOfUser(NoteRecord noteRecord);
    Task<List<Note>> GetNotesOfUser(string userId);
}