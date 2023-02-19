using System.Data.SqlClient;
using Dapper;
using VotingSystem.Data;
using VotingSystem.Data.Enum;
using VotingSystem.DataAccess.Abstraction;

namespace VotingSystem.DataAccess;

public class ElectionDataAccess : IElectionDataAccess
{
    private readonly ILogger<ElectionDataAccess> _logger;

    private readonly string _connectionString;

    public ElectionDataAccess(ILogger<ElectionDataAccess> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["ConnectionString"];
    }
    
    /// <inheritdoc/>
    public Election? GetById(Guid id)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = "SELECT * FROM [Election] WHERE [Id] = @Id";
            List<Election> elections = conn.Query<Election>(sql, new { Id = id }).ToList();

            if (elections.Count == 0)
                return null;
            
            return elections[0];
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return null;
        }
    }

    /// <inheritdoc/>
    public List<Election> GetByNation(string nation)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = "SELECT * FROM [Election] WHERE [Nation] = @Nation";
            List<Election> elections = conn.Query<Election>(sql, new { Nation = nation }).ToList();
            
            return elections;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new List<Election>();
        }
    }

    /// <inheritdoc/>
    public List<ElectionInviteModel> GetElectionInvites(Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            
            string sql = @"SELECT * FROM [ElectionInvite] WHERE [ElectionId] = @ElectionId";
            List<ElectionInviteModel> elections = conn.Query<ElectionInviteModel>(sql, new { ElectionId = electionId }).ToList();

            if (elections.Count == 0)
                return new List<ElectionInviteModel>();
            
            return elections;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return null;
        }
    }

    /// <inheritdoc/>
    public List<User> GetCandidates(Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            
            string sql = @"
                SELECT * FROM [ElectionCandidate] ec
                INNER JOIN [User] u
                ON u.[Id] = ec.[UserId]
                WHERE ec.[ElectionId] = @ElectionId
            ";
            
            List<User> candidates = conn.Query<User>(sql, new { ElectionId = electionId }).ToList();
            return candidates;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return null;
        }
    }

    /// <inheritdoc/>
    public int Add(Election election)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                INSERT INTO [Election] (Id, [Name], Nation, [Type], StartTime, EndTime) 
                VALUES (@Id, @Name, @Nation, @Type, @StartTime, @EndTime)
            ";
            
            var rowsAffected = conn.Execute(sql, election);
            return rowsAffected;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    /// <inheritdoc/>
    public int AddElectionAdmin(Guid userId, Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                INSERT INTO [ElectionAdmin] ([UserId], [ElectionId]) 
                VALUES (@UserId, @ElectionId)
            ";
            
            var rowsAffected = conn.Execute(sql, new { UserId = userId, ElectionId = electionId });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }

    /// <inheritdoc/>
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
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int AddElectionInviteEmail(ElectionInviteModel electionInvite)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                INSERT INTO [ElectionInvite] ([ElectionId], [UserEmail], [StatusId]) 
                VALUES (@ElectionId, @UserEmail, @StatusId)
            ";

            var rowsAffected = conn.Execute(sql, new { ElectionId = electionInvite.ElectionId, UserEmail = electionInvite.UserEmail, StatusId = electionInvite.StatusId });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }

    /// <inheritdoc/>
    public int AddCandidate(Guid userId, Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

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
            
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }

    /// <inheritdoc/>
    public int Delete(Guid id)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                DELETE FROM [Election]
                WHERE [Id] = @Id
            ";
            
            var rowsAffected = conn.Execute(sql, new { Id = id });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int DeleteAllCandidates(Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            
            string sql = @"
                DELETE FROM [ElectionCandidate]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int DeleteAllAdmins(Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = @"
                DELETE FROM [ElectionAdmin]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int DeleteAllElectionInvites(Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                DELETE FROM [ElectionInvite]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int DeleteAllVotes(Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            
            string sql = @"
                DELETE FROM [Vote]
                WHERE [ElectionId] = @ElectionId
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int DeleteCandidate(Guid userId, Guid electionId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                DELETE FROM [ElectionCandidate]
                WHERE [ElectionId] = @ElectionId AND [UserId] = @UserId
            ";
            
            var rowsAffected = conn.Execute(sql, new
            {
                ElectionId = electionId,
                UserId = userId
            });
            
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int DeleteUsersElectionInvite(Guid electionId, string userEmail)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            
            string sql = @"
                DELETE FROM [ElectionInvite]
                WHERE [ElectionId] = @ElectionId AND [UserEmail] = @UserEmail
            ";
            
            var rowsAffected = conn.Execute(sql, new { ElectionId = electionId, UserEmail = userEmail });
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }

    /// <inheritdoc/>
    public int Update(Election election)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

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
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
}