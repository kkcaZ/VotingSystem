using VotingSystem.Data.Enum;

namespace VotingSystem.Data;

public class User
{
    public Guid Id { get; set; }

    public string FirstNames { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string EmailAddress { get; set; } = string.Empty;
    
    public string PostCode { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string UniqueIdentifier { get; set; } = string.Empty;

    public string Nationality { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public List<Role> Roles { get; set; } = new List<Role>();
}