namespace server.Users.Models;

public record User()
{
    public int Id;
    public required string Name;
    public required string Password;
    public required bool Admin;
    public required string Email;
}