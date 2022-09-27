using SearchTool.Datatypes;

namespace SearchTool.Solver
{
    public class GenericWildcardSolver : IWildcardSolver
    {

        /*
         * TODO: The implementation assumes that a possible match is only of the form (where recordToken = R, wildcardToken = W)
         *          RWRW...RWR with multiple wildcards / records placed next to each other counts as a singular entity.
         *          This does not take into account when a wildcard is placed at either (or both) end(s) of the query, which was 
         *          inside the scope of the solution.
         */
        public List<List<Token>> Solve(string searchSpace, in List<string> listofQuerySubstrings, in List<int>[] substringRecordPositions)
        {
            var querySubstringCount = listofQuerySubstrings.Count;
            var visitedSubstringTable = new bool[querySubstringCount][];
            for (int listIndex = 0; listIndex < querySubstringCount; listIndex++)
            {
                visitedSubstringTable[listIndex] = new bool[substringRecordPositions[listIndex].Count];
            }

            var searchResults = new List<List<Token>>();

            // NOTE: is it smarter to do a conditional-and-continue or a conditional-to-codeblock
            // Worst case each initial substring constructs a result so interate over all of them
            // Iterate over every possible starting point for the matching
            for (int initialSubstringIndex = 0; initialSubstringIndex < substringRecordPositions[0].Count; initialSubstringIndex++)
            {
                // Bailout fast in case the current substring has already been visited
                bool currentInitialSubstringHasBeenVisited = visitedSubstringTable[0][initialSubstringIndex];
                if (currentInitialSubstringHasBeenVisited)
                    continue;

                
                // Manually construct the first entry, then reuse it as a variable for holding the previous record
                var previousRecord = new RecordToken(
                    listofQuerySubstrings[0], 
                    substringRecordPositions[0][initialSubstringIndex]
                    );

                var tempResult = new List<Token> { previousRecord };
                visitedSubstringTable[0][initialSubstringIndex] = true;

                // Find the other [n-1] entries for a viable solution
                for (int substringIndex = 1; substringIndex < querySubstringCount; substringIndex++)
                {
                    for (int occurenceIndex = 0; occurenceIndex < substringRecordPositions[substringIndex].Count; occurenceIndex++)
                    {
                        // Bailout fast in case the current substring has already been visited
                        bool currentSubstringHasBeenVisited = visitedSubstringTable[substringIndex][occurenceIndex];
                        if (currentSubstringHasBeenVisited)
                            continue;
                        visitedSubstringTable[substringIndex][occurenceIndex] = true;

                        var currentSubstringRecord = substringRecordPositions[substringIndex][occurenceIndex];
                        bool currentSubstringIsViable = previousRecord.position + previousRecord.value.Length < currentSubstringRecord;
                        if (currentSubstringIsViable)
                        {
                            var previousRecordEndPosition = previousRecord.position + previousRecord.value.Length;
                            bool thereExistAWildcard = currentSubstringRecord - previousRecordEndPosition > 0;
                            if (thereExistAWildcard)
                            {
                                var wildcard = new WildcardToken(
                                    searchSpace.Substring(previousRecordEndPosition, currentSubstringRecord - previousRecordEndPosition),
                                    previousRecordEndPosition);
                                tempResult.Add(wildcard);
                            }
                            var tempSubstringRecord = new RecordToken(listofQuerySubstrings[substringIndex], substringRecordPositions[substringIndex][occurenceIndex]);
                            tempResult.Add(tempSubstringRecord);

                            previousRecord = tempSubstringRecord;
                            HelperFunctions.UpdateVisitedTable(ref visitedSubstringTable, in substringRecordPositions, tempSubstringRecord.position);
                            break;
                        }
                    }
                }

                // Assert that finding the result contains [n] recordTokens and [n-1] wildcardTokens
                bool aCompleteResultWasFound = tempResult.Count() == listofQuerySubstrings.Count() * 2 - 1;
                if (aCompleteResultWasFound)
                    searchResults.Add(tempResult);
            }

            return searchResults;
        }
    }
}
