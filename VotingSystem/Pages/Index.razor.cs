using Microsoft.AspNetCore.Components;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;
using Notification = VotingSystem.Shared.Notification;

namespace VotingSystem.Pages;

public partial class Index : AuthenticatedPage
{
    [Inject] private IUserService _userService { get; set; }
    [Inject] private IElectionService _electionService { get; set; }

    private Notification _notification = new();

    private string _userName = string.Empty;
    private List<Election> _currentElections = new();
    private List<Election> _upcomingElections = new();
    private List<Election> _pastElections = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        // Page initialisation after authenticated
        if (!firstRender || _userId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            return;

        // Get Election Invites
        var invites = _userService.GetUsersElectionInvites(_userId);

        if (invites.Count > 0)
        {
            _notification.Header = "New Invite!";
            _notification.Message = "You have been invited to join an election.<br/><a href='/profile/invites'>Click here to view</a>";
            _notification.Type = NotificationType.Success;
            _notification.Visible = true;
        }
        
        // Get User
        var user = _userService.GetUserById(_userId);
        _userName = user.FirstNames;
        
        // Get Elections
        var elections = _electionService.GetElectionsByNation(user.Nationality);
        _currentElections = elections.Where(e => e.StartTime < DateTime.Now && e.EndTime > DateTime.Now).ToList();
        _upcomingElections = elections.Where(e => e.StartTime > DateTime.Now).ToList();
        _pastElections = elections.Where(e => e.EndTime < DateTime.Now).ToList();

        StateHasChanged();
    }
}