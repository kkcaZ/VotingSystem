using VotingSystem.Data;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Abstraction;

namespace VotingSystem.IntegrationTests;

public class UserDataAccessTests
{
    private readonly IUserDataAccess _userDataAccess;
    
    private readonly User _testUser = new User()
    {
        Id = Guid.Parse("00000000-1111-2222-3333-444444444444"),
        FirstNames = "Test",
        Surname = "User",
        PostCode = "LS12AB",
        Address = "Dock Street",
        EmailAddress = "test@email.co.uk",
        PhoneNumber = "07379286769",
        NationalIdentifier = "AA123456C",
        Nationality = "United Kingdom",
        Password = "password"
    };

    public UserDataAccessTests()
    {
        // _userDataAccess = new UserDataAccess();
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void GetById()
    {
        var user = _userDataAccess.GetById(Guid.Parse("CF050B61-2CE9-4531-A37A-B80FDEE383C0"));
        
        Assert.That(user?.FirstNames, Is.EqualTo("Zack"));
    }

    [Test]
    public void Add()
    {
        // Arrange
        User expectedUser = _testUser;
        
        // Act
        int rowsEffected = _userDataAccess.Add(expectedUser);
        
        // Assert
        // Relies on GetById test passing
        var actualUser = _userDataAccess.GetById(Guid.Parse("00000000-1111-2222-3333-444444444444"));
        
        Assert.That(rowsEffected, Is.EqualTo(1));
        
        Assert.NotNull(actualUser);
        Assert.That(actualUser?.Id, Is.EqualTo(expectedUser.Id));
        Assert.That(actualUser?.FirstNames, Is.EqualTo(expectedUser.FirstNames));
        Assert.That(actualUser?.Surname, Is.EqualTo(expectedUser.Surname));
        Assert.That(actualUser?.PostCode, Is.EqualTo(expectedUser.PostCode));
        Assert.That(actualUser?.Address, Is.EqualTo(expectedUser.Address));
        Assert.That(actualUser?.EmailAddress, Is.EqualTo(expectedUser.EmailAddress));
        Assert.That(actualUser?.PhoneNumber, Is.EqualTo(expectedUser.PhoneNumber));
        Assert.That(actualUser?.NationalIdentifier, Is.EqualTo(expectedUser.NationalIdentifier));
        Assert.That(actualUser?.Nationality, Is.EqualTo(expectedUser.Nationality));
        Assert.That(actualUser?.Password, Is.EqualTo(expectedUser.Password));
    }

    [Test]
    public void Update()
    {
        // Arrange
        _userDataAccess.Add(_testUser);
        
        User expectedUser = _testUser;
        expectedUser.FirstNames = "Jeff";
        
        // Act
        int rowsAffected = _userDataAccess.Update(expectedUser);
        
        // Assert
        var actualUser = _userDataAccess.GetById(Guid.Parse("00000000-1111-2222-3333-444444444444"));
        
        Assert.That(rowsAffected, Is.EqualTo(1));
        Assert.That(actualUser?.FirstNames, Is.EqualTo(expectedUser.FirstNames));
    }

    [TearDown]
    public void TearDown()
    {
        _userDataAccess.Delete(_testUser.Id);
    }
}