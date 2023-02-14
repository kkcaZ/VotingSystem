using Microsoft.AspNetCore.Identity;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
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

    /// <inheritdoc/>
    public User GetUserById(Guid id)
    {
        User user = _userDataAccess.GetById(id);
        return user;
    }

    /// <inheritdoc/>
    public User GetUserByEmail(string email)
    {
        User user = _userDataAccess.GetByEmail(email);
        return user;
    }

    
    /// <inheritdoc/>
    public List<Election> GetUsersAdministeredElections(Guid userId)
    {
        var elections = _userDataAccess.GetUsersAdministeredElections(userId);
        return elections;
    }

    /// <inheritdoc/>
    public List<Election> GetUsersElectionInvites(Guid userId)
    {
        User user = _userDataAccess.GetById(userId);

        if (user == null)
            return new List<Election>();
        
        List<Election> invitedElections = _userDataAccess.GetUsersElectionInvites(user.EmailAddress);
        return invitedElections;
    }

    /// <inheritdoc/>
    public bool UpdateElectionInvite(Guid userId, Guid electionId, ElectionInviteStatus status)
    {
        User user = GetUserById(userId);

        var rowsAffected = _userDataAccess.UpdateElectionInviteStatus(electionId, user.EmailAddress, status);

        if (rowsAffected == 0)
            return false;

        return true;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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