using VotingSystem.Data;

namespace VotingSystem.DataAccess.Abstraction;

public interface IUserDataAccess : IDataAccess<User>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public User? GetByEmail(string email);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nationalIdentifier"></param>
    /// <returns></returns>
    public User? GetByNationalIdentifier(string nationalIdentifier);
}