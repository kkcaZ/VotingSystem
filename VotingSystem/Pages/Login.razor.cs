using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text.RegularExpressions;
using VotingSystem.Data;

namespace VotingSystem.Pages;

public partial class Login : ComponentBase
{
    private User _userDetails = new();
    private string _loginStage = "login-form stage-1";
    private List<string> _cultureList = new List<string>();

    private bool _nextBtnDisabled = true;
    private bool _idInputDisabled = false;

    private string _confirmPassword = "";
    private string _passwordValidationStyle = "";

    protected override Task OnInitializedAsync()
    {
        // Load countries into Nationality selection
        foreach(var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
            RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);

            if (!_cultureList.Contains(regionInfo.EnglishName))
                _cultureList.Add(regionInfo.EnglishName);
        }

        _cultureList.Sort();

        return base.OnInitializedAsync();
    }

    private void NationalityUpdated(ChangeEventArgs e)
    {
        // Change label name for Unique Identifier. E.g. for United Kingdom, this is National Insurance Number
        if (e.Value?.ToString() == "United Kingdom")
        {

        }

        _loginStage = "login-form stage-2";
        _idInputDisabled = false;
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

    private void NextButton()
    {
        if (_loginStage.Contains("stage-2"))
        {
            _loginStage = "login-form stage-3";
            _idInputDisabled = true;
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
    /// <param name="e"></param>
    private void ValidatePassword(string password, string confirmationPassword)
    { 
        if (password == confirmationPassword)
            _passwordValidationStyle = "valid";
        else
            _passwordValidationStyle = "invalid";

        StateHasChanged();
    }
}