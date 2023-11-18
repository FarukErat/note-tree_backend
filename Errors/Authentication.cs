using ErrorOr;

namespace Workout.Errors;

public static class Authentication
{
    public static Error UserNameTaken => Error.Conflict(
        code: "Authentication.UserNameTaken",
        description: "The username is already taken."
    );

    public static Error UserNotFound => Error.NotFound(
        code: "Authentication.UserNotFound",
        description: "No user was found with the given username."
    );

    public static Error IncorrectPassword => Error.Validation(
        code: "Authentication.IncorrectPassword",
        description: "The password is incorrect."
    );

    public static Error NotLoggedIn => Error.Unauthorized(
        code: "Authentication.NotLoggedIn",
        description: "You are not logged in."
    );
}
