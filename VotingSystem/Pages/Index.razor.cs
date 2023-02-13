using Microsoft.AspNetCore.Components;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;

namespace VotingSystem.Pages;

public partial class Index : AuthenticatedPage
{
    [Inject] public IUserService _userService { get; set; }

    private Notification _notification = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender || UserId == null)
            return;

        var invites = _userService.GetUsersElectionInvites(UserId);

        if (invites.Count > 0)
        {
            _notification.Header = "New Invite!";
            _notification.Message = "You have been invited to join an election.<br/><a href='/profile/invites'>Click here to view</a>";
            _notification.Type = NotificationType.Success;
            _notification.Visible = true;
            StateHasChanged();
        }
    }
}