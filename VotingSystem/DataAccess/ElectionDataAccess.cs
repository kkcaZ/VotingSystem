using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Connections.Features;
using VotingSystem.Data;
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
            string sql = "SELECT * FROM [User] WHERE [Id] = @Id";
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

    /// <summary>
    /// Returns all elections that are administered by given <paramref name="id"/>
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns>List of elections that are owned by the user</returns>
    public List<Election> GetUsersElections(Guid id)
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

    public int AddElectionInviteEmail(Guid electionId, string email)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO [ElectionInvite] ([ElectionId], [UserEmail]) 
                VALUES (@ElectionId, @UserEmail)
            ";

            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId, UserEmail = email });
            
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
            // DeleteAllCandidates(id);
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
                    [StartDate] = @StartDate,
                    [EndDate] = @EndDate
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