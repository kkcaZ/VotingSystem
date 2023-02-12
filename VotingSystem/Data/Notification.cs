using VotingSystem.Data.Enum;

namespace VotingSystem.Data;

public class Notification
{
    public bool Visible = false;
    public string Message = string.Empty;
    public string Header = string.Empty;
    public NotificationType Type;
}