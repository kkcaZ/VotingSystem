using VotingSystem.Data;
using VotingSystem.Data.Enum;

namespace VotingSystem.DataAccess.Abstraction;

public interface IUserDataAccess : IDataAccess<User>
{
    /// <summary>
    /// Retrieves <see cref="User"/> with Email Address as the identifier
    /// </summary>
    /// <param name="email">Email of <see cref="User"/></param>
    /// <returns><see cref="User"/> with given <paramref name="email"/></returns>
    public User? GetByEmail(string email);
    
    /// <summary>
    /// Gets all elections that are administered by given <paramref name="id"/>
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns>List of elections that are owned by the user</returns>
    public List<Election> GetUsersAdministeredElections(Guid id);
    
    /// <summary>
    /// Gets all elections that the user with <paramref name="email"/> has been invited to.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public List<Election> GetUsersElectionInvites(string email);

    /// <summary>
    /// Updates status in database for record with <paramref name="electionId"/> & <paramref name="userEmail"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="userEmail"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public int UpdateElectionInviteStatus(Guid electionId, string userEmail, ElectionInviteStatus status);
    
    /// <summary>
    /// Retrieves <see cref="User"/> using the National Identifier
    /// </summary>
    /// <param name="nationalIdentifier">National Identifier of <see cref="User"/></param>
    /// <returns><see cref="User"/> with given <paramref name="nationalIdentifier"/></returns>
    public User? GetByNationalIdentifier(string nationalIdentifier);
}