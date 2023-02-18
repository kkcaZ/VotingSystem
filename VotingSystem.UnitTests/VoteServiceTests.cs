using Moq;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.UnitTests;

public class VoteServiceTests
{
    private readonly Mock<IVoteDataAccess> _voteDataAccess;
    private readonly Mock<IElectionService> _electionService;
    private readonly IVoteService _voteService;

    private readonly Vote _testVote;
    private readonly Election _testElection;
    private readonly User _testUser;

    public VoteServiceTests()
    {
        _voteDataAccess = new Mock<IVoteDataAccess>();
        _electionService = new Mock<IElectionService>();
        _voteService = new VoteService(_voteDataAccess.Object, _electionService.Object);

        _testElection = new Election()
        {
            Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
            Name = "Test Election",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Nation = "United Kingdom",
            Type = ElectionType.Preferential
        };
        
        _testUser = new User()
        {
            Id = Guid.Parse("00000000-1111-2222-3333-444444444444"),
            FirstNames = "Test",
            Surname = "User",
            Address = "123 Test Lane",
            PostCode = "LS41FU",
            EmailAddress = "test@gmail.com",
            PhoneNumber = "07286379367",
            NationalIdentifier = "AA123456C",
            Nationality = "United Kingdom",
            Password = "password"
        };
        
        _testVote = new Vote
        {
            VoterId = _testUser.Id,
            CandidateId = _testUser.Id,
            ElectionId = _testElection.Id,
            Timestamp = DateTime.Now
        };
    }

    [Test]
    public void CountElectionVotes_ReturnsTupleList()
    {
        // Arrange
        List<(Guid candidateId, int voteCount)> expectedVoteCounts = new()
        {
            (_testUser.Id, 5)
        };

        List<User> expectedCandidates = new()
        {
            _testUser
        };

        _electionService.Setup(x => x.GetCandidates(_testElection.Id)).Returns(expectedCandidates);
        _voteDataAccess.Setup(x => x.GetCandidateVoteCount(_testElection.Id, _testUser.Id)).Returns(5);
        
        // Act
        var actualVoteCounts = _voteService.CountElectionVotes(_testElection.Id);
        
        // Assert
        Assert.That(actualVoteCounts, Is.EqualTo(expectedVoteCounts));
    }

    [Test]
    public void SubmitVote_ReturnsTrue()
    {
        // Arrange
        _voteDataAccess.Setup(x => x.Add(_testVote)).Returns(1);
        
        // Act
        var actualStatus = _voteService.SubmitVote(_testVote);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }

    [Test]
    public void HasUserVoted_WithVoteRecord_ReturnsTrue()
    {
        // Arrange
        _voteDataAccess.Setup(x => x.Get(_testElection.Id, _testUser.Id)).Returns(_testVote);
        
        // Act
        var actualStatus = _voteService.HasUserVoted(_testElection.Id, _testUser.Id);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }
    
    [Test]
    public void HasUserVoted_WithNoVoteRecord_ReturnsTrue()
    {
        // Arrange
        Vote? nullVote = null;
        _voteDataAccess.Setup(x => x.Get(_testElection.Id, _testUser.Id)).Returns(nullVote);
        
        // Act
        var actualStatus = _voteService.HasUserVoted(_testElection.Id, _testUser.Id);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(false));
    }
}