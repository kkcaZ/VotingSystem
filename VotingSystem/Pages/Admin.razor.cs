using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;
using Notification = VotingSystem.Data.Notification;

namespace VotingSystem.Pages;

public enum AdminPanel
{
    ElectionList,
    CreateElection,
    ViewElection
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
    [Inject] private IElectionService _electionService { get; set; }
    [Inject] private IVoteService _voteService { get; set; }
    [Inject] private IUserService _userService { get; set; }
    
    // Elections that user current administers
    private List<Election>? _administeredElections;
    
    // Election select values
    private List<string> _nations = new();
    private List<string> _types = new();
    
    // Dictates which panel is showing
    private AdminPanel _adminPanel = AdminPanel.ElectionList;

    // Create / Edit election form
    private ElectionFormModel _formModel = new ElectionFormModel();

    // View Election
    private Election _election = new();
    private List<User> _candidates = new();
    private List<(Guid candidateId, int votes)> _candidateVotes = new();
    
    private Notification _notification = new();

    protected override Task OnInitializedAsync()
    {
        // Initialise form selection fields
        _nations = new List<string>()
        {
            "United Kingdom"
        };
        
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
            _administeredElections = _userService.GetUsersAdministeredElections(_userId);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Validates election form & decides whether to call <see cref="CreateElection"/> or <see cref="SaveElectionChanges"/>
    /// </summary>
    private void ElectionFormButtonPressed()
    {
        if (String.IsNullOrEmpty(_formModel.Election.Name) ||
            String.IsNullOrEmpty(_formModel.Election.Nation) ||
            _formModel.Election.Type == null)
        {
            _notification.Header = "Invalid Input!";
            _notification.Message = "All fields must have a value!";
            _notification.Type = NotificationType.Failure;
            _notification.Visible = true;
            return;
        }

        if (_formModel.Election.EndTime <= _formModel.Election.StartTime)
        {
            _notification.Header = "Invalid Input!";
            _notification.Message = "The end time must be after the start time!";
            _notification.Type = NotificationType.Failure;
            _notification.Visible = true;
            return;
        }
        
        if (_formModel.ElectionInvites.Count == 0)
        {
            _notification.Header = "Invalid Input!";
            _notification.Message = "Please invite at least one candidate!";
            _notification.Type = NotificationType.Failure;
            _notification.Visible = true;
            return;
        }

        _notification.Visible = false;
        
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
    
    /// <summary>
    /// Calls <see cref="_electionService"/> to create election using details in <see cref="_formModel"/>
    /// If successful, it goes back to the election panel & refreshes the list of administered elections 
    /// </summary>
    private void CreateElection()
    {
        if (_electionService.CreateElection(_formModel.Election, _userId, _formModel.ElectionInvites))
        {
            _administeredElections = _userService.GetUsersAdministeredElections(_userId);
            OpenElectionListPanel();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Calls <see cref="_electionService"/> to delete election
    /// </summary>
    /// <param name="id"></param>
    private void DeleteElection(Guid id)
    {
        _electionService.DeleteElection(id);
        _administeredElections = _userService.GetUsersAdministeredElections(_userId);
        StateHasChanged();
    }

    /// <summary>
    /// Opens election details panel & retrieves required data for election with <paramref name="electionId"/>
    /// </summary>
    /// <param name="electionId"></param>
    private void ViewElectionDetails(Guid electionId)
    {
        _election = _electionService.GetElectionById(electionId);
        _candidates = _electionService.GetCandidates(electionId);
        _candidateVotes = _voteService.CountElectionVotes(electionId);
        
        _adminPanel = AdminPanel.ViewElection;
    }

    /// <summary>
    /// Calls <see cref="_electionService"/> to update the election using the details in <see cref="_formModel"/>
    /// </summary>
    private void SaveElectionChanges()
    {
        if (_electionService.UpdateElection(_formModel.Election, _formModel.InviteChanges))
        {
            _adminPanel = AdminPanel.ElectionList;
            _administeredElections = _userService.GetUsersAdministeredElections(_userId);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Opens create election panel & sets default form values
    /// </summary>
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
    
    /// <summary>
    /// Opens edit election panel & sets default form values
    /// </summary>
    private void OpenEditElectionPanel(Guid electionId)
    {
        _adminPanel = AdminPanel.CreateElection;

        var election = _electionService.GetElectionById(electionId);
        var electionInvites = _electionService.GetElectionCandidateInvites(electionId);
        
        _formModel = new ElectionFormModel()
        {
            Title = $"Edit {election.Name}",
            ButtonText = "Save Changes",
            Election = election,
            ElectionInvites = electionInvites
        };
    }

    /// <summary>
    /// Opens election list panel
    /// </summary>
    private void OpenElectionListPanel()
    {
        _adminPanel = AdminPanel.ElectionList;
    }

    /// <summary>
    /// Formats invite from <see cref="_formModel"/> & tracks the change in <see cref="_formModel"/>
    /// </summary>
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
    
    /// <summary>
    /// If change is in <see cref="_formModel"/> invite changes, then remove it from there
    /// Otherwise, create a new change that indicates this candidate invite needs to be removed from the db once saved.
    /// </summary>
    /// <param name="electionId"></param>
    /// <param name="userEmail"></param>
    /// <param name="status"></param>
    private void RemoveCandidateInvite(Guid electionId, string userEmail, ElectionInviteStatus status)
    {
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