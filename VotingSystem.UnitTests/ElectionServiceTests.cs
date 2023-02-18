using Moq;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.UnitTests;

public class ElectionServiceTests
{
    private readonly Mock<IElectionDataAccess> _electionDataAccess;
    private readonly Mock<IUserService> _userService;
    private readonly IElectionService _electionService;

    private readonly Election _testElection;
    private readonly User _testUser;
    private readonly List<ElectionInviteModel> _testInvites;
    
    public ElectionServiceTests()
    {
        _electionDataAccess = new Mock<IElectionDataAccess>();
        _userService = new Mock<IUserService>();
        _electionService = new ElectionService(_electionDataAccess.Object, _userService.Object);

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
        
        _testInvites = new List<ElectionInviteModel>()
        {
            new ElectionInviteModel()
            {
                ElectionId = _testElection.Id,
                StatusId = ElectionInviteStatus.Pending,
                UserEmail = "test@gmail.com"
            }
        };
    }

    [Test]
    public void GetElectionById_ReturnsElection()
    {
        // Arrange
        var expectedElection = _testElection;
        _electionDataAccess.Setup(x => x.GetById(_testElection.Id)).Returns(_testElection);

        // Act
        var actualElection = _electionService.GetElectionById(expectedElection.Id);

        // Assert
        Assert.That(actualElection.Id, Is.EqualTo(expectedElection.Id));
        Assert.That(actualElection.Name, Is.EqualTo(expectedElection.Name));
        Assert.That(actualElection.StartTime, Is.EqualTo(expectedElection.StartTime));
        Assert.That(actualElection.EndTime, Is.EqualTo(expectedElection.EndTime));
        Assert.That(actualElection.Type, Is.EqualTo(expectedElection.Type));
        Assert.That(actualElection.Nation, Is.EqualTo(expectedElection.Nation));
    } 
    
    [Test]
    public void GetElectionsByNation_ReturnsElectionList()
    {
        // Arrange
        var expectedElections = new List<Election>()
        {
            _testElection
        };
        
        _electionDataAccess.Setup(x => x.GetByNation(_testElection.Nation)).Returns(expectedElections);

        // Act
        var actualElections = _electionService.GetElectionsByNation(_testElection.Nation);

        // Assert
        Assert.That(actualElections.Count, Is.EqualTo(expectedElections.Count));
        Assert.That(actualElections.First().Id, Is.EqualTo(expectedElections.First().Id));
        Assert.That(actualElections.First().Name, Is.EqualTo(expectedElections.First().Name));
        Assert.That(actualElections.First().StartTime, Is.EqualTo(expectedElections.First().StartTime));
        Assert.That(actualElections.First().EndTime, Is.EqualTo(expectedElections.First().EndTime));
        Assert.That(actualElections.First().Type, Is.EqualTo(expectedElections.First().Type));
        Assert.That(actualElections.First().Nation, Is.EqualTo(expectedElections.First().Nation));
    } 
    
    [Test]
    public void GetElectionCandidateInvites_ReturnsElectionList()
    {
        // Arrange
        var expectedInvites = _testInvites;

        _electionDataAccess.Setup(x => x.GetElectionInvites(_testElection.Id)).Returns(expectedInvites);

        // Act
        var actualInvites = _electionService.GetElectionCandidateInvites(_testElection.Id);

        // Assert
        Assert.That(actualInvites.Count, Is.EqualTo(expectedInvites.Count));
        Assert.That(actualInvites.First().ElectionId, Is.EqualTo(expectedInvites.First().ElectionId));
        Assert.That(actualInvites.First().StatusId, Is.EqualTo(expectedInvites.First().StatusId));
        Assert.That(actualInvites.First().UserEmail, Is.EqualTo(expectedInvites.First().UserEmail));
    } 
    
    [Test]
    public void GetCandidates_ReturnsUserList()
    {
        // Arrange
        var expectedCandidates = new List<User>()
        {
            _testUser
        };

        _electionDataAccess.Setup(x => x.GetCandidates(_testElection.Id)).Returns(expectedCandidates);

        // Act
        var actualCandidates = _electionService.GetCandidates(_testElection.Id);

        // Assert
        Assert.That(actualCandidates.Count, Is.EqualTo(expectedCandidates.Count));
        Assert.That(actualCandidates.First().Id, Is.EqualTo(expectedCandidates.First().Id));
        Assert.That(actualCandidates.First().FirstNames, Is.EqualTo(expectedCandidates.First().FirstNames));
        Assert.That(actualCandidates.First().Surname, Is.EqualTo(expectedCandidates.First().Surname));
    } 
    
    [Test]
    public void CreateElection_WithSuccessfulCreation_ReturnsTrue()
    {
        // Arrange
        _electionDataAccess.Setup(x => x.Add(_testElection)).Returns(1);
        _electionDataAccess.Setup(x => x.AddElectionAdmin(_testUser.Id, It.IsAny<Guid>())).Returns(1);
        _electionDataAccess.Setup(x => x.AddElectionInviteEmail(It.IsAny<Guid>(), _testInvites.First().UserEmail, ElectionInviteStatus.Pending)).Returns(1);
        
        // Act
        var actualStatus = _electionService.CreateElection(_testElection, _testUser.Id, _testInvites);

        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    } 
    
    [Test]
    public void DeleteElection_ReturnsFalse()
    {
        // Arrange
        _electionDataAccess.Setup(x => x.Delete(_testElection.Id)).Returns(1);
        
        // Act
        var actualStatus = _electionService.DeleteElection(_testElection.Id);

        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }

    [Test]
    public void UpdateElection_WithNewInvite_ReturnsTrue()
    {
        // Arrange
        var inviteUpdates = new List<ElectionInviteUpdate>()
        {
            new ElectionInviteUpdate
            {
                ElectionInvite = _testInvites.First(),
                ChangeType = InviteChangeType.Created
            }
        };
        
        _electionDataAccess.Setup(x => x.Update(_testElection)).Returns(1);
        _electionDataAccess.Setup(x => x.AddElectionInviteEmail(_testInvites.First())).Returns(1);
        
        // Act
        var actualStatus = _electionService.UpdateElection(_testElection, inviteUpdates);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }
    
    [Test]
    public void UpdateElection_WithDeletedInvite_ReturnsTrue()
    {
        // Arrange
        var inviteUpdates = new List<ElectionInviteUpdate>()
        {
            new ElectionInviteUpdate
            {
                ElectionInvite = _testInvites.First(),
                ChangeType = InviteChangeType.Deleted
            }
        };
        
        _electionDataAccess.Setup(x => x.Update(_testElection)).Returns(1);
        _electionDataAccess.Setup(x => x.DeleteUsersElectionInvite(_testInvites.First().ElectionId, _testInvites.First().UserEmail)).Returns(1);
        
        // Act
        var actualStatus = _electionService.UpdateElection(_testElection, inviteUpdates);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }

    [Test]
    public void AddCandidate_ReturnsTrue()
    {
        // Arrange
        _electionDataAccess.Setup(x => x.AddCandidate(_testUser.Id, _testElection.Id)).Returns(1);
        
        // Act
        var actualStatus = _electionService.AddCandidate(_testUser.Id, _testElection.Id);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }
    
    [Test]
    public void DeleteCandidate_ReturnsTrue()
    {
        // Arrange
        _electionDataAccess.Setup(x => x.DeleteCandidate(_testUser.Id, _testElection.Id)).Returns(1);
        
        // Act
        var actualStatus = _electionService.DeleteCandidate(_testElection.Id, _testUser.Id);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
    }
}