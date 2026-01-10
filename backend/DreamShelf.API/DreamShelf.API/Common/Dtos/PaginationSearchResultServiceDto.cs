namespace DreamShelf.API.Common.Dtos.Response;

public class PaginationSearchResultServiceDto
{
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public List<int> BookIds { get; set; } = [];
}