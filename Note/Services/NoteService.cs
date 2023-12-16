using ErrorOr;
using Workout.Authentication.Models;
using Workout.Authentication.Interfaces;
using Workout.Notes.Interfaces;
using Workout.Notes.Models;

namespace Workout.Authentication.Services;

public sealed class NoteService : INoteService
{
    private readonly INoteDataManager _noteDb;
    private readonly ICacheService _cacheService;
    private readonly IUserAuthDataManager _userAuthDb;

    public NoteService(INoteDataManager noteDb, ICacheService cacheService, IUserAuthDataManager userAuthDb)
    {
        _noteDb = noteDb;
        _cacheService = cacheService;
        _userAuthDb = userAuthDb;
    }

    public async Task<ErrorOr<Success>> SaveNotesAsync(List<Note> notes, HttpContext httpContext)
    {
        Session? session = (Session?)httpContext.Items["Session"];
        if (session?.UserId == null)
        {
            return Errors.Authentication.NotLoggedIn;
        }

        // save notes to database
        string? noteRecordId = await _noteDb.SaveNotesOfUser(
            new NoteRecord()
            {
                Id = session.NoteRecordId,
                Notes = notes
            });

        if (session.NoteRecordId == null && noteRecordId != null)
        {
            session.NoteRecordId = noteRecordId;
            await _userAuthDb.SetNoteRecordIdAsync(session.UserId, noteRecordId);
            await _cacheService.UpdateSessionByIdAsync(session.UserId, session);
        }

        return Result.Success;
    }

    public async Task<ErrorOr<List<Note>>> GetNotesAsync(HttpContext httpContext)
    {
        Session? session = (Session?)httpContext.Items["Session"];
        if (session?.NoteRecordId == null)
        {
            return Notes.Errors.Notes.NoSavedNote;
        }
        return await _noteDb.GetNotesOfUser(session.NoteRecordId);
    }
}