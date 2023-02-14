using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace VotingSystem.Shared;

public class AuthenticatedPage : ComponentBase
{
    protected Guid _userId;
    
    [Inject] private ProtectedSessionStorage _sessionStorage { get; set; }
    [Inject] private NavigationManager _navManager { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Check users authentication 
            var authenticated = await _sessionStorage.GetAsync<bool>("authenticated");
            if (!authenticated.Success && !authenticated.Value)
                _navManager.NavigateTo("/login");

            // Get userId from session storage
            var userId = await _sessionStorage.GetAsync<Guid>("userId");
            if (!userId.Success)
                _navManager.NavigateTo("/login");

            _userId = userId.Value;
        }

        await base.OnInitializedAsync();
    }
}