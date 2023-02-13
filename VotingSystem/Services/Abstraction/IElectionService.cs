using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IElectionService
{
    public Election GetElection(Guid electionId);
    public List<ElectionInviteModel> GetElectionCandidateInvites(Guid electionId);
    public bool CreateElection(Election election, Guid adminId, List<ElectionInviteModel> invitedEmails);
    public bool DeleteElection(Guid electionId);
}