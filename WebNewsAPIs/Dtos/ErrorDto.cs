namespace WebNewsAPIs.Dtos
{
    public class ErrorModel
    {
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
