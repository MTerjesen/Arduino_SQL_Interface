using System.Threading.Tasks;
namespace Arduino_SQL_Interface
/// <summary>
/// Handles writing to the SQL database
/// </summary>
{
    class DatabaseLight : SqlConnLight
    {

        public string table { get; private set; }
        /// <summary>
        /// Creates a new object without any properties
        /// </summary>
        public DatabaseLight() : base()
        {

        }
        /// <summary>
        /// Creates a new object for writing to a database
        /// </summary>
        /// <param name="Server">Server to connect to</param>
        /// <param name="Database">Database to use</param>
        /// <param name="Table">Table to write to</param>
        /// <param name="User">Username for connecting to server</param>
        /// <param name="Password">Password for the user</param>
        public DatabaseLight(string Server, string Database, string Table, string User, string Password) : base(Server, Database, User, Password)
        {
            table = Table;
        }
        /// <summary>
        /// Async version
        /// Writes data to the table.
        /// Designed for writing to the 'Jars' table
        /// </summary>
        /// <param name="amountDelivered">Data to write to AmountDelivered</param>
        /// <param name="rfid">Rfid of jar</param>
        /// <returns>Task</returns>
        public async Task WriteAsync(string status, string JarId, string amountDelivered = "")
        {
            try
            {
                string query = string.Format("UPDATE {0} SET AmountDelivered={1}, Status={2} WHERE JarId={3}", table, amountDelivered, status, JarId);
                await RunQueryAsync(query);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Writes data to the table.
        /// Designed for writing to the 'Jars' table
        /// </summary>
        /// <param name="amountDelivered">Data to write to AmountDelivered</param>
        /// <param name="rfid">Rfid of jar</param>
        public void Write(string status, string JarId, string amountDelivered = "")
        {
            try
            {
                string query = string.Format("UPDATE {0} SET AmountDelivered={1}, Status={2} WHERE JarId={3}", table, amountDelivered, status, JarId);
                RunQuery(query);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
    }
}
