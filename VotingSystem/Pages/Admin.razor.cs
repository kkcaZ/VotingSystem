using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;

namespace VotingSystem.Pages;

public enum AdminPanel
{
    ElectionList,
    CreateElection,
    EditElection
}

public class ElectionFormModel
{
    public string Title { get; set; } = "Create an Election";
    public Election Election { get; set; } = new();
    public List<ElectionInviteModel> ElectionInvites { get; set; } = new();
    public string CandidateEmail { get; set; } = string.Empty;
    public string ButtonText { get; set; } = "Create Election";
}

public partial class Admin : AuthenticatedPage
{
    private List<Election>? _administeredElections;
    private List<string> _nations = new();
    private List<string> _types = new();
    
    private AdminPanel _adminPanel = AdminPanel.ElectionList;

    private ElectionFormModel _formModel = new ElectionFormModel();

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
        _formModel.ElectionInvites.Add(new ElectionInviteModel()
        {
            UserEmail = _formModel.CandidateEmail,
            StatusId = ElectionInviteStatus.Pending
        });
        _formModel.CandidateEmail = string.Empty;
    }
    
    private void CreateElection()
    {
        if (_formModel.Election.EndTime < _formModel.Election.StartTime)
        {
            
            return;
        }
        
        if (_electionService.CreateElection(_formModel.Election, UserId, _formModel.ElectionInvites))
        {
            _administeredElections = _userService.GetUsersAdministeredElections(UserId);
            OpenElectionListPanel();
            StateHasChanged();
        }
    }

    private void DeleteElection(Guid id)
    {
        _electionService.DeleteElection(id);
        _administeredElections = _userService.GetUsersAdministeredElections(UserId);
        StateHasChanged();
    }

    private void OpenCreateElectionPanel()
    {
        _adminPanel = AdminPanel.CreateElection;
        _formModel = new ElectionFormModel();
        _formModel.Election.Type = null;
        var now = DateTime.Now;
        _formModel.Election.StartTime = new DateTime(now.Year, now.Month, now.Day);
        _formModel.Election.EndTime = new DateTime(now.Year, now.Month, now.Day);
        _formModel.ElectionInvites = new();
    }
    
    private void OpenEditElectionPanel(Guid electionId)
    {
        _adminPanel = AdminPanel.CreateElection;

        var election = _electionService.GetElection(electionId);
        var electionInvites = _electionService.GetElectionCandidateInvites(electionId);
        
        _formModel = new ElectionFormModel()
        {
            Title = $"Edit {election.Name}",
            ButtonText = "Save Changes",
            Election = election,
            ElectionInvites = electionInvites
        };
    }

    private void OpenElectionListPanel()
    {
        _adminPanel = AdminPanel.ElectionList;
    }
}