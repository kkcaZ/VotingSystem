using VotingSystem.Data.Enum;

namespace VotingSystem.Data;

public class ElectionInviteModel
{
    public Guid ElectionId { get; set; }
    public string UserEmail { get; set; }
    public ElectionInviteStatus StatusId { get; set; }
}