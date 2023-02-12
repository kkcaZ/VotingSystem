using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IElectionService
{
    public List<Election> GetUsersElections(Guid userId);
    public bool CreateElection(Election election, Guid adminId, List<string> invitedEmails);
    public bool DeleteElection(Guid electionId);
}