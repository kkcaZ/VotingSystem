using VotingSystem.Data;

namespace VotingSystem.Services.Abstraction;

public interface IUserService
{
    public bool AddUser(ref User user);

    public bool Authenticate(string nationalIdentifier, string password, out User user);
}