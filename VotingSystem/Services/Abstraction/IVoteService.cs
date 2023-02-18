using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IVoteService
{
    public List<(Guid candidateId, int voteCount)> CountElectionVotes(Guid electionId);
    public int CountCandidateVotes(Guid electionId, Guid candidateId);
    public bool SubmitVote(Guid voterId, Guid candidateId, Guid electionId);
    public bool SubmitVote(Vote vote);
    public bool HasUserVoted(Guid electionId, Guid voterId);
}