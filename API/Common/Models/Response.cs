namespace API.Common.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Result { get; set; }
    }
}
