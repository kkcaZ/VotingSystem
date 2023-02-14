namespace VotingSystem.Data;

public class ElectionInviteUpdate
{
    public ElectionInviteModel ElectionInvite { get; set; }
    public InviteChangeType ChangeType { get; set; }
}

public enum InviteChangeType
{
    Created,
    Deleted
}