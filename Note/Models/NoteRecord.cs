using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Workout.Notes.Models;

public sealed class NoteRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public List<Note>? Notes { get; set; }
}