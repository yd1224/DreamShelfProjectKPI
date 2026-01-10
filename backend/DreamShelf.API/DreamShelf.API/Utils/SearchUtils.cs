using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace DreamShelf.API.Utils;

public static class SearchUtils
{
    /// <summary>
    /// Create a <see cref="BooleanQuery"/> which follows the logic:
    /// <para>
    /// Splits the query into individual lowercase non-empty tokens and for each token (besides the last one) creates
    /// a full term query. For the last one creates a prefix query.
    /// </para>
    /// </summary>
    /// <param name="query">Term to search</param>
    /// <param name="fieldToSearch">Field to search in</param>
    /// <returns>A query to use in searcher</returns>
    public static BooleanQuery CreateLastTermPrefixQuery(string query, string fieldToSearch)
    {
        var tokens = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0) return [];
        
        var booleanQuery = new BooleanQuery();
        
        for (int i = 0; i < tokens.Length - 1; i++)
        {
            booleanQuery.Add(new TermQuery(new Term(fieldToSearch, tokens[i])), Occur.MUST);
        }
 
        var lastToken = tokens[^1];
        booleanQuery.Add(new PrefixQuery(new Term(fieldToSearch, lastToken)), Occur.MUST);
        
        return booleanQuery;
    }
    


}