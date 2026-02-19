using System;
using System.Configuration;
using System.Data.SqlClient;

namespace QuanLyNhanVien.DataAccess
{
    /// <summary>
    /// Thread-safe database connection factory.
    /// Uses Lazy&lt;T&gt; for safe one-time initialization of the connection string.
    /// 
    /// Supports runtime connection string updates from the Connection Wizard:
    /// after the wizard saves to App.config and calls RefreshConnectionString(),
    /// subsequent calls to GetConnection() use the new value.
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly object _lock = new object();
        private static string _connectionString;
        private static bool _initialized;

        /// <summary>
        /// The connection string, lazily initialized from App.config.
        /// Thread-safe.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (!_initialized)
                {
                    lock (_lock)
                    {
                        if (!_initialized)
                        {
                            var cs = ConfigurationManager.ConnectionStrings["QuanLyNhanVien"];
                            if (cs == null)
                                throw new InvalidOperationException(
                                    "Connection string 'QuanLyNhanVien' not found in App.config.");
                            _connectionString = cs.ConnectionString;
                            _initialized = true;
                        }
                    }
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// Creates a new SqlConnection. Caller is responsible for opening and disposing.
        /// Each call returns a fresh connection — no shared mutable state.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Forces re-reading the connection string from App.config.
        /// Call this after the Connection Wizard updates the config.
        /// </summary>
        public static void RefreshConnectionString()
        {
            lock (_lock)
            {
                _initialized = false;
                _connectionString = null;
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }

        /// <summary>
        /// Quick connectivity test. Returns true if a connection can be opened
        /// and a simple query executed. Used by Program.Main to decide whether
        /// to launch the Connection Wizard.
        /// </summary>
        public static bool TestConnection(int timeoutSeconds = 3)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                builder.ConnectTimeout = timeoutSeconds;

                using (var conn = new SqlConnection(builder.ToString()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        cmd.ExecuteScalar();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
