using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Services;

public class ElectionService : IElectionService
{
    private readonly IElectionDataAccess _electionDataAccess;
    private readonly IUserService _userService;

    public ElectionService(IElectionDataAccess electionDataAccess, IUserService userService)
    {
        _electionDataAccess = electionDataAccess;
        _userService = userService;
    }

    public Election GetElectionById(Guid electionId)
    {
        Election election = _electionDataAccess.GetById(electionId);
        return election;
    }

    public List<Election> GetElectionsByNation(string nation)
    {
        var elections = _electionDataAccess.GetByNation(nation);
        return elections;
    }

    public List<ElectionInviteModel> GetElectionCandidateInvites(Guid electionId)
    {
        List<ElectionInviteModel> invites = _electionDataAccess.GetElectionInvites(electionId);
        return invites;
    }

    public List<User> GetCandidates(Guid electionId)
    {
        List<User> candidates = _electionDataAccess.GetCandidates(electionId);
        return candidates;
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
        // Update base details
        var rowsAffected = _electionDataAccess.Update(election);

        if (rowsAffected == 0)
            return false;
        
        // Update election candidate info
        foreach (var update in inviteUpdates)
        {
            if (update.ChangeType == InviteChangeType.Created)
            {
                _electionDataAccess.AddElectionInviteEmail(update.ElectionInvite);
            }
            else
            {
                // Revoke user invite
                _electionDataAccess.DeleteUsersElectionInvite(update.ElectionInvite.ElectionId,
                    update.ElectionInvite.UserEmail);

                // Delete user from candidate table
                if (update.ElectionInvite.StatusId == ElectionInviteStatus.Accepted)
                {
                    var user = _userService.GetUserByEmail(update.ElectionInvite.UserEmail);
                    _electionDataAccess.DeleteCandidate(update.ElectionInvite.ElectionId, user.Id);
                }
            }
        }

        return true;
    }

    public bool AddCandidate(Guid userId, Guid electionId)
    {
        var rowsAffected = _electionDataAccess.AddCandidate(userId, electionId);
        
        if (rowsAffected == 0)
            return false;

        return true;
    }

    public bool DeleteCandidate(Guid electionId, Guid userId)
    {
        var rowsAffected = _electionDataAccess.DeleteCandidate(userId, electionId);

        if (rowsAffected == 0)
            return false;

        return true;
    }
}