using SearchTool.Datatypes;
using SearchTool.Auxiliary;

namespace SearchTool.Solver
{
    public class SemanticWildcardSolver : IWildcardSolver
    {
        public List<List<Token>> Solve(string searchSpace, in List<string> listofQuerySubstrings, in List<int>[] substringRecordPositions)
        {
            List<List<Token>> searchResults;
            int querySubstringCount;
            bool[][] visitedSubstringTable;

            searchResults = new List<List<Token>>();
            querySubstringCount = listofQuerySubstrings.Count;
            visitedSubstringTable = new bool[querySubstringCount][];
            for (int listIndex = 0; listIndex < querySubstringCount; listIndex++)
            {
                visitedSubstringTable[listIndex] = new bool[substringRecordPositions[listIndex].Count];
            }

            var braceCounter = new DelimiterCounter(querySubstringCount).PopulateCounter(searchSpace, listofQuerySubstrings, substringRecordPositions);

            for (int initialSubstringIndex = 0; initialSubstringIndex < substringRecordPositions[0].Count; initialSubstringIndex++)
            {
                bool currentInitialSubstringHasBeenVisited = visitedSubstringTable[0][initialSubstringIndex];
                if (currentInitialSubstringHasBeenVisited)
                    continue;

                var previousRecord = new RecordToken(listofQuerySubstrings[0], substringRecordPositions[0][initialSubstringIndex]);
                var tempResult = new List<Token> { previousRecord };
                visitedSubstringTable[0][initialSubstringIndex] = true;

                for (int substringIndex = 1; substringIndex < querySubstringCount; substringIndex++)
                {
                    for (int occurenceIndex = 0; occurenceIndex < substringRecordPositions[substringIndex].Count; occurenceIndex++)
                    {
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
                                    searchSpace[previousRecordEndPosition..currentSubstringRecord],
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
                bool aCompleteResultWasFound = tempResult.Count == listofQuerySubstrings.Count * 2 - 1;
                if (aCompleteResultWasFound)
                    searchResults.Add(tempResult);
            }

            return searchResults;
        }
    }
}