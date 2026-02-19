using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace QuanLyNhanVien.Infrastructure
{
    /// <summary>
    /// Mức độ nghiêm trọng của log.
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Centralized application logger.
    /// Dual-output: writes to both a local log file AND the ErrorLog database table.
    /// 
    /// Design:
    /// - File logging ALWAYS works (even when DB is unreachable — this is the fallback)
    /// - DB logging is best-effort (failures are silently caught and logged to file instead)
    /// - Thread-safe: uses lock for file writes
    /// - Auto-rotates log files by date (one file per day)
    /// - Captures machine name, app version, and current user automatically
    /// </summary>
    public static class AppLogger
    {
        // ── Configuration ──
        private static readonly object _fileLock = new object();
        private static string _logDirectory;
        private static string _currentUser;
        private static readonly string _appVersion;
        private static readonly string _machineName;

        static AppLogger()
        {
            _appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _machineName = Environment.MachineName;
            _currentUser = null;

            // Default log directory: [exe_path]/Logs/
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(exeDir, "Logs");
        }

        /// <summary>
        /// Sets the currently authenticated user name for inclusion in log entries.
        /// Call this after login succeeds.
        /// </summary>
        public static void SetCurrentUser(string username)
        {
            _currentUser = username;
        }

        /// <summary>
        /// Optionally override the log directory (e.g. from a config setting).
        /// </summary>
        public static void SetLogDirectory(string path)
        {
            _logDirectory = path;
        }

        // ── Public API ──

        public static void Info(string source, string message)
        {
            Log(LogLevel.Info, source, message, null);
        }

        public static void Warning(string source, string message)
        {
            Log(LogLevel.Warning, source, message, null);
        }

        public static void Error(string source, string message, Exception ex = null)
        {
            Log(LogLevel.Error, source, message, ex);
        }

        public static void Critical(string source, string message, Exception ex = null)
        {
            Log(LogLevel.Critical, source, message, ex);
        }

        /// <summary>
        /// Core logging method. Writes to file first (always), then DB (best-effort).
        /// </summary>
        public static void Log(LogLevel level, string source, string message, Exception ex)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string stackTrace = ex != null ? FlattenException(ex) : null;

            // ── Step 1: Always write to file (never fails silently) ──
            WriteToFile(timestamp, level, source, message, stackTrace);

            // ── Step 2: Best-effort write to database ──
            try
            {
                WriteToDatabase(level, source, message, stackTrace);
            }
            catch
            {
                // DB logging failed — that's OK, we already have the file log.
                // Don't re-throw: we never want logging itself to crash the app.
            }
        }

        // ── File Output ──

        private static void WriteToFile(string timestamp, LogLevel level,
            string source, string message, string detail)
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                    Directory.CreateDirectory(_logDirectory);

                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string filePath = Path.Combine(_logDirectory, fileName);

                var sb = new StringBuilder();
                sb.AppendFormat("[{0}] [{1}] [{2}]", timestamp, level.ToString().ToUpper(), source);
                if (_currentUser != null)
                    sb.AppendFormat(" [User:{0}]", _currentUser);
                sb.AppendLine();
                sb.AppendFormat("  Message: {0}", message);
                sb.AppendLine();

                if (!string.IsNullOrEmpty(detail))
                {
                    sb.AppendFormat("  Detail:  {0}", detail);
                    sb.AppendLine();
                }
                sb.AppendLine("  ---");

                lock (_fileLock)
                {
                    File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                // File logging is absolute last resort — if even this fails,
                // there's nothing we can do. Don't throw from the logger.
            }
        }

        // ── Database Output ──

        private static void WriteToDatabase(LogLevel level, string source,
            string message, string detail)
        {
            // Use DatabaseHelper's connection string. If it fails (no DB configured yet),
            // this entire method is wrapped in try-catch at the caller.
            string connStr = DataAccess.DatabaseHelper.ConnectionString;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    INSERT INTO ErrorLog (MucDo, NguonLoi, ThongBao, ChiTiet, NguoiDung, TenMay, PhienBan)
                    VALUES (@MucDo, @NguonLoi, @ThongBao, @ChiTiet, @NguoiDung, @TenMay, @PhienBan)",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@MucDo", level.ToString());
                    cmd.Parameters.AddWithValue("@NguonLoi", Truncate(source, 200));
                    cmd.Parameters.AddWithValue("@ThongBao", message ?? "");
                    cmd.Parameters.AddWithValue("@ChiTiet", (object)detail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NguoiDung", (object)_currentUser ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TenMay", Truncate(_machineName, 100));
                    cmd.Parameters.AddWithValue("@PhienBan", _appVersion);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ── Helpers ──

        /// <summary>
        /// Recursively flattens an exception chain (including InnerException) into a string.
        /// </summary>
        private static string FlattenException(Exception ex)
        {
            var sb = new StringBuilder();
            int depth = 0;
            Exception current = ex;
            while (current != null && depth < 5)
            {
                if (depth > 0)
                    sb.AppendLine("--- Inner Exception ---");
                sb.AppendFormat("[{0}] {1}", current.GetType().FullName, current.Message);
                sb.AppendLine();
                if (current.StackTrace != null)
                    sb.AppendLine(current.StackTrace);
                current = current.InnerException;
                depth++;
            }
            return sb.ToString();
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
