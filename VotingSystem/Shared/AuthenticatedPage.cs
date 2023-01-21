using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace VotingSystem.Shared;

public class AuthenticatedPage : ComponentBase
{
    [Inject] private ProtectedSessionStorage _sessionStorage { get; set; }
    [Inject] private NavigationManager _navManager { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Redirect user to login if not authenticated 
        var authenticated = await _sessionStorage.GetAsync<bool>("authenticated");
        if (!authenticated.Success || !authenticated.Value)
        {
            _navManager.NavigateTo("/login");
        }
        
        await base.OnInitializedAsync();
    }
}