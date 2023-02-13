using Microsoft.AspNetCore.Components;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;
using Notification = VotingSystem.Shared.Notification;

namespace VotingSystem.Pages;

public partial class ElectionInvite : AuthenticatedPage
{
    [Inject] private IUserService _userService { get; set; }

    private List<Election> _invites = new();
    private Notification _notification = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender || UserId == null)
            return;

        _invites = _userService.GetUsersElectionInvites(UserId);
        if (_invites.Count > 0) 
            StateHasChanged();
    }

    private void AcceptInvite(Guid electionId)
    {
        _userService.UpdateElectionInvite(UserId, electionId, ElectionInviteStatus.Accepted);
        _invites = _userService.GetUsersElectionInvites(UserId);

        _notification.Header = "Accepted!";
        _notification.Message = "You have accepted the election invite.";
        _notification.Visible = true;
        
        StateHasChanged();
    }

    private void DeclinedInvite(Guid electionId)
    {
        _userService.UpdateElectionInvite(UserId, electionId, ElectionInviteStatus.Declined);
        _invites = _userService.GetUsersElectionInvites(UserId);
        
        _notification.Header = "Declined!";
        _notification.Message = "You have declined the election invite.";
        _notification.Type = NotificationType.Failure;
        _notification.Visible = true;
        
        StateHasChanged();
    }
}