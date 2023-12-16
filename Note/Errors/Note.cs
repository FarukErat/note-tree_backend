using ErrorOr;

namespace Workout.Notes.Errors;

public static class Notes
{
    public static Error NoSavedNote => Error.NotFound(
        code: "Note.NoSavedNote",
        description: "No note was found with the given id."
    );
}