using VotingSystem.Data.Enum;

namespace VotingSystem.Data;

public class Election
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public ElectionType? Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}