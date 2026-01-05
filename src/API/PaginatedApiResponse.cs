namespace API;

public record PaginatedApiResponse : ApiResponse
{
    public int SelectedPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int ItemCount { get; set; }

    public PaginatedApiResponse(string message, bool success, object? result, int statuscode, int selectedPage, int totalpage, int pagesize, int itemcount) : base(message, success, result, statuscode)
    {
        SelectedPage = selectedPage;
        TotalPages = totalpage;
        PageSize = pagesize;
        ItemCount = itemcount;
    }

}
