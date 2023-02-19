using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IElectionService
{
    /// <summary>
    /// Retrieves election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public Election GetElectionById(Guid electionId);
    
    /// <summary>
    /// Retrieves list of elections in <paramref name="nation"/>
    /// </summary>
    /// <param name="nation"></param>
    /// <returns></returns>
    public List<Election> GetElectionsByNation(string nation);
    
    /// <summary>
    /// Retrieves list of candidates for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public List<User> GetCandidates(Guid electionId);
    
    /// <summary>
    /// Creates <paramref name="election"/> in db.
    /// Adds admin with <paramref name="adminId"/> to the db.
    /// Adds invites to election for users with emails in <paramref name="invitedEmails"/> to the db.
    /// </summary>
    /// <param name="election"></param>
    /// <param name="adminId"></param>
    /// <param name="invitedEmails"></param>
    /// <returns></returns>
    public bool CreateElection(Election election, Guid adminId, List<ElectionInviteModel> invitedEmails);
    
    /// <summary>
    /// Removes election from the database, removing all records that depend on this election as well
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public bool DeleteElection(Guid electionId);
    
    /// <summary>
    /// Updates <paramref name="election"/> as well as candidate invite changes
    /// </summary>
    /// <param name="election"></param>
    /// <param name="inviteUpdates"></param>
    /// <returns></returns>
    public bool UpdateElection(Election election, List<ElectionInviteUpdate> inviteUpdates);
    
    /// <summary>
    /// Retrieves candidate invites for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public List<ElectionInviteModel> GetElectionCandidateInvites(Guid electionId);
    
    /// <summary>
    /// Adds user with <paramref name="userId"/> as candidate for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public bool AddCandidate(Guid userId, Guid electionId);
    
    /// <summary>
    /// Removes uer with <paramref name="userId"/> as candidate for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool DeleteCandidate(Guid electionId, Guid userId);
}