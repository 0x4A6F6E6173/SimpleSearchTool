using SearchTool.Datatypes;
using SearchTool.Solver;

namespace SearchTool
{
    // REFACTOR: Use string streams instead of strings (don't know if possible (should certainly be))
    // REFACTOR: Use tokens for quaries
    // REFACTOR: Simply function headers
    // TODO: Abstract queries into (searchspace, searchQuery) and (searchspace, List[searchQueries]) Maybe not?
    // TODO: Standardise the naming scheme for readability
    // TODO: Refactor til at bruge enumerators og movenext?
    // TODO: Needs functionality for starting or ending with a wildcard
    // TODO: Going Search.Wildcard(...)[n][substring] feels horrible with substring, change to have 2 ordered lists of searchqueries and wildcards
    // TODO: Make it possible to search on wildcards found:
    //      Given a search query with a wildcard: "query\?string{wildcard0}" then
    //      it should be possible to use the wildcards found value in the search following it.

    /*
     * TODO Construct a more streamlined-for-use featureset
     * 
     * searchspace = Stream|Iterator|String;
     * searchQuery = String;
     * searchResult = Search.ProvideSearchspace(searchSpace).FindString(searchQuery);
     * or
     * searchResult = Search.ProvideSearchSpace(searchSpace).Search(options => {
     *      options.UseFindFirst() | options.UseFindEvery() | options.UseFindEvery(Force);
     *      options.GenericWildCardSearch(searchQuery);
     * });
     */

    public static class Search
    {
        public static readonly string wildcardToken = @"\?";

