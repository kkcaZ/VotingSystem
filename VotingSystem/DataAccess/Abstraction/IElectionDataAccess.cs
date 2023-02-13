using VotingSystem.Data;

namespace VotingSystem.DataAccess.Abstraction;

public interface IElectionDataAccess : IDataAccess<Election>
{
    public List<ElectionInviteModel> GetElectionInvites(Guid electionId);
    public int AddElectionAdmin(Guid userId, Guid electionId);
    public int AddElectionInviteEmail(Guid electionId, string email);
}