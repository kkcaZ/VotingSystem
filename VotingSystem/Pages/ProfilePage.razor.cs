using Microsoft.AspNetCore.Components;
using VotingSystem.Data;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;

namespace VotingSystem.Pages;

public partial class ProfilePage : AuthenticatedPage
{
    [Inject] private IUserService _userService { get; set; }
    
    private User _user = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _user = _userService.GetUserById(_userId);
            StateHasChanged();
        }
    }
}