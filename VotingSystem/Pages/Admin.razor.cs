using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.VisualBasic;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;

namespace VotingSystem.Pages;

public partial class Admin : AuthenticatedPage
{
    private Election _election = new();
    private List<Election>? _administeredElections;
    private List<string> _nations = new();
    private List<string> _types = new();
    
    private string _candidateEmail = string.Empty;
    private List<string> _candidates = new();

    [Inject] private IElectionService _electionService { get; set; }
    [Inject] private IUserService _userService { get; set; }
    [Inject] private ProtectedSessionStorage _sessionStorage { get; set; }

    protected override Task OnInitializedAsync()
    {
        // Create nations
        _nations = new List<string>()
        {
            "United Kingdom"
        };

        // Create Election types
        var electionTypes = Enum.GetValues(typeof(ElectionType));
        foreach (var type in electionTypes)
        {
            // Used to split string by capital letter into word array
            // From: https://codereview.stackexchange.com/questions/133707/extension-method-splitting-string-on-each-capital-letter
            var words = 
                Regex.Matches(type.ToString(), @"([A-Z][a-z]+)")
                    .Cast<Match>()
                    .Select(m => m.Value);
            
            _types.Add(string.Join(" ", words));
        }

        // Initialise default election
        _election.Type = null;
        var now = DateTime.Now;
        _election.StartTime = new DateTime(now.Year, now.Month, now.Day);
        _election.EndTime = new DateTime(now.Year, now.Month, now.Day);

        _candidates = new();

        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        
        if (firstRender) {
            // Load administered elections
            _administeredElections = _userService.GetUsersAdministeredElections(UserId);
            StateHasChanged();
        }
    }

    private void AddCandidateButton()
    {
        _candidates.Add(_candidateEmail);
        _candidateEmail = string.Empty;
    }
    
    private void CreateElection()
    {
        if (_election.EndTime < _election.StartTime)
        {
            
            return;
        }
        
        if (_electionService.CreateElection(_election, UserId, _candidates))
        {
            Console.WriteLine("Election Created");
            _administeredElections = _userService.GetUsersAdministeredElections(UserId);
        }
    }

    private void DeleteElection(Guid id)
    {
        _electionService.DeleteElection(id);
        _administeredElections = _userService.GetUsersAdministeredElections(UserId);
        StateHasChanged();
    }
}