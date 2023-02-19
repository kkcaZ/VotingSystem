using VotingSystem.Data;

namespace VotingSystem.DataAccess.Abstraction;

public interface IVoteDataAccess
{
    /// <summary>
    /// Retrieves vote for user with <paramref name="voterId"/> on election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="voterId"></param>
    /// <returns></returns>
    public Vote? Get(Guid electionId, Guid voterId);
    
    /// <summary>
    /// Retrieves number of votes for candidate with <paramref name="candidateId"/> in election with <paramref name="electionId"/> 
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="candidateId"></param>
    /// <returns></returns>
    public int GetCandidateVoteCount(Guid electionId, Guid candidateId);
    
    /// <summary>
    /// Adds <paramref name="vote"/> to database
    /// </summary>
    /// <param name="vote"></param>
    /// <returns></returns>
    public int Add(Vote vote);
}