using VotingSystem.Data;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Services;

public class UserService : IUserService
{
    private readonly IUserDataAccess _userDataAccess;
    
    public UserService(IUserDataAccess userDataAccess)
    {
        _userDataAccess = userDataAccess;
    }
    
    /// <summary>
    /// Adds new user to the database if unique credentials do not clash
    /// </summary>
    /// <param name="user">User to add to the database</param>
    /// <returns>True if user is added to the database, otherwise false</returns>
    public bool AddUser(ref User user)
    {
        // Check if user credentials already exist
        if (_userDataAccess.GetByEmail(user.EmailAddress) != null)
            return false;

        if (_userDataAccess.GetByNationalIdentifier(user.NationalIdentifier) != null)
            return false;
        
        // Create user
        user.Id = Guid.NewGuid();
        _userDataAccess.Add(user);
        return true;
    }

    public bool Authenticate(string nationalIdentifier, string password, out User? authenticatedUser)
    {
        var user = _userDataAccess.GetByNationalIdentifier(nationalIdentifier);

        if (user?.Password == password)
        {
            authenticatedUser = user;
            return true;
        }

        authenticatedUser = null;
        return false;
    }
}