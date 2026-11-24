using System.Configuration;

namespace RecommendationApp
{
    /// <summary>
    /// Single place that knows how to reach the database.
    ///
    /// The connection string lives in App.config, so changing the server or moving
    /// to SQL authentication does not mean editing every form. No password is
    /// hard-coded here.
    /// </summary>
    public static class Database
    {
        private const string DefaultConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=RecommendationDB;Trusted_Connection=True;";

        public static string ConnectionString
        {
            get
            {
                var setting = ConfigurationManager.ConnectionStrings["RecommendationDB"];
                return setting != null && !string.IsNullOrWhiteSpace(setting.ConnectionString)
                    ? setting.ConnectionString
                    : DefaultConnectionString;
            }
        }
    }
}
