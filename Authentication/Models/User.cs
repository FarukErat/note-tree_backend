/*
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations remove
*/

using System.ComponentModel.DataAnnotations.Schema;

namespace Workout.Authentication.Models;

public class User
{
    // primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    // attributes
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordAlgo { get; set; } = string.Empty;
}
