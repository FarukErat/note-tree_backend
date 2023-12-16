using ErrorOr;
using Workout.Notes.Models;

namespace Workout.Notes.Interfaces;

public interface INoteService
{
    Task<ErrorOr<Success>> SaveNotesAsync(List<Note> notes, HttpContext httpContext);
    Task<ErrorOr<List<Note>>> GetNotesAsync(HttpContext httpContext);
}