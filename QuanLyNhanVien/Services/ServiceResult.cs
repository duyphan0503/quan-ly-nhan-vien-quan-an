namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Generic result wrapper for Service Layer operations.
    /// Avoids using exceptions for expected validation failures —
    /// only truly exceptional errors (DB connection, etc.) should throw.
    /// </summary>
    /// <typeparam name="T">The type of data returned on success.</typeparam>
    public class ServiceResult<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }

        private ServiceResult() { }

        /// <summary>Creates a successful result with data and optional message.</summary>
        public static ServiceResult<T> Ok(T data, string message = null)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>Creates a failed result with an error message.</summary>
        public static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Data = default(T),
                Message = message
            };
        }
    }

    /// <summary>
    /// Non-generic result for operations that don't return data (void-like).
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private ServiceResult() { }

        /// <summary>Creates a successful result with optional message.</summary>
        public static ServiceResult Ok(string message = null)
        {
            return new ServiceResult { Success = true, Message = message };
        }

        /// <summary>Creates a failed result with an error message.</summary>
        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }
    }
}
