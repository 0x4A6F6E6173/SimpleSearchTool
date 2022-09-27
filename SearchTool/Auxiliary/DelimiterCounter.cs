using SearchTool.Datatypes;

namespace SearchTool.Auxiliary
{
    // TODO: Array.Empty(...) most likely creates an array of the given type at a default type, would save time and space to init with the right side at the getgo.

    // Simplifies the data handling and access stemming from the delimiters occuring in the searchspace and searchqueries
    public class DelimiterCounter
    {
        // For 'n' ordered substrings where each substring has a direct relation to the one following then
        // the indicies are defined as [pair of substrings][# of the first pair substring, # of second pair substring][delimiter (indexed by delimiterMapper)]
        private int[][,][] delimitersInSearchSpace;
        // 
        private int[,] delimitersInSubstrings;
        private readonly List<Delimiter> delimiters;

        // Index mapping for each delimiter
        private Dictionary<string, int> delimiterMapper;

        // Provide a custom set of delimiters
        public DelimiterCounter(List<Delimiter> delimiters, int substringCount)
        {
            delimiterMapper = new Dictionary<string, int>();
            this.delimiters = delimiters;
            delimitersInSearchSpace = new int[substringCount][,][];
            delimitersInSubstrings = new int[substringCount, delimiters.Count * 2];
            InitializeDelimiterMapper(in delimiters);
        }

        // Use the default delimiters
        public DelimiterCounter(int substringCount)
        {
            delimiterMapper = new Dictionary<string, int>();
            delimiters = new List<Delimiter>
            {
                new Delimiter("(", ")"),
                new Delimiter("[", "]"),
                new Delimiter("{", "}"),
                new Delimiter("<", ">")
            };
            delimitersInSearchSpace = new int[substringCount][,][];
            delimitersInSubstrings = new int[substringCount, delimiters.Count * 2];
            InitializeDelimiterMapper(in delimiters);
        }

        private void InitializeDelimiterMapper(in List<Delimiter> delimiterList)
        {
            int indexCounter = 0;
            foreach (var delimiter in delimiterList)
            {
                delimiterMapper[delimiter.PreDelimiter] = indexCounter++;
                delimiterMapper[delimiter.PostDelimiter] = indexCounter++;
            }
        }

        // returns the amount of braces found between the j'th and k'th pairs of substring occurences at the i'th substring space.
        public int[] this[int i, int j, int k] { get { return delimitersInSearchSpace[i][j, k]; } }

        // Counts every occurence of '(', ')', '{', '}', '[', and ']' between every substring found.
        public DelimiterCounter PopulateCounter(string searchspace, in List<string> listofQuerySubstrings, in List<int>[] substringRecordPositions)
        {
            CountDelimitersInSubstrings(in listofQuerySubstrings);
            CountDelimitersInSearchSpace(searchspace, listofQuerySubstrings.Count(), in substringRecordPositions);

            return this;
        }

        private void CountDelimitersInSearchSpace(string searchspace, int querySubstringCount, in List<int>[] substringRecordPositions)
        {
            // This could be made more efficient than brute force by using dynamic programming - but no reasonable profit can be projected without the necessary data.
            /*
             * For n distinct substrings and k occurences of each substring then
             * The first index is for choosing between the n-1 different pairs of ordered substrings
             * The secound matrix indexing is for chooseing between the kxk different possible occurences
             * The third index is for accessing the count of either 0: '()', 1: '{}', or 2: '[]'.
             */
            /*
            var orderedBraceAccumulator = new List<BraceAccumulator>[3];
            orderedBraceAccumulator[0] = new List<BraceAccumulator>();
            orderedBraceAccumulator[1] = new List<BraceAccumulator>();
            orderedBraceAccumulator[2] = new List<BraceAccumulator>();

            for (int searchSpaceIndex = 0; searchSpaceIndex < searchspace.Length; searchSpaceIndex++)
            {
                var currentChar = searchspace[searchSpaceIndex];
                if (currentChar is '(') orderedBraceAccumulator[0].Add(new BraceAccumulator(searchSpaceIndex, orderedBraceAccumulator[0][^1].accumulated + 1));
                if (currentChar is ')') orderedBraceAccumulator[0].Add(new BraceAccumulator(searchSpaceIndex, orderedBraceAccumulator[0][^1].accumulated - 1));
                if (currentChar is '[') orderedBraceAccumulator[1].Add(new BraceAccumulator(searchSpaceIndex, orderedBraceAccumulator[1][^1].accumulated + 1));
                if (currentChar is ']') orderedBraceAccumulator[1].Add(new BraceAccumulator(searchSpaceIndex, orderedBraceAccumulator[1][^1].accumulated - 1));
                if (currentChar is '{') orderedBraceAccumulator[2].Add(new BraceAccumulator(searchSpaceIndex, orderedBraceAccumulator[2][^1].accumulated + 1));
                if (currentChar is '}') orderedBraceAccumulator[2].Add(new BraceAccumulator(searchSpaceIndex, orderedBraceAccumulator[2][^1].accumulated - 1));
            }

            delimitersInSearchSpace = new int[querySubstringCount - 1][,][];
            for (int querySubstringIndex = 0; querySubstringIndex < querySubstringCount; querySubstringIndex++)
            {
                delimitersInSearchSpace[querySubstringIndex] = new int[substringRecordPositions[querySubstringIndex].Count(), substringRecordPositions[querySubstringIndex + 1].Count()][];
                for (int startIndex = 0; startIndex < substringRecordPositions[querySubstringIndex].Count(); startIndex++)
                {
                    for (int endIndex = 0; endIndex < substringRecordPositions[querySubstringIndex + 1].Count(); endIndex++)
                    {
                        var braces = new int[3];
                        braces[0] = 0;
                        braces[1] = 1;
                        braces[2] = 2;

                        delimitersInSearchSpace[querySubstringIndex][startIndex, endIndex] = braces;
                    }
                }
            }
            */
        }


        /// <summary>
        /// Loops through every query substring, counting the occurences of the different delimiters.
        /// </summary>
        /// <param name="listofQuerySubstrings"></param>
        private void CountDelimitersInSubstrings(in List<string> listofQuerySubstrings)
        {
            var delimiters = delimiterMapper.Keys.ToList();
            for (int substringIndex = 0; substringIndex < listofQuerySubstrings.Count; substringIndex++)
            {
                var foundOccurences = Search.FindStrings(listofQuerySubstrings[substringIndex], delimiters);

            }
        }

        /// <summary>
        ///  Given any delimiter found in the process of execution update the supplied counter based on whether the delimiter is the pre- or post- delimiter.
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="counter"></param>
        private void mapUpdater(string delimiter, ref int[] counter)
        {
            counter[delimiterMapper[delimiter]]++;
        }
    }
}