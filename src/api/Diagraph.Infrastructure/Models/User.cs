using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class User : DbEntity
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool Locked { get; set; }
    
    public int UnsuccsessfulLoginAttempts { get; set; }
}