using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IVoteService
{
    /// <summary>
    /// Retrieves number of records for each candidate in election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public List<(Guid candidateId, int voteCount)> CountElectionVotes(Guid electionId);
    
    /// <summary>
    /// Retrieves number of votes for candidate with <paramref name="candidateId"/> in election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="candidateId"></param>
    /// <returns></returns>
    public int CountCandidateVotes(Guid electionId, Guid candidateId);
    
    /// <summary>
    /// Submits vote for user with <paramref name="voterId"/> in election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="voterId"></param>
    /// <param name="candidateId"></param>
    /// <param name="electionId"></param>
    /// <returns></returns>
    public bool SubmitVote(Guid voterId, Guid candidateId, Guid electionId);
    
    /// <summary>
    /// Submits vote for user 
    /// </summary>
    /// <param name="vote"></param>
    /// <returns></returns>
    public bool SubmitVote(Vote vote);
    
    /// <summary>
    /// Checks whether user has submitted a vote in election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="voterId"></param>
    /// <returns></returns>
    public bool HasUserVoted(Guid electionId, Guid voterId);
}