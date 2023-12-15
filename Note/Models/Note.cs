namespace Workout.Notes.Models;

public sealed class Note
{
    public string? Content { get; set; }
    public List<Note>? SubNotes { get; set; }
}