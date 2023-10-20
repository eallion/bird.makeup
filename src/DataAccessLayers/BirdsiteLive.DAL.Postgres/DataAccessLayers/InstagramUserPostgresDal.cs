using BirdsiteLive.DAL.Postgres.Settings;

namespace BirdsiteLive.DAL.Postgres.DataAccessLayers;

public class InstagramUserPostgresDal : UserPostgresDal
{
    
        #region Ctor
        public InstagramUserPostgresDal(PostgresSettings settings) : base(settings)
        {
            tableName = _settings.InstagramUserTableName;
        }
        #endregion

        public override string tableName { get; set; }
}