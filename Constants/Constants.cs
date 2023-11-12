namespace Workout.Constants;

public static class ErrorMessages
{
    public const string NoMatchingHashAlgorithm = "Unsupported hashing algorithm.";
    public const string UnauthorizedRequest = "Request is not authorized.";
    public const string UserNotFound = "User not found.";
    public const string UsernameTaken = "Username taken.";
    public const string WrongPassword = "Wrong password.";
    public const string NotLoggedIn = "Not logged in.";
}

public static class HashAlgorithms
{
    public const string Argon2id = "argon2id";
    public const string Bcrypt = "bcrypt";
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string NewUser = "NewUser";
    public const string User = "User";
}

public static class Cookies
{
    public const string SessionId = "SID";
    public const string UserId = "UserId";
    public const string Username = "Username";
    public const string Role = "Role";
}