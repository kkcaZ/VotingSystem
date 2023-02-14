using VotingSystem.Data;
using VotingSystem.Data.Enum;

namespace VotingSystem.DataAccess.Abstraction;

public interface IElectionDataAccess : IDataAccess<Election>
{
    public List<ElectionInviteModel> GetElectionInvites(Guid electionId);
    public int AddElectionAdmin(Guid userId, Guid electionId);
    public int AddElectionInviteEmail(Guid electionId, string email, ElectionInviteStatus status = ElectionInviteStatus.Pending);
    public int AddElectionInviteEmail(ElectionInviteModel electionInvite);
    public int AddCandidate(Guid userId, Guid electionId);
    public int DeleteUsersElectionInvite(Guid electionId, string userEmail);
    public int DeleteCandidate(Guid userId, Guid electionId);
}