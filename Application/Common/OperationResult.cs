namespace Domain
{
    public class OperationResult<T>
    {
        public string? Message { get; set; }
        public bool Success { get; set; }  // ← no es nullable
        public T? Data { get; set; }

        public static OperationResult<T> Ok(string message, T? data = default)
        {
            return new OperationResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static OperationResult<T> Fail(string message)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }

    }
}