        public static bool ContainsString(Query query)
        {
            for (int index = 0; index < query.SearchSpace.Length; index++)
            {
                var foundFirstCharOfSearchQuery = query.SearchSpace[index] == query.SearchQuery[0];
                if (foundFirstCharOfSearchQuery)
                {
                    // Assert whether the current substring@Index matches the full search query
                    if (AssertSearchInput(query.SearchQuery, query.SearchSpace, index))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Returns a list of every occurrence of the search query that was found in the search space
        public static List<Token> FindString(Query query)
        {
            var recordList = new List<Token>();

            for (int index = 0; index < query.SearchSpace.Length; ++index)
            {
                // If we find the search queries first char in the search space
                var foundFirstCharOfSearchQuery = query.SearchSpace[index] == query.SearchQuery[0];
                if (foundFirstCharOfSearchQuery)
                {
                    // Assert whether the current substring@Index matches the full search query
                    if (AssertSearchInput(query.SearchQuery, query.SearchSpace, index))
                    {
                        recordList.Add(new RecordToken(query.SearchQuery, index));
                    }
                }
            }

            return recordList;
        }

        // Returns a list of every occurrence of the search query that was found in the search space
        public static List<int>[] FindStrings(string searchSpace, List<string> searchQueries)
        {
            return FindSearchStringsByPosition(searchSpace, searchQueries);
        }

        // Default call
        public static List<List<Token>> WildcardSearch(Query query) { return WildcardSearch(query, new GenericWildcardSolver()); }

        public static List<List<Token>> WildcardSearch(Query query, IWildcardSolver solver)
        {
            /* Making a generic wildcard search over [n] substrings and [n-1] wildcards requires that:
             *  - Rule 1: Each querying substring must exist for the complete search to be viable.
             *  - Rule 2: Each substring i in n must be found in order. (0 < i < n)
             *  
             *  Part 1: Split the query up into substringQueries delimited by the wildcard token.
             *  Part 2: Construct a matrix containing every occurence of every substring in the searchSpace.
             *  Part 3: Find as many full search queries from the matrix as possible.
             */
            // ============================ PART 1 ============================
            var listofQuerySubstrings = CutoutWildcard(query.SearchQuery);
            var searchSpace = query.SearchSpace;
            var querySubstringCount = listofQuerySubstrings.Count;

            foreach (var str in listofQuerySubstrings)
                Console.Write(str + " ");
            Console.Write('\n');

            // Bail out fast on an empty search
            bool bailoutOnEmptySearchSpace = querySubstringCount == 0;
            bool bailoutOnEmptyQuery = query.SearchSpace.Length == 0;
            if (bailoutOnEmptySearchSpace || bailoutOnEmptyQuery) return new List<List<Token>>();

            // ============================ PART 2 ============================
            var substringSearchRecords = FindSearchStringsByPosition(searchSpace, listofQuerySubstrings);

            // Assert that each query substring was found in the search space at least once.
            foreach (var substrSearchRecord in substringSearchRecords)
            {
                bool foundNoViableQueryResult = substrSearchRecord.Count == 0;
                if (foundNoViableQueryResult) return new List<List<Token>>();
            }

            // ============================ PART 3 ============================
            return solver.Solve(searchSpace, in listofQuerySubstrings, in substringSearchRecords);
        }


        /// <summary>
        /// Lists the position of every occurence of the query substrings found in the searchspace
        /// </summary>
        /// <param name="searchSpace"></param>
        /// <param name="searchQueries"></param>
        /// <returns>
        /// An array where the i'th array index gives a list containing the integer position of every occurence of the i'th substring.
        /// </returns>
        private static List<int>[] FindSearchStringsByPosition(string searchSpace, List<string> searchQueries)
        {
            var firstCharOfEachQuery = (from queries in searchQueries select queries[0]).ToArray();
            var substringSearchRecords = new List<int>[searchQueries.Count];
            //substringSearchRecords.Distinct<List<int>>();
            for (int index = 0; index < searchQueries.Count; index++)
            {
                substringSearchRecords[index] = new List<int>();
            }

            for (int searchSpaceIndex = 0; searchSpaceIndex < searchSpace.Length; searchSpaceIndex++)
            {
                for (int querySubstringListIndex = 0; querySubstringListIndex < firstCharOfEachQuery.Length; querySubstringListIndex++)
                {
                    bool foundFirstCharOfSearchQuery = searchSpace[searchSpaceIndex] == firstCharOfEachQuery[querySubstringListIndex];
                    if (foundFirstCharOfSearchQuery)
                    {
                        // Assert whether the current substringQuery@searchSpaceIndex matches the full search query
                        // TODO: Needs refactoring, the functions return value dictates flow directly. Look for a fix in the books.
                        // TODO: This opens the door for 2 viable near-identical strings, where one is concatenated with a suffix, to both be accepted at the same positions.
                        if (AssertSearchInput(searchQueries[querySubstringListIndex], searchSpace, searchSpaceIndex))
                        {
                            substringSearchRecords[querySubstringListIndex].Add(searchSpaceIndex);
                        }
                    }
                }
            }

            return substringSearchRecords;
        }

        private static List<string> CutoutWildcard(string searchQuery)
        {
            // Find every wildcard in the searchQuery
            var wildcardSearchRecord = new Query(searchQuery, wildcardToken);
            var wildcardTokens = FindString(wildcardSearchRecord);

            // Bailout fast in case there isn't a wildcard
            bool thereIsNoWildcard = wildcardTokens.Count == 0;
            if (thereIsNoWildcard) return new() { searchQuery };

            // Split the searchQuery up using the wildcard(s) position(s).
            int position = 0;
            var wildcardTokenLength = wildcardToken.Length;
            var result = new List<string>();
            foreach (var wildcardToken in wildcardTokens)
            {
                result.Add(searchQuery[position..wildcardToken.position]);
                position += wildcardToken.position - position + wildcardTokenLength;
            }

            // Edgecase: catching the last substring
            var lastWildcardToken = wildcardTokens.LastOrDefault();
            bool isThereALastSubstring = position <= searchQuery.Length;
            if (isThereALastSubstring)
            {
                result.Add(searchQuery[position..searchQuery.Length]);
            }

            // Clean up empty substrings
            for (int index = 0; index < result.Count; index++)
            {
                bool substringIsEmpty = result[index].Length == 0;
                if (substringIsEmpty) result.RemoveAt(index);
            }

            return result;
        }

        private static bool AssertSearchInput(string query, string searchSpace, int searchSpaceIndexPosition)
        {
            // Iterate over the searchspace at the current position and assert whether the current string matches 'quary'
            for (int searchSpaceIndex = searchSpaceIndexPosition, queryIndex = 0;
                     queryIndex < query.Length;
                   ++searchSpaceIndex, ++queryIndex)
            {
                // Merging the if statements could lead to indexing outside the array, which would throw an exception.
                var reachedEndOfSearchSpace = searchSpaceIndex >= searchSpace.Length;
                if (reachedEndOfSearchSpace) return false;

                bool missmatchBetweenSearchQueryAndSearchSpaceSubstring = searchSpace[searchSpaceIndex] != query[queryIndex];
                if (missmatchBetweenSearchQueryAndSearchSpaceSubstring) return false;
            }

            return true;
        }
    }
}