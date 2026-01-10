using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Utils;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace DreamShelf.API.Services;

public class AuthorSearchService : IAuthorSearchService
{
    private const LuceneVersion LuceneVersion = Lucene.Net.Util.LuceneVersion.LUCENE_48;
    private readonly FSDirectory _indexDirectory;
    private readonly IndexWriter _indexWriter;

    public AuthorSearchService()
    {
        var indexDirPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "author-index");
        
        _indexDirectory = FSDirectory.Open(indexDirPath);
        
        var analyzer = new StandardAnalyzer(LuceneVersion);
        
        var config = new IndexWriterConfig(LuceneVersion, analyzer);
        
        _indexWriter = new IndexWriter(_indexDirectory, config);
    }
    
    
    public void AddToIndex(int id, string? fullName, string? pseudonym)
    {
        var document = new Lucene.Net.Documents.Document()
        {
            new StringField("id", id.ToString(), Field.Store.YES),
            new TextField("fullName", fullName ?? "", Field.Store.YES),
            new TextField("pseudonym", pseudonym ?? "", Field.Store.YES),
        };
        
        _indexWriter.UpdateDocument(new Term("id", id.ToString()), document);
        _indexWriter.Commit();
    }

    public List<AuthorResponseDto> Search(string term)
    {
        if (!DirectoryReader.IndexExists(_indexDirectory))
            return []; 
        
        using var reader = DirectoryReader.Open(_indexDirectory);
        List<AuthorResponseDto> results = new List<AuthorResponseDto>();
        
        
        var searcher = new IndexSearcher(reader);

        var fullNameQuery = SearchUtils.CreateLastTermPrefixQuery(term, "fullName");
        var pseudonymQuery = SearchUtils.CreateLastTermPrefixQuery(term, "pseudonym");
        
        var booleanQuery = new BooleanQuery
        {
            { fullNameQuery, Occur.SHOULD },
            { pseudonymQuery, Occur.SHOULD }
        };

        var topDocs = searcher.Search(booleanQuery, 20);

        foreach (var result in topDocs.ScoreDocs)
        {
            var  doc = searcher.Doc(result.Doc);
            var id = int.Parse(doc.Get("id"));
            var fullName = doc.Get("fullName");
            var pseudonym = doc.Get("pseudonym");
            
            results.Add(new AuthorResponseDto
            {
                Id = id,
                DisplayName = UserUtils.DisplayName(fullName, pseudonym)
            });
        }
        
        return results;
    }

 
}
