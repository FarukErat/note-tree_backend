/*
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations remove
*/

using System.ComponentModel.DataAnnotations.Schema;

namespace Workout.Models;

public class User
{
    // primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    // attributes
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Password Password { get; set; } = null!;
}

public class Password
{
    // primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    // attributes
    public string Hash { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
}
