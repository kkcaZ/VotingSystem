using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.Pages;

public partial class SignUp
{
    private User _userDetails = new();
    private string _signupStage = "signup-form stage-1";
    private List<string> _cultureList = new List<string>();

    private bool _signUpBtnDisabled = true;

    private string _confirmPassword = "";
    private string _passwordValidationStyle = "";

    private Notification notification = new();
    
    [Inject] private IUserService _userService { get; set; }
    [Inject] private ProtectedSessionStorage _sessionStorage { get; set; }
    [Inject] private NavigationManager _navManager { get; set; }

    protected override Task OnInitializedAsync()
    {
        _cultureList.Add("United Kingdom");

        return base.OnInitializedAsync();
    }

    private void NationalityUpdated(ChangeEventArgs e)
    {
        _signupStage = "login-form stage-2";
    }
    
    private void NationalIdUpdated(ChangeEventArgs e)
    {
        if (e == null || e.Value == null)
            return;

        // NIN Regex from: https://stackoverflow.com/questions/10204378/regular-expression-to-validate-uk-national-insurance-number
        string idPattern = "^(?!BG)(?!GB)(?!NK)(?!KN)(?!TN)(?!NT)(?!ZZ)(?:[A-CEGHJ-PR-TW-Z][A-CEGHJ-NPR-TW-Z])(?:\\s*\\d\\s*){6}([A-D]|\\s)$";
        string input = e.Value.ToString();

        if (Regex.Match(input, idPattern).Success)
            _signUpBtnDisabled = false;
        else
            _signUpBtnDisabled = true;
    }
    
    /// <summary>
    /// Handles displaying different stages of login / sign-up to the user
    /// </summary>
    private async Task SignUpButton()
    {
        if (!ValidatePassword(_userDetails.Password, _confirmPassword))
        {
            notification.Message = "Passwords do not match.";
            notification.Header = "Invalid Field!";
            notification.Type = NotificationType.Failure;
            notification.Visible = true;
            StateHasChanged();
            return;
        }

        if (string.IsNullOrWhiteSpace(_userDetails.NationalIdentifier) ||
            string.IsNullOrWhiteSpace(_userDetails.FirstNames) ||
            string.IsNullOrWhiteSpace(_userDetails.Surname) ||
            string.IsNullOrWhiteSpace(_userDetails.EmailAddress) ||
            string.IsNullOrWhiteSpace(_userDetails.PhoneNumber) ||
            string.IsNullOrWhiteSpace(_userDetails.PostCode) ||
            string.IsNullOrWhiteSpace(_userDetails.Address) ||
            string.IsNullOrWhiteSpace(_userDetails.Password) ||
            string.IsNullOrWhiteSpace(_confirmPassword)
            )
        {
            notification.Header = "Empty Fields!";
            notification.Message = "You must enter information in all fields.";
            notification.Type = NotificationType.Failure;
            notification.Visible = true;
            return;
        }

        // Regex from: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        if (!Regex.IsMatch(_userDetails.EmailAddress,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase))
        {
            notification.Header = "Invalid Email!";
            notification.Message = "Your email address is not formatted correctly.";
            notification.Type = NotificationType.Failure;
            notification.Visible = true;
            return;
        }

        notification.Visible = false;

        if (_userService.AddUser(ref _userDetails))
        {
            _sessionStorage.SetAsync("authenticated", true);
            _sessionStorage.SetAsync("userId", _userDetails.Id);
            _navManager.NavigateTo("/");
        }
    }

    /// <summary>
    /// Password confirmation input has changed
    /// </summary>
    /// <param name="e"></param>
    private void ConfirmPasswordChanged(ChangeEventArgs e) => ValidatePassword(_userDetails.Password, e.Value?.ToString());

    /// <summary>
    /// Password input has changed
    /// </summary>
    /// <param name="e"></param>
    private void PasswordChanged(ChangeEventArgs e) => ValidatePassword(e.Value.ToString(), _confirmPassword);

    /// <summary>
    /// Validate whether two passwords are equal
    /// </summary>
    /// <param name="password"></param>
    /// <param name="confirmationPassword">Password to compare to <paramref name="password"/></param>
    private bool ValidatePassword(string password, string confirmationPassword)
    {
        if (password == confirmationPassword)
        {
            _passwordValidationStyle = "valid";
            _signUpBtnDisabled = false;
            StateHasChanged();
            return true;
        }
        else
        {
            _passwordValidationStyle = "invalid";
            _signUpBtnDisabled = true;
            StateHasChanged();
            return false;
        }
    }
}