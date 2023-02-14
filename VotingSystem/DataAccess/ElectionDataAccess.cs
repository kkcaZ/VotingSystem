using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Connections.Features;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.DataAccess.Abstraction;

namespace VotingSystem.DataAccess;

public class ElectionDataAccess : IElectionDataAccess
{
    private readonly string _connectionString = @"Server=127.0.0.1,1433;Database=VotingDb;User ID=SA;Password=Password123!";
    
    public Election? GetById(Guid id)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = "SELECT * FROM [Election] WHERE [Id] = @Id";
            List<Election> elections = conn.Query<Election>(sql, new { Id = id }).ToList();

            if (elections.Count == 0)
                return null;
            
            // Close connection
            conn.Close();
            return elections[0];
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public List<Election> GetByNation(string nation)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = "SELECT * FROM [Election] WHERE [Nation] = @Nation";
            List<Election> elections = conn.Query<Election>(sql, new { Nation = nation }).ToList();

            // Close connection
            conn.Close();
            return elections;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return new List<Election>();
        }
    }

    public List<ElectionInviteModel> GetElectionInvites(Guid electionId)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"SELECT * FROM [ElectionInvite] WHERE [ElectionId] = @ElectionId";
            List<ElectionInviteModel> elections = conn.Query<ElectionInviteModel>(sql, new { ElectionId = electionId }).ToList();

            if (elections.Count == 0)
                return new List<ElectionInviteModel>();
            
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

    public List<User> GetCandidates(Guid electionId)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                SELECT * FROM [ElectionCandidate] ec
                INNER JOIN [User] u
                ON u.[Id] = ec.[UserId]
                WHERE ec.[ElectionId] = @ElectionId
            ";
            List<User> candidates = conn.Query<User>(sql, new { ElectionId = electionId }).ToList();

            // Close connection
            conn.Close();
            return candidates;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public int Add(Election election)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO [Election] (Id, [Name], Nation, [Type], StartTime, EndTime) 
                VALUES (@Id, @Name, @Nation, @Type, @StartTime, @EndTime)
            ";
            
            var rowsAffected = conn.Execute(sql, election);
            
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public int AddElectionAdmin(Guid userId, Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO [ElectionAdmin] ([UserId], [ElectionId]) 
                VALUES (@UserId, @ElectionId)
            ";
            
            var rowsAffected = conn.Execute(sql, new { UserId = userId, ElectionId = electionId });
            
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public int AddElectionInviteEmail(Guid electionId, string email, ElectionInviteStatus status = ElectionInviteStatus.Pending)
    {
        try
        {
            var rowsAffected = AddElectionInviteEmail(new ElectionInviteModel()
            {
                ElectionId = electionId,
                UserEmail = email,
                StatusId = status
            });

            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }
    
    public int AddElectionInviteEmail(ElectionInviteModel electionInvite)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO [ElectionInvite] ([ElectionId], [UserEmail], [StatusId]) 
                VALUES (@ElectionId, @UserEmail, @StatusId)
            ";

            var rowsAffected = conn.Execute(sql, new { ElectionId = electionInvite.ElectionId, UserEmail = electionInvite.UserEmail, StatusId = electionInvite.StatusId });
            
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public int AddCandidate(Guid userId, Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO [ElectionCandidate] ([UserId], [ElectionId]) 
                VALUES (@UserId, @ElectionId)
            ";

            var rowsAffected = conn.Execute(sql, new
            {
                UserId = userId,
                ElectionId = electionId,
                Votes = 0
            });
            
            conn.Close();
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public int Delete(Guid id)
    {
        try
        {
            DeleteAllCandidates(id);
            DeleteAllAdmins(id);
            DeleteAllElectionInvites(id);
            
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [Election]
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
    
    public int DeleteAllCandidates(Guid electionId)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [ElectionCandidate]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });

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
    
    public int DeleteAllAdmins(Guid electionId)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [ElectionAdmin]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });

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

    public int DeleteAllElectionInvites(Guid electionId)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [ElectionInvite]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });

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
    
    public int DeleteCandidate(Guid userId, Guid electionId)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [ElectionCandidate]
                WHERE [ElectionId] = @ElectionId AND [UserId] = @UserId
            ";
            
            var rowsAffected = conn.Execute(sql, new
            {
                ElectionId = electionId,
                UserId = userId
            });

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
    public int DeleteUsersElectionInvite(Guid electionId, string userEmail)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                DELETE FROM [ElectionInvite]
                WHERE [ElectionId] = @ElectionId AND [UserEmail] = @UserEmail
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId, UserEmail = userEmail });

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

    public int Update(Election election)
    {
        try
        {
            // Open connection
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Execute query
            string sql = @"
                UPDATE [Election] 
                SET 
                    [Id] = @Id, 
                    [Name] = @Name, 
                    [Nation] = @Nation, 
                    [Type] = @Type, 
                    [StartTime] = @StartTime,
                    [EndTime] = @EndTime
                WHERE [Id] = @Id
            ";
            
            var rowsAffected = conn.Execute(sql, election);

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