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

    public Election GetElection(Guid electionId)
    {
        Election election = _electionDataAccess.GetById(electionId);
        return election;
    }

    public List<ElectionInviteModel> GetElectionCandidateInvites(Guid electionId)
    {
        List<ElectionInviteModel> invites = _electionDataAccess.GetElectionInvites(electionId);
        return invites;
    }
    
    public bool CreateElection(Election election, Guid adminId, List<ElectionInviteModel> invitedEmails)
    {
        election.Id = Guid.NewGuid();
        var rowsAffected = _electionDataAccess.Add(election);

        if (rowsAffected == 0)
            return false;
        
        var adminRowsAffected = _electionDataAccess.AddElectionAdmin(adminId, election.Id);

        if (adminRowsAffected == 0)
            return false;

        foreach (var email in invitedEmails)
            _electionDataAccess.AddElectionInviteEmail(election.Id, email.UserEmail);

        return true;
    }

    public bool DeleteElection(Guid electionId)
    {
        var rowsAffected = _electionDataAccess.Delete(electionId);
        
        if (rowsAffected == 0)
            return false;

        return true;
    }

    public bool UpdateElection(Election election, List<ElectionInviteUpdate> inviteUpdates)
    {
        var rowsAffected = _electionDataAccess.Update(election);

        if (rowsAffected == 0)
            return false;
        
        foreach (var update in inviteUpdates)
        {
            if (update.ChangeType == InviteChangeType.Created)
            {
                _electionDataAccess.AddElectionInviteEmail(update.ElectionInvite);
            }
            else
            {
                _electionDataAccess.DeleteUsersElectionInvite(update.ElectionInvite.ElectionId,
                    update.ElectionInvite.UserEmail);
            }
        }

        return true;
    }
}