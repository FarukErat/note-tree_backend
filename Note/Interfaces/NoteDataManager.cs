using Workout.Notes.Dtos.Requests;
using Workout.Notes.Models;

namespace Workout.Notes.Interfaces;

public interface INoteDataManager
{
    Task SaveNotesOfUser(SaveNote saveNote);
    Task<List<Note>> GetNotesOfUser(string userId);
}