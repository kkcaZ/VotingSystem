using VotingSystem.Data;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Services;

public class VoteService : IVoteService
{
    private readonly IVoteDataAccess _voteDataAccess;
    
    public VoteService(IVoteDataAccess voteDataAccess)
    {
        _voteDataAccess = voteDataAccess;
    }
    
    public int CountVotes(Election election, User candidate)
    {
        throw new NotImplementedException();
    }

    public bool SubmitVote(Guid voterId, Guid candidateId, Guid electionId)
    {
        return SubmitVote(new Vote()
        {
            VoterId = voterId,
            CandidateId = candidateId,
            ElectionId = electionId,
            Timestamp = DateTime.Now
        });
    }
    
    public bool SubmitVote(Vote vote)
    {
        var rowsAffected = _voteDataAccess.Add(vote);
        
        if (rowsAffected == 0)
            return false;

        return true;
    }

    public bool HasUserVoted(Guid electionId, Guid voterId)
    {
        return _voteDataAccess.Get(electionId, voterId) != null;
    }
}