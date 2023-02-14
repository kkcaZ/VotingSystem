using VotingSystem.Data;
using VotingSystem.Data.Enum;

namespace VotingSystem.Services.Abstraction;

public interface IUserService
{
    /// <summary>
    /// Gets user with <paramref name="id"/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public User GetUserById(Guid id);

    /// <summary>
    /// Gets user with <paramref name="email"/>
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public User GetUserByEmail(string email);
    
    /// <summary>
    /// Gets elections that the user administers
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<Election> GetUsersAdministeredElections(Guid userId);
    
    /// <summary>
    /// Gets users invites to elections
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<Election> GetUsersElectionInvites(Guid userId);

    /// <summary>
    /// Updates election invite status for account with <paramref name="userId"/> to be <paramref name="status"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public bool UpdateElectionInvite(Guid userId, Guid electionId, ElectionInviteStatus status);

    /// <summary>
    /// Adds new user to the database if unique credentials do not clash
    /// </summary>
    /// <param name="user">User to add to the database</param>
    /// <returns>True if user is added to the database, otherwise false</returns>
    public bool AddUser(ref User user);

    /// <summary>
    /// Checks that the login credentials are correct for the user & returns their data in <paramref name="user"/>
    /// </summary>
    /// <param name="nationalIdentifier"></param>
    /// <param name="password"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool Authenticate(string nationalIdentifier, string password, out User user);
}