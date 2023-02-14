using VotingSystem.Data;

namespace VotingSystem.DataAccess.Abstraction;

public interface IVoteDataAccess
{
    public Vote? Get(Guid electionId, Guid voterId);
    public int GetCandidateVoteCount(Guid electionId, Guid candidateId);
    public int Add(Vote vote);
}