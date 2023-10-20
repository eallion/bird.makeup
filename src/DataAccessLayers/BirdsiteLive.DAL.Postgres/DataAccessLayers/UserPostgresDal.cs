using System;
using System.Threading.Tasks;
using BirdsiteLive.DAL.Models;
using BirdsiteLive.DAL.Postgres.DataAccessLayers.Base;
using BirdsiteLive.DAL.Postgres.Settings;
using Npgsql;

namespace BirdsiteLive.DAL.Postgres.DataAccessLayers;

public abstract class UserPostgresDal : PostgresBase
{
    
        #region Ctor
        public UserPostgresDal(PostgresSettings settings) : base(settings)
        {
            
        }
        #endregion

        public abstract string tableName { get; set; }
        
        public async Task<SyncTwitterUser> GetTwitterUserAsync(string acct)
        {
            var query = $"SELECT * FROM {tableName} WHERE acct = $1";

            acct = acct.ToLowerInvariant();

            await using var connection = DataSource.CreateConnection();
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(query, connection) {
                Parameters = { new() { Value = acct}}
            };
            var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;
            
            return new SyncTwitterUser
            {
                Id = reader["id"] as int? ?? default,
                Acct = reader["acct"] as string,
                TwitterUserId = reader["twitterUserId"] as long? ?? default,
                LastTweetPostedId = reader["lastTweetPostedId"] as long? ?? default,
                LastSync = reader["lastSync"] as DateTime? ?? default,
                FetchingErrorCount = reader["fetchingErrorCount"] as int? ?? default,
                FediAcct = reader["fediverseaccount"] as string,
            };

        }
}