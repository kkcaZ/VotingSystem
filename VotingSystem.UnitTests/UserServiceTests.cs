using Moq;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services;
using VotingSystem.Services.Abstraction;

namespace VotingSystem.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserDataAccess> _userDataAccess;
    private readonly IUserService _userService;
    
    private readonly User _testUser;
    private readonly List<Election> _electionList;

    public UserServiceTests()
    {
        _userDataAccess = new Mock<IUserDataAccess>();
        _userService = new UserService(_userDataAccess.Object);
        
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
        
        _electionList = new List<Election>()
        {
            new Election()
            {
                Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                Name = "Test Election",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                Nation = "United Kingdom",
                Type = ElectionType.Preferential
            }
        };
    }

    [Test]
    public void GetUserById_ReturnsUser()
    {
        // Arrange
        var expectedUser = _testUser;
        _userDataAccess.Setup(x => x.GetById(expectedUser.Id)).Returns(expectedUser);
        
        // Act
        var actualUser = _userService.GetUserById(_testUser.Id);
        
        // Assert
        Assert.That(actualUser.Id, Is.EqualTo(expectedUser.Id));
        Assert.That(actualUser.FirstNames, Is.EqualTo(expectedUser.FirstNames));
        Assert.That(actualUser.Surname, Is.EqualTo(expectedUser.Surname));
        Assert.That(actualUser.Address, Is.EqualTo(expectedUser.Address));
        Assert.That(actualUser.PostCode, Is.EqualTo(expectedUser.PostCode));
        Assert.That(actualUser.EmailAddress, Is.EqualTo(expectedUser.EmailAddress));
        Assert.That(actualUser.PhoneNumber, Is.EqualTo(expectedUser.PhoneNumber));
        Assert.That(actualUser.NationalIdentifier, Is.EqualTo(expectedUser.NationalIdentifier));
        Assert.That(actualUser.Nationality, Is.EqualTo(expectedUser.Nationality));
        Assert.That(actualUser.Password, Is.EqualTo(expectedUser.Password));
    }
    
    [Test]
    public void GetUserByEmail_ReturnsUser()
    {
        // Arrange
        var expectedUser = _testUser;
        _userDataAccess.Setup(x => x.GetByEmail(expectedUser.EmailAddress)).Returns(expectedUser);
        
        // Act
        var actualUser = _userService.GetUserByEmail(_testUser.EmailAddress);
        
        // Assert
        Assert.That(actualUser.Id, Is.EqualTo(expectedUser.Id));
        Assert.That(actualUser.FirstNames, Is.EqualTo(expectedUser.FirstNames));
        Assert.That(actualUser.Surname, Is.EqualTo(expectedUser.Surname));
        Assert.That(actualUser.Address, Is.EqualTo(expectedUser.Address));
        Assert.That(actualUser.PostCode, Is.EqualTo(expectedUser.PostCode));
        Assert.That(actualUser.EmailAddress, Is.EqualTo(expectedUser.EmailAddress));
        Assert.That(actualUser.PhoneNumber, Is.EqualTo(expectedUser.PhoneNumber));
        Assert.That(actualUser.NationalIdentifier, Is.EqualTo(expectedUser.NationalIdentifier));
        Assert.That(actualUser.Nationality, Is.EqualTo(expectedUser.Nationality));
        Assert.That(actualUser.Password, Is.EqualTo(expectedUser.Password));
    }
    
    [Test]
    public void GetUsersAdministeredElections_ReturnsElectionList()
    {
        // Arrange
        var expectedAdministeredElections = _electionList;
        _userDataAccess.Setup(x => x.GetUsersAdministeredElections(_testUser.Id)).Returns(expectedAdministeredElections);

        // Act
        var actualAdministeredElections = _userService.GetUsersAdministeredElections(_testUser.Id);
        
        // Assert
        Assert.That(actualAdministeredElections.Count, Is.EqualTo(expectedAdministeredElections.Count));
        Assert.That(actualAdministeredElections.First().Id, Is.EqualTo(expectedAdministeredElections.First().Id));
        Assert.That(actualAdministeredElections.First().Name, Is.EqualTo(expectedAdministeredElections.First().Name));
        Assert.That(actualAdministeredElections.First().StartTime, Is.EqualTo(expectedAdministeredElections.First().StartTime));
        Assert.That(actualAdministeredElections.First().EndTime, Is.EqualTo(expectedAdministeredElections.First().EndTime));
        Assert.That(actualAdministeredElections.First().Nation, Is.EqualTo(expectedAdministeredElections.First().Nation));
        Assert.That(actualAdministeredElections.First().Type, Is.EqualTo(expectedAdministeredElections.First().Type));
    }
    
    [Test]
    public void GetUsersElectionInvites_ReturnsElectionList()
    {
        // Arrange
        var expectedAdministeredElections = _electionList;
        _userDataAccess.Setup(x => x.GetUsersElectionInvites(_testUser.EmailAddress)).Returns(expectedAdministeredElections);
        _userDataAccess.Setup(x => x.GetById(_testUser.Id)).Returns(_testUser);
        
        // Act
        var actualAdministeredElections = _userService.GetUsersElectionInvites(_testUser.Id);
        
        // Assert
        Assert.That(actualAdministeredElections.Count, Is.EqualTo(expectedAdministeredElections.Count));
        Assert.That(actualAdministeredElections.First().Id, Is.EqualTo(expectedAdministeredElections.First().Id));
        Assert.That(actualAdministeredElections.First().Name, Is.EqualTo(expectedAdministeredElections.First().Name));
        Assert.That(actualAdministeredElections.First().StartTime, Is.EqualTo(expectedAdministeredElections.First().StartTime));
        Assert.That(actualAdministeredElections.First().EndTime, Is.EqualTo(expectedAdministeredElections.First().EndTime));
        Assert.That(actualAdministeredElections.First().Nation, Is.EqualTo(expectedAdministeredElections.First().Nation));
        Assert.That(actualAdministeredElections.First().Type, Is.EqualTo(expectedAdministeredElections.First().Type));
    }
    
    [Test]
    public void UpdateElectionInvite_WithRowsEffected_ReturnsTrue()
    {
        // Arrange
        var electionId = _electionList.First().Id;
        _userDataAccess.Setup(x => x.GetById(_testUser.Id)).Returns(_testUser);
        _userDataAccess.Setup(x =>
                x.UpdateElectionInviteStatus(electionId, _testUser.EmailAddress, ElectionInviteStatus.Accepted))
            .Returns(1);
        
        // Act
        var actualUpdateStatus = _userService.UpdateElectionInvite(_testUser.Id, electionId, ElectionInviteStatus.Accepted);
        
        // Assert
        Assert.That(actualUpdateStatus, Is.EqualTo(true));
    }
    
    [Test]
    public void UpdateElectionInvite_WithNoRowsEffected_ReturnsFalse()
    {
        // Arrange
        var electionId = _electionList.First().Id;
        _userDataAccess.Setup(x => x.GetById(_testUser.Id)).Returns(_testUser);
        _userDataAccess.Setup(x =>
            x.UpdateElectionInviteStatus(electionId, _testUser.EmailAddress, ElectionInviteStatus.Accepted))
            .Returns(0);
        
        // Act
        var actualUpdateStatus = _userService.UpdateElectionInvite(_testUser.Id, electionId, ElectionInviteStatus.Accepted);
        
        // Assert
        Assert.That(actualUpdateStatus, Is.EqualTo(false));
    }

    [Test]
    public void AddUser_WithNoConflicts_ReturnsTrue()
    {
        // Arrange
        User? nullExpectedUser = null;
        _userDataAccess.Setup(x => x.GetByEmail(_testUser.EmailAddress)).Returns(nullExpectedUser);
        _userDataAccess.Setup(x => x.GetByNationalIdentifier(_testUser.NationalIdentifier)).Returns(nullExpectedUser);

        var user = _testUser;
        
        // Act
        var actualStatus = _userService.AddUser(ref user);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
        Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
    }
    
    [Test]
    public void AddUser_WithEmailConflict_ReturnsFalse()
    {
        // Arrange
        User? nullExpectedUser = null;
        _userDataAccess.Setup(x => x.GetByEmail(_testUser.EmailAddress)).Returns(_testUser);
        _userDataAccess.Setup(x => x.GetByNationalIdentifier(_testUser.NationalIdentifier)).Returns(nullExpectedUser);

        var user = _testUser;
        
        // Act
        var actualStatus = _userService.AddUser(ref user);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(false));
    }
    
    [Test]
    public void AddUser_WithNationalIdentifierConflict_ReturnsFalse()
    {
        // Arrange
        User? nullExpectedUser = null;
        _userDataAccess.Setup(x => x.GetByEmail(_testUser.EmailAddress)).Returns(nullExpectedUser);
        _userDataAccess.Setup(x => x.GetByNationalIdentifier(_testUser.NationalIdentifier)).Returns(_testUser);

        var user = _testUser;
        
        // Act
        var actualStatus = _userService.AddUser(ref user);
        
        // Assert
        Assert.That(actualStatus, Is.EqualTo(false));
    }

    [Test]
    public void Authenticate_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var expectedUser = _testUser;
        _userDataAccess.Setup(x => x.GetByNationalIdentifier(expectedUser.NationalIdentifier)).Returns(expectedUser);
        
        // Act
        User actualUser;
        var actualStatus =
            _userService.Authenticate(expectedUser.NationalIdentifier, expectedUser.Password, out actualUser);

        // Assert
        Assert.That(actualStatus, Is.EqualTo(true));
        Assert.That(actualUser.Id, Is.Not.EqualTo(Guid.Empty));
    }
    
    [Test]
    public void Authenticate_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var expectedUser = _testUser;
        _userDataAccess.Setup(x => x.GetByNationalIdentifier(expectedUser.NationalIdentifier)).Returns(expectedUser);
        
        // Act
        User actualUser;
        var actualStatus =
            _userService.Authenticate(expectedUser.NationalIdentifier, "incorrectPassword", out actualUser);

        // Assert
        Assert.That(actualStatus, Is.EqualTo(false));
        Assert.That(actualUser, Is.EqualTo(null));
    }
    
    [Test]
    public void Authenticate_WithIncorrectNationalIdentifier_ReturnsFalse()
    {
        // Arrange
        User? nullExpectedUser = null;
        _userDataAccess.Setup(x => x.GetByNationalIdentifier(_testUser.NationalIdentifier)).Returns(nullExpectedUser);
        
        // Act
        User actualUser;
        var actualStatus =
            _userService.Authenticate(_testUser.NationalIdentifier, _testUser.Password, out actualUser);

        // Assert
        Assert.That(actualStatus, Is.EqualTo(false));
        Assert.That(actualUser, Is.EqualTo(null));
    }
}