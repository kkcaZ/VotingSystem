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
    CreateElection
}

public class ElectionFormModel
{
    public string Title { get; set; } = "Create an Election";
    public Election Election { get; set; } = new();
    public List<ElectionInviteModel> ElectionInvites { get; set; } = new();
    public string CandidateEmail { get; set; } = string.Empty;
    public string ButtonText { get; set; } = "Create Election";
    public List<ElectionInviteUpdate> InviteChanges { get; set; } = new();
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
            _administeredElections = _userService.GetUsersAdministeredElections(_userId);
            StateHasChanged();
        }
    }

    private void ElectionFormButtonPressed()
    {
        switch (_formModel.ButtonText)
        {
            case "Create Election":
                CreateElection();
                return;
            case "Save Changes":
                SaveElectionChanges();
                return;
            default:
                return;
        }
    }
    
    private void CreateElection()
    {
        if (_formModel.Election.EndTime < _formModel.Election.StartTime)
        {
            
            return;
        }
        
        if (_electionService.CreateElection(_formModel.Election, _userId, _formModel.ElectionInvites))
        {
            _administeredElections = _userService.GetUsersAdministeredElections(_userId);
            OpenElectionListPanel();
            StateHasChanged();
        }
    }

    private void DeleteElection(Guid id)
    {
        _electionService.DeleteElection(id);
        _administeredElections = _userService.GetUsersAdministeredElections(_userId);
        StateHasChanged();
    }

    private void SaveElectionChanges()
    {
        if (_electionService.UpdateElection(_formModel.Election, _formModel.InviteChanges))
        {
            _adminPanel = AdminPanel.ElectionList;
            _administeredElections = _userService.GetUsersAdministeredElections(_userId);
            StateHasChanged();
        }
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

    private void AddCandidateInvite()
    {
        // Init invite
        var electionInvite = new ElectionInviteModel()
        {
            ElectionId = _formModel.Election.Id,
            UserEmail = _formModel.CandidateEmail,
            StatusId = ElectionInviteStatus.Pending
        };
        
        // Add election invite
        _formModel.ElectionInvites.Add(electionInvite);
        
        // Track change in invite
        _formModel.InviteChanges.Add(new()
        {
            ElectionInvite = electionInvite,
            ChangeType = InviteChangeType.Created
        });

        // Empty the text input for emails (better ux)
        _formModel.CandidateEmail = string.Empty;
    }
    
    private void RemoveCandidateInvite(Guid electionId, string userEmail, ElectionInviteStatus status)
    {
        // Init invite
        var invite = _formModel.ElectionInvites.FirstOrDefault(x => x.ElectionId == electionId && x.UserEmail == userEmail);

        // Remove from invite list
        _formModel.ElectionInvites.Remove(invite);
        
        // If item has been added during this edit / create session, then just remove from the changes
        var existingAddedInvite = _formModel.InviteChanges.Where(x =>
            x.ElectionInvite.ElectionId == electionId &&
            x.ElectionInvite.UserEmail == userEmail &&
            x.ChangeType == InviteChangeType.Created).ToList();

        if (existingAddedInvite.Count > 0)
        {
            _formModel.InviteChanges.Remove(existingAddedInvite.First());
            _formModel.ElectionInvites.Remove(invite);
            return;
        }
        
        // Add Invite Change that indicates item should be removed from database
        _formModel.InviteChanges.Add(new ElectionInviteUpdate()
        {
            ElectionInvite = invite,
            ChangeType = InviteChangeType.Deleted
        });
    }
}