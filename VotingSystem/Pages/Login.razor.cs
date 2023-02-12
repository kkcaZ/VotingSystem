using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using VotingSystem.Data;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Pages;

public partial class Login : ComponentBase
{
    // User
    private User _userDetails = new();
    
    // Styles
    private string _loginStage = "login-form stage-1";
    private string _invalidLoginStyle = "";
    
    // Dropdown
    private List<string> _cultureList = new List<string>();

    // Disables
    private bool _nextBtnDisabled = true;

    [Inject] private IUserService _userService { get; set; }
    [Inject] private ProtectedSessionStorage _sessionStorage { get; set; }
    [Inject] private NavigationManager _navManager { get; set; }

    protected override Task OnInitializedAsync()
    {
        // Load countries into Nationality selection
        // Seems to be unsupported on linux :/
        // foreach(var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
        //     RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);
        //
        //     if (!_cultureList.Contains(regionInfo.EnglishName))
        //         _cultureList.Add(regionInfo.EnglishName);
        // }
        
        _cultureList.Add("United Kingdom");
        _cultureList.Sort();

        return base.OnInitializedAsync();
    }

    private void NationalityUpdated(ChangeEventArgs e)
    {
        _loginStage = "login-form stage-2";
    }

    private void UniqueIdUpdated(ChangeEventArgs e)
    {
        if (e == null || e.Value == null)
            return;

        // NIN Regex from: https://stackoverflow.com/questions/10204378/regular-expression-to-validate-uk-national-insurance-number
        string idPattern = "^(?!BG)(?!GB)(?!NK)(?!KN)(?!TN)(?!NT)(?!ZZ)(?:[A-CEGHJ-PR-TW-Z][A-CEGHJ-NPR-TW-Z])(?:\\s*\\d\\s*){6}([A-D]|\\s)$";
        string input = e.Value.ToString();

        if (Regex.Match(input, idPattern).Success)
            _nextBtnDisabled = false;
        else
            _nextBtnDisabled = true;
    }

    /// <summary>
    /// Handles displaying different stages of login / sign-up to the user
    /// </summary>
    private void SignInButton()
    {
        User user;
        if (_userService.Authenticate(_userDetails.NationalIdentifier, _userDetails.Password, out user))
        {
            _sessionStorage.SetAsync("authenticated", true);
            _sessionStorage.SetAsync("userId", user.Id);
            _navManager.NavigateTo("/");
        }
        else
        {
            _invalidLoginStyle = "invalid";
        }
    }
}