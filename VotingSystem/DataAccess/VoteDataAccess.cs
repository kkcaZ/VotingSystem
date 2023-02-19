using System.Data.SqlClient;
using Dapper;
using VotingSystem.Data;
using VotingSystem.DataAccess.Abstraction;

namespace VotingSystem.DataAccess;

public class VoteDataAccess : IVoteDataAccess
{
    private readonly ILogger<VoteDataAccess> _logger;
    
    private readonly string _connectionString;

    public VoteDataAccess(ILogger<VoteDataAccess> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["ConnectionString"];
    }
    
    /// <inheritdoc/>
    public Vote? Get(Guid electionId, Guid voterId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT *
                FROM [Vote] 
                WHERE [ElectionId] = @ElectionId
                AND [VoterId] = @VoterId
            ";
            
            var voteCount = conn.Query<Vote>(sql, new { ElectionId = electionId, VoterId = voterId });

            if (!voteCount.Any())
                return null;

            return voteCount.First();
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return null;
        }
    }
    
    /// <inheritdoc/>
    public int GetCandidateVoteCount(Guid electionId, Guid candidateId)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT COUNT(*) 
                FROM [Vote] 
                WHERE [ElectionId] = @ElectionId
                AND [CandidateId] = @CandidateId
            ";
            
            var voteCount = conn.Query<int>(sql, new { ElectionId = electionId, CandidateId = candidateId }).First();
            return voteCount;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
    
    /// <inheritdoc/>
    public int Add(Vote vote)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            string sql = @"
                INSERT INTO [Vote] ([ElectionId], [CandidateId], [VoterId], [Timestamp]) 
                VALUES (@ElectionId, @CandidateId, @VoterId, @Timestamp)
            ";
            
            var rowsAffected = conn.Execute(sql, vote);
            return rowsAffected;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return 0;
        }
    }
}