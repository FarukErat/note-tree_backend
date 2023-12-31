namespace NoteTree.Notes.Models;

public sealed class Note
{
    public string? Content { get; set; }
    public List<Note>? Children { get; set; }
}