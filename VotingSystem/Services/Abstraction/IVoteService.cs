using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IVoteService
{
    public int CountVotes(Election election, User candidate);
    public bool SubmitVote(Election election, List<User> candidate, User voter);
    
}