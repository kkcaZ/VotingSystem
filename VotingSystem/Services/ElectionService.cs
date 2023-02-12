using VotingSystem.Data;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Services;

public class ElectionService : IElectionService
{
    private readonly IElectionDataAccess _electionDataAccess;

    public ElectionService(IElectionDataAccess electionDataAccess)
    {
        _electionDataAccess = electionDataAccess;
    }
    
    public bool CreateElection(Election election, Guid adminId, List<string> invitedEmails)
    {
        election.Id = Guid.NewGuid();
        var rowsAffected = _electionDataAccess.Add(election);

        if (rowsAffected == 0)
            return false;
        
        var adminRowsAffected = _electionDataAccess.AddElectionAdmin(adminId, election.Id);

        if (adminRowsAffected == 0)
            return false;

        foreach (var email in invitedEmails)
            _electionDataAccess.AddElectionInviteEmail(election.Id, email);

        return true;
    }

    public List<Election> GetUsersElections(Guid userId)
    {
        var elections = _electionDataAccess.GetUsersElections(userId);
        return elections;
    }

    public bool DeleteElection(Guid electionId)
    {
        var rowsAffected = _electionDataAccess.Delete(electionId);
        
        if (rowsAffected == 0)
            return false;

        return true;
    }
}