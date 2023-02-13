using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IElectionService
{
    public bool CreateElection(Election election, Guid adminId, List<ElectionInviteModel> invitedEmails);
    public bool DeleteElection(Guid electionId);
}