using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IElectionService
{
    public Election GetElectionById(Guid electionId);
    public List<Election> GetElectionsByNation(string nation);
    public List<User> GetCandidates(Guid electionId);
    public bool CreateElection(Election election, Guid adminId, List<ElectionInviteModel> invitedEmails);
    public bool DeleteElection(Guid electionId);
    public bool UpdateElection(Election election, List<ElectionInviteUpdate> inviteUpdates);
    public List<ElectionInviteModel> GetElectionCandidateInvites(Guid electionId);
    public bool AddCandidate(Guid userId, Guid electionId);
    public bool DeleteCandidate(Guid electionId, Guid userId);
}