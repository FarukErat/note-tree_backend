using ErrorOr;
using NoteTree.Notes.Models;

namespace NoteTree.Notes.Interfaces;

public interface INoteService
{
    Task<ErrorOr<Success>> SaveNotesAsync(List<Note> notes, HttpContext httpContext);
    Task<ErrorOr<List<Note>>> GetNotesAsync(HttpContext httpContext);
}