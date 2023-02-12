using VotingSystem.Data;

namespace VotingSystem.DataAccess.Abstraction;

public interface IElectionDataAccess : IDataAccess<Election>
{
    public List<Election> GetUsersElections(Guid id);
    public int AddElectionAdmin(Guid userId, Guid electionId);
    public int AddElectionInviteEmail(Guid electionId, string email);
}