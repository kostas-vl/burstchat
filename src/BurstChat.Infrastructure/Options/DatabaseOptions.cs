namespace BurstChat.Infrastructure.Options
{
    /// <summary>
    //  Contains configuration properties for establishing a connection with a database.
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// The database provider technology.
        /// </summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        //  The appropriate connection string based on the value of the provided.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}
