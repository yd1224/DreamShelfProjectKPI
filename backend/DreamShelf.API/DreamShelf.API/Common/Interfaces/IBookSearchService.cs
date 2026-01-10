using DreamShelf.API.Common.Dtos.Response;

namespace DreamShelf.API.Common.Interfaces;

public interface IBookSearchService
{
    void AddToIndex(int id, string title);
    List<int> Search(string term);
    PaginationSearchResultServiceDto PaginationSearch(string term, int page);
    void RemoveFromIndex(int id);
}