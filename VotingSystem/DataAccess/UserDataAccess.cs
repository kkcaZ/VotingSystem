using System.Data.SqlClient;
using VotingSystem.Data;
using VotingSystem.DataAccess.Abstraction;
using Dapper;
using VotingSystem.Data.Enum;

namespace VotingSystem.DataAccess;

public class UserDataAccess : IUserDataAccess
{
    private readonly string _connectionString = @"Server=127.0.0.1,1433;Database=VotingDb;User ID=SA;Password=Password123!";
    
    /// <inheritdoc/>
    public User? GetById(Guid id)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = "SELECT * FROM [User] WHERE [Id] = @Id";
            List<User> users = conn.Query<User>(sql, new { Id = id }).ToList();

            if (users.Count == 0)
                return null;
            
            // Close connection
            conn.Close();
            return users[0];
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    /// <inheritdoc />
    public User? GetByEmail(string email)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = "SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress";
            List<User> users = conn.Query<User>(sql, new { EmailAddress = email }).ToList();

            if (users.Count == 0)
                return null;
            
            // Close connection
            conn.Close();
            return users.First();
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    /// <inheritdoc/>
    public List<Election> GetUsersAdministeredElections(Guid id)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                SELECT * FROM [Election] e
                INNER JOIN ElectionAdmin ea
                ON ea.ElectionId = e.Id
                WHERE ea.UserId = @UserId
            ";
            List<Election> elections = conn.Query<Election>(sql, new { UserId = id }).ToList();

            // Close connection
            conn.Close();
            return elections;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    
    /// <inheritdoc/>
    public List<Election> GetUsersElectionInvites(string email)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                SELECT * FROM [Election] e
                INNER JOIN ElectionInvite ei
                ON ei.ElectionId = e.Id
                WHERE ei.UserEmail = @Email AND ei.StatusId = 0
            ";
            List<Election> elections = conn.Query<Election>(sql, new { Email = email }).ToList();

            // Close connection
            conn.Close();
            return elections;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    
    /// <inheritdoc/>
    public User? GetByNationalIdentifier(string nationalIdentifier)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = "SELECT * FROM [User] WHERE [NationalIdentifier] = @NationalIdentifier";
            List<User> users = conn.Query<User>(sql, new { NationalIdentifier = nationalIdentifier }).ToList();

            if (users.Count == 0)
                return null;
            
            // Close connection
            conn.Close();
            return users.First();
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <inheritdoc/>
    public int Add(User user)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO [User] (Id, FirstNames, Surname, PhoneNumber, EmailAddress, PostCode, Address, NationalIdentifier, Nationality, Password) 
                VALUES (@Id, @FirstNames, @Surname, @PhoneNumber, @EmailAddress, @PostCode, @Address, @NationalIdentifier, @Nationality, @Password)
            ";
            
            var rowsAffected = conn.Execute(sql, user);
            
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    /// <inheritdoc/>
    public int Delete(Guid id)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [User]
                WHERE [Id] = @Id
            ";
            
            var rowsAffected = conn.Execute(sql, new { Id = id });

            // Close connection
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    /// <inheritdoc/>
    public int Update(User user)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                UPDATE [User] 
                SET 
                    [Id] = @Id, 
                    [FirstNames] = @FirstNames, 
                    [Surname] = @Surname, 
                    [PhoneNumber] = @PhoneNumber, 
                    [EmailAddress] = @EmailAddress, 
                    [PostCode] = @PostCode, 
                    [Address] = @Address, 
                    [NationalIdentifier] = @NationalIdentifier, 
                    [Nationality] = @Nationality, 
                    [Password] = @Password
                WHERE [Id] = @Id
            ";
            
            var rowsAffected = conn.Execute(sql, user);

            // Close connection
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int UpdateElectionInviteStatus(Guid electionId, string userEmail, ElectionInviteStatus status)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                UPDATE [ElectionInvite] 
                SET 
                    [StatusId] = @StatusId
                WHERE [ElectionId] = @ElectionId AND [UserEmail] = @UserEmail
            ";
            
            var rowsAffected = conn.Execute(sql, new { StatusId = status, ElectionId = electionId, UserEmail = userEmail });

            // Close connection
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }
}