using Workout.Authentication.Models;
using Workout.Notes.Models;

namespace Workout.Notes.Dtos.Requests
{
    public sealed class SaveNote
    {
        public string? UserId { get; set; }
        public List<Note>? Notes { get; set; }
    }
}