/*
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations remove
*/

using NoteTree.Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace NoteTree.Authentication.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public AppDbContext(DbContextOptions options) : base(options) { }
}