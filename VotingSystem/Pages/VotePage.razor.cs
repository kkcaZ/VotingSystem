using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using VotingSystem.Data;
using VotingSystem.Services.Abstraction;
using VotingSystem.Shared;

namespace VotingSystem.Pages;

public partial class VotePage : AuthenticatedPage
{
    [Inject] private IElectionService _electionService { get; set; }
    [Inject] private IVoteService _voteService { get; set; }
    [Inject] private NavigationManager _navManager { get; set; }
    
    [Parameter] public string ElectionId { get; set; } = string.Empty;

    private Guid _electionId = Guid.Empty;
    private Election _election = new();
    private List<User> _candidates = new();
    private bool _userVoted = false;

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(ElectionId) || !Guid.TryParse(ElectionId, out _electionId))
            _navManager.NavigateTo("/");
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;
        
        _election = _electionService.GetElectionById(_electionId);
        _candidates = _electionService.GetCandidates(_electionId);
        _userVoted = _voteService.HasUserVoted(_electionId, _userId);
        
        StateHasChanged();
    }

    /// <summary>
    /// Calls server to submit vote for user
    /// </summary>
    /// <param name="candidateId"></param>
    private void SubmitVote(Guid candidateId)
    {
        if (_voteService.SubmitVote(_userId, candidateId, _electionId))
        {
            _userVoted = true;
            StateHasChanged();
        }
    }
}