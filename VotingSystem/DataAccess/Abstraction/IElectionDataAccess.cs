using VotingSystem.Data;
using VotingSystem.Data.Enum;

namespace VotingSystem.DataAccess.Abstraction;

public interface IElectionDataAccess : IDataAccess<Election>
{
    /// <summary>
    /// Retrieves list of elections with <paramref name="nation"/>
    /// </summary>
    /// <param name="nation"></param>
    /// <returns></returns>
    public List<Election> GetByNation(string nation);
    
    /// <summary>
    /// Retrieves list of election invites for given <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public List<ElectionInviteModel> GetElectionInvites(Guid electionId);
    
    /// <summary>
    /// Retrieves list of users that are candidates of election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public List<User> GetCandidates(Guid electionId);
    
    /// <summary>
    /// Adds user with <paramref name="userId"/> to admin table for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int AddElectionAdmin(Guid userId, Guid electionId);
    
    /// <summary>
    /// Adds user <paramref name="email"/> to election invite table for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="email"></param>
    /// <param name="status"></param>
    /// <returns>Number of rows affected</returns>
    public int AddElectionInviteEmail(Guid electionId, string email, ElectionInviteStatus status = ElectionInviteStatus.Pending);
    
    /// <summary>
    /// Adds user <paramref name="email"/> to election invite table for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionInvite"></param>
    /// <returns>Number of rows affected</returns>
    public int AddElectionInviteEmail(ElectionInviteModel electionInvite);
    
    /// <summary>
    /// Adds user with <paramref name="userId"/> to candidate table for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int AddCandidate(Guid userId, Guid electionId);
    
    /// <summary>
    /// Deletes all candidates for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int DeleteAllCandidates(Guid electionId);
    
    /// <summary>
    /// Deletes all admins for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int DeleteAllAdmins(Guid electionId);
    
    /// <summary>
    /// Deletes all election invites for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int DeleteAllElectionInvites(Guid electionId);
    
    /// <summary>
    /// Deletes all votes for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int DeleteAllVotes(Guid electionId);
    
    /// <summary>
    /// Deletes election invite for user with <paramref name="userEmail"/> on election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="userEmail"></param>
    /// <returns>Number of rows affected</returns>
    public int DeleteUsersElectionInvite(Guid electionId, string userEmail);
    
    /// <summary>
    /// Removes candidate with <paramref name="userId"/> from election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="electionId"></param>
    /// <returns>Number of rows affected</returns>
    public int DeleteCandidate(Guid userId, Guid electionId);
}