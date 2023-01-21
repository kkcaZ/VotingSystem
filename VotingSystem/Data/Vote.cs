namespace VotingSystem.Data;

public class Vote
{
    public Guid VoterId { get; set; }
    public Guid CandidateId { get; set; }
    public Guid ElectionId { get; set; }
    public DateTime Timestamp { get; set; }
}