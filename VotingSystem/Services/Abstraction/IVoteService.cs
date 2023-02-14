using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IVoteService
{
    public int CountVotes(Election election, User candidate);
    public bool SubmitVote(Guid voterId, Guid candidateId, Guid electionId);
    public bool SubmitVote(Vote vote);
    public bool HasUserVoted(Guid electionId, Guid voterId);
}