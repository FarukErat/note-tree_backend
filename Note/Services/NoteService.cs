using ErrorOr;
using Workout.Constants;
using Workout.Authentication.Models;
using Workout.Authentication.Interfaces;
using Workout.Notes.Interfaces;
using Workout.Notes.Dtos.Requests;
using Workout.Notes.Models;

namespace Workout.Authentication.Services;

public sealed class NoteService : INoteService
{
    private readonly INoteDataManager _noteDb;
    private readonly ICacheService _cacheService;

    public NoteService(INoteDataManager noteDb, ICacheService cacheService)
    {
        _noteDb = noteDb;
        _cacheService = cacheService;
    }

    public async Task<ErrorOr<Success>> SaveNotesAsync(List<Note> notes, HttpContext httpContext)
    {
        Session? session = (Session?)httpContext.Items["Session"];
        if (session?.UserId == null)
        {
            return Errors.Authentication.NotLoggedIn;
        }

        // save notes to database
        await _noteDb.SaveNotesOfUser(
            new SaveNote()
            {
                UserId = session.UserId,
                Notes = notes
            });

        return Result.Success;
    }

    public async Task<ErrorOr<List<Note>>> GetNotesAsync(HttpContext httpContext)
    {
        Session? session = (Session?)httpContext.Items["Session"];
        if (session?.UserId == null)
        {
            return Errors.Authentication.NotLoggedIn;
        }
        return await _noteDb.GetNotesOfUser(session.UserId);
    }
}