namespace WebNewsAPIs.Dtos
{
    public class SearchPaging<T>
    {
        public int total { get;set; }
        public T result { get;set; }
    }
}
