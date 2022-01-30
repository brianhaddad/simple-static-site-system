namespace SSSP.Classes
{
    public class FileActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static FileActionResult Successful()
            => new()
            {
                Success = true,
            };

        public static FileActionResult Failed(string message)
            => new()
            {
                Success = false,
                Message = message,
            };
    }
}
