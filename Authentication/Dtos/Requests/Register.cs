using System.ComponentModel.DataAnnotations;

namespace NoteTree.Authentication.Dtos.Requests;

public class RegisterRequest
{
    [Required]
    [MinLength(6), MaxLength(20)]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [MinLength(6), MaxLength(64)]
    public string Password { get; set; } = string.Empty;
}
