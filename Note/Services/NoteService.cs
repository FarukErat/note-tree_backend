using ErrorOr;
using NoteTree.Authentication.Models;
using NoteTree.Authentication.Interfaces;
using NoteTree.Notes.Interfaces;
using NoteTree.Notes.Models;

namespace NoteTree.Authentication.Services;

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
        string noteRecordId = await _noteDb.SaveNotesOfUser(
            new NoteRecord()
            {
                Id = session.NoteRecordId,
                Notes = notes
            });

        if (session.NoteRecordId == null)
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
            return new List<Note>();
        }
        return await _noteDb.GetNotesOfUser(session.NoteRecordId);
    }
}