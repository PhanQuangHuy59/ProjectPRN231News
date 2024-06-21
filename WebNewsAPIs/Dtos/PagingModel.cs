namespace WebNewsAPIs.Dtos
{
	public class PagingModel
	{
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public int expander { get; set; }
		public Func<int, string> Url { get; set; }

	}
}
