using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Utils;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace DreamShelf.API.Services;

public class BookSearchService: IBookSearchService
{
    private const LuceneVersion LuceneVersion = Lucene.Net.Util.LuceneVersion.LUCENE_48;
    private readonly FSDirectory _indexDirectory;
    private readonly IndexWriter _indexWriter;
    private  readonly Analyzer _analyzer;
    private const int ToTake = 5;

    public BookSearchService()
    {
        var indexDirPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "book-index");
        
        _indexDirectory = FSDirectory.Open(indexDirPath);
        
         _analyzer = new StandardAnalyzer(LuceneVersion, new CharArraySet(LuceneVersion.LUCENE_48, new List<char[]>(), false));
        
        var config = new IndexWriterConfig(LuceneVersion, _analyzer);
        
        _indexWriter = new IndexWriter(_indexDirectory, config);
    }
    
    public void AddToIndex(int id, string title)
    {
        
        var document = new Lucene.Net.Documents.Document()
        {
            new StringField("id", id.ToString(), Field.Store.YES),
            new TextField("title", title ?? "", Field.Store.YES),
        };
        
        _indexWriter.UpdateDocument(new Term("id", id.ToString()), document);
        _indexWriter.Commit();
    }

    public List<int> Search(string term)
    {
        if (!DirectoryReader.IndexExists(_indexDirectory))
            return []; 
        
        using var reader = DirectoryReader.Open(_indexDirectory);
        List<int> results = [];
        
        var searcher = new IndexSearcher(reader);

        term = term.RemovePunctuation();
        
        var query = SearchUtils.CreateLastTermPrefixQuery(term, "title");
   
        var topDocs = searcher.Search(query, ToTake);

        foreach (var result in topDocs.ScoreDocs)
        {
            var  doc = searcher.Doc(result.Doc);
            var id = int.Parse(doc.Get("id"));
            
            results.Add(id);
        }
        
        return results;
    }
    
    public PaginationSearchResultServiceDto PaginationSearch(string term, int page)
    {
        if (!DirectoryReader.IndexExists(_indexDirectory))
            return new PaginationSearchResultServiceDto(); 
        
        using var reader = DirectoryReader.Open(_indexDirectory);
        List<int> results = [];
        
        var searcher = new IndexSearcher(reader);
        
        var query = SearchUtils.CreateLastTermPrefixQuery(term, "title");
   
        var topDocs = searcher.Search(query, page * ToTake);
        
        var paginatedDocs = topDocs.ScoreDocs
            .ToList()
            .Skip((page - 1) * ToTake)
            .Take(ToTake);
        
        foreach (var result in paginatedDocs)
        {
            var  doc = searcher.Doc(result.Doc);
            var id = int.Parse(doc.Get("id"));
            
            results.Add(id);
        }
        
        return new PaginationSearchResultServiceDto
        {
            BookIds = results,
            HasNextPage = topDocs.ScoreDocs.Length >= page * ToTake,
            HasPreviousPage = page > 1,
        };
    }

    public void RemoveFromIndex(int id)
    {
        _indexWriter.DeleteDocuments(new Term("id", id.ToString()));
    }
}

