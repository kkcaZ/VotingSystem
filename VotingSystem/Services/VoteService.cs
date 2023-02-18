using Microsoft.AspNetCore.Razor.TagHelpers;
using VotingSystem.Data;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Services;

public class VoteService : IVoteService
{
    private readonly IVoteDataAccess _voteDataAccess;
    private readonly IElectionService _electionService;
    
    public VoteService(IVoteDataAccess voteDataAccess, IElectionService electionService)
    {
        _voteDataAccess = voteDataAccess;
        _electionService = electionService;
    }

    public List<(Guid candidateId, int voteCount)> CountElectionVotes(Guid electionId)
    {
        var candidates = _electionService.GetCandidates(electionId);
        List<(Guid candidateId, int voteCount)> voteCounts = new List<(Guid candidateId, int voteCount)>();
        
        foreach (var candidate in candidates)
        {
            var count = CountCandidateVotes(electionId, candidate.Id);
            voteCounts.Add((candidate.Id, count));
        }

        return voteCounts;
    }
    
    public int CountCandidateVotes(Guid electionId, Guid candidateId)
    {
        var candidateVoteCount = _voteDataAccess.GetCandidateVoteCount(electionId, candidateId);
        return candidateVoteCount;
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