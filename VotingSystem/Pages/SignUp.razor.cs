using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using VotingSystem.Data;
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
    
    [Inject] private IUserService _userService { get; set; }

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
    private void SignUpButton()
    {
        if (!ValidatePassword(_userDetails.Password, _confirmPassword))
                return;

        if (_userService.AddUser(ref _userDetails))
        {
            
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