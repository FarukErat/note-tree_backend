using MongoDB.Bson;
using MongoDB.Driver;
using NoteTree.Notes.Interfaces;
using NoteTree.Notes.Models;

namespace NoteTree.Authentication.Services;

public sealed class NoteDataManager : INoteDataManager
{
    private readonly IMongoCollection<NoteRecord> _noteRecords;
    public NoteDataManager(ConfigProvider configProvider)
    {
        MongoClient client = new(configProvider.MongoDbConnectionString);
        IMongoDatabase database = client.GetDatabase(configProvider.MongoDbDatabaseName);
        _noteRecords = database.GetCollection<NoteRecord>("UserNotes");
    }

    public async Task<List<Note>> GetNotesOfUser(string id)
    {
        NoteRecord? noteRecord = await _noteRecords.Find(x => x.Id == id).FirstOrDefaultAsync();

        return noteRecord?.Notes ?? new List<Note>();
    }

    public async Task<string> SaveNotesOfUser(NoteRecord noteRecord)
    {
        noteRecord.Id ??= ObjectId.GenerateNewId().ToString();
        FilterDefinition<NoteRecord> filter = Builders<NoteRecord>.Filter.Eq(
            x => x.Id,
            noteRecord.Id);
        UpdateDefinition<NoteRecord> update = Builders<NoteRecord>.Update.Set(
                x => x.Notes,
                noteRecord.Notes);

        await _noteRecords.UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = true
        });

        return noteRecord.Id;
    }
}