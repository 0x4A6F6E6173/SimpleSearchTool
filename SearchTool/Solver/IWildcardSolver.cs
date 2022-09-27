using SearchTool.Datatypes;

namespace SearchTool.Solver
{

    // Given an array of lists of searchRecords hf. substringSearchRecords and
    // given a  list  of lists of searchRecords hf. searchResults then:
    //      Add the first viable substring record, based on initial position, to a new list in searchRecords.
    //      Iterate through searchRecords starting from index 1 where at each iteration do:
    //          Add the first substring record which initial position is:
    //              1. The lowest initial position which,
    //              2. is still greater than the (initial position of the previous search record
    //                  + size of the substringQuery of the previous record), and
    //              3. The substring has not been used before.
    // NOTE: substringSearchRecord has the property that the occurences of each substring will be ordered in terms
    //       position in the searchSpaces as encountered.
    // NOTE: This could be made much more efficent with another matrix data structure by minimizing for-loops.
    public interface IWildcardSolver
    {
        public abstract List<List<Token>> Solve(string searchSpace, in List<string> listofQuerySubstrings, in List<int>[] substringRecordPositions);
    }
}
