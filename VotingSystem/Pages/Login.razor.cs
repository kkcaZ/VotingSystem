using Microsoft.AspNetCore.Components;
using VotingSystem.Data;

namespace VotingSystem.Pages;

public partial class Login : ComponentBase
{
    private User _userDetails = new();
    private string LoginStage = "login-form stage-1";
    
}