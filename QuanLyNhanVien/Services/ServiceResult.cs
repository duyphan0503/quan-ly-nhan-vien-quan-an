namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Bao bọc (wrapper) chung biểu thị kết quả cho những thao tác diễn ra ở tầng Dịch Vụ - Service Layer.
    /// Giúp hạn chế việc sử dụng Throw Exceptions cho các lỗi xác thực thông thường —
    /// Ngoại lệ (Exceptions) chỉ được kích hoạt (Throw) cho các lỗi mang tính nghiêm trọng (Hệ quản trị CSDL ngắt kết nối, lỗi luồng,...).
    /// </summary>
    /// <typeparam name="T">Loại dữ liệu sẽ được phản hồi lại nếu thành công.</typeparam>
    public class ServiceResult<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }

        private ServiceResult() { }

        /// <summary>Khởi tạo một kết quả với thông tin cấu trúc thành công kèm theo ghi chú (tùy chọn).</summary>
        public static ServiceResult<T> Ok(T data, string message = null)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
            };
        }

        /// <summary>Khởi tạo một kết quả báo cáo thất bại với nguyên nhân (ghi chú lỗi).</summary>
        public static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Data = default(T),
                Message = message,
            };
        }
    }

    /// <summary>
    /// Định nghĩa kiểu kết quả dạng đối tượng phi hình thức để không phải trả về dữ liệu (void-like).
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private ServiceResult() { }

        /// <summary>Khởi tạo kết quả mang thông báo thành công (có thể kẹp ghi chú).</summary>
        public static ServiceResult Ok(string message = null)
        {
            return new ServiceResult { Success = true, Message = message };
        }

        /// <summary>Khởi tạo kết quả thất bại bao gồm thông tin chi tiết.</summary>
        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }
    }
}
