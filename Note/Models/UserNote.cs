using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Workout.Notes.Models;

public sealed class UserNote
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public List<Note>? Notes { get; set; }
}