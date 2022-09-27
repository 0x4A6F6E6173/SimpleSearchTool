using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchTool.Solver
{
    internal class HelperFunctions
    {
        // Since A new record has been visited then it's possible that other substring occurences have been made unviable.
        public static void UpdateVisitedTable(ref bool[][] visitedSubstringTable, in List<int>[] searchRecords, int updatedPosition)
        {
            for (int substringIndex = 0; substringIndex < visitedSubstringTable.Count(); substringIndex++)
            {
                for (int occurenceIndex = 0; occurenceIndex < visitedSubstringTable[substringIndex].Count(); occurenceIndex++)
                {
                    // This makes a lot of wasted work, but a viable improvement requires an improved matrix datastructure.
                    bool substringNoLongerViable = searchRecords[substringIndex][occurenceIndex] < updatedPosition;
                    visitedSubstringTable[substringIndex][occurenceIndex] = substringNoLongerViable;
                }
            }
        }
    }
}
