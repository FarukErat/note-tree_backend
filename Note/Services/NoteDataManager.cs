using MongoDB.Bson;
using MongoDB.Driver;
using Workout.Notes.Dtos.Requests;
using Workout.Notes.Interfaces;
using Workout.Notes.Models;

namespace Workout.Authentication.Services;

public sealed class NoteDataManager : INoteDataManager
{
    private readonly IMongoCollection<UserNote> _userNotes;
    public NoteDataManager(ConfigProvider configProvider)
    {
        MongoClient client = new(configProvider.MongoDbConnectionString);
        IMongoDatabase database = client.GetDatabase(configProvider.MongoDbDatabaseName);
        _userNotes = database.GetCollection<UserNote>("UserNotes");
    }

    public async Task<List<Note>> GetNotesOfUser(string userId)
    {
        UserNote? userNote = await _userNotes.Find(x => x.UserId == userId).FirstOrDefaultAsync();

        return userNote?.Notes ?? new List<Note>();
    }

    public Task SaveNotesOfUser(SaveNote saveNote)
    {
        // delete if exists
        _userNotes.DeleteOne(x => x.UserId == saveNote.UserId);

        // insert new
        return _userNotes.InsertOneAsync(
            new UserNote
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = saveNote.UserId,
                Notes = saveNote.Notes
            });
    }
}