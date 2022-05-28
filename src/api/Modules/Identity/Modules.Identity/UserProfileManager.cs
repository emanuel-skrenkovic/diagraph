using Diagraph.Modules.Identity.Database;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity;

// TODO: maybe better to have a generic
// GetOrAdd extension method on DbContext.
public class UserProfileManager
{
    private readonly IdentityDbContext _context;
    
    public UserProfileManager(IdentityDbContext context)
        => _context = context;
    
    // TODO: need better way than a 'saveChanges' flag.
    public async Task<UserProfile> GetOrAddProfile(Guid userId, bool saveChanges = false)
    {
        UserProfile profile = await _context
            .Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (profile == null)
        {
            profile = _context
                .Profiles
                .Add(new UserProfile { UserId = userId })
                .Entity;
        }

        if (saveChanges) await _context.SaveChangesAsync();

        return profile;
    }
}