using NUnit.Framework;
using SearchTool;
using SearchTool.Datatypes;
using SearchTool.Solver;

namespace TestSearchTool
{
    /*
     * Would it be better to use substring instead of hardcoding the strings and their position?
     */

    [TestFixture]
    public class Search_TestFindString
    {
        [Test]
        public void FindString_QueryNotContainedInSearchSpace()
        {
            var searchSpace = "21+e921ur24hgfwneodjqlnv+r4039t530yt834";
            var searchQuery = " ";
            var query = new Query(searchSpace, searchQuery);
            var expectedSearchResultList = new List<Token>();

            var actualResult = Search.FindString(query);

            Assert.That(actualResult, Is.EqualTo(expectedSearchResultList));
        }

        [Test]
        public void FindString_SingleCharacter_QueryContainedInSearchSpaceOnce()
        {
            var searchSpace = "21+e921ur24hgfwneodjqlnv+r4039t530yt834";
            var searchQuery = "f";
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<Token> {
                new RecordToken(searchQuery, 13)
            };

            var actualResult = Search.FindString(query);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void FindString_SingleCharacter_QueryContainedInSearchSpaceMultipleTimes()
        {
            var searchSpace = "21+e921ur24hgfwneodjqlnv+r4039t530yt834";
            var searchQuery = "+";
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<Token> {
                new RecordToken(searchQuery, 2),
                new RecordToken(searchQuery, 24)
            };

            var actualResult = Search.FindString(query);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void FindString_String_QueryContainedInSearchSpaceOnce()
        {
            var searchSpace = "21+e921ur24hgfwneodjqlnv+r4039t530yt834";
            var searchQuery = "ur24";
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<Token> {
                 new RecordToken(searchQuery, 7)
            };

            var actualResult = Search.FindString(query);

            Assert.That(actualResult, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public void FindString_String_QueryContainedInSearchSpaceMultipleTimes()
        {
            var searchSpace = "21+e921ur24hgfwneodjqlnv+r4039t530yt834";
            var searchQuery = "21";
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<Token> {
                new RecordToken(searchQuery, 0),
                new RecordToken(searchQuery, 5)
            };

            var actualResult = Search.FindString(query);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void FindString_QueryPrefixFoundAtTheEndOfSearchSpace()
        {
            var searchSpace = "21+e921ur24hgfwneodjqlnv+r4039t530yt834";
            var searchQuery = "3472";
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<Token>();

            var actualResult = Search.FindString(query);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }

    [TestFixture]
    public class Search_TestGenericWildcardSearch
    {
        string wildcardString = Search.wildcardToken;

        [Test]
        public void GenericWildcardSearch_SearchForFullTheSearchSpace_WithoutWildcard()
        {
            var searchSpace = "searchSpaceString";
            var searchQuery0 = searchSpace;
            var query = new Query(searchSpace, searchQuery0);
            var expectedResult = new List<List<Token>> {
                new List<Token> { new RecordToken(searchSpace, 0) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchForASubstringInSearchSpace_WithoutWildcard()
        {
            var searchSpace = "searchSpaceString";
            var searchQuery0 = "Space";
            var query = new Query(searchSpace, searchQuery0);
            var expectedResult = new List<List<Token>> {
                new List<Token> { new RecordToken(searchQuery0, 6) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchForSomeString_SearchspaceContainsWildcardOnly()
        {
            var searchSpace = wildcardString;
            var searchQuery0 = "someString";
            var query = new Query(searchSpace, searchQuery0);
            var expectedResult = new List<List<Token>>();

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchWithWildcardOnly_EmptySearchspace()
        {
            var searchSpace = "";
            var searchQuery0 = wildcardString;
            var query = new Query(searchSpace, searchQuery0);
            var expectedResult = new List<List<Token>>();

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchWithoutWildcard_FindString()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var searchQuery0 = "ipsum";
            var query = new Query(searchSpace, searchQuery0);
            var expectedResult = new List<List<Token>> {
                new List<Token> { new RecordToken(searchQuery0, 6) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchWithoutWildcard_DontFindString()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var searchQuery0 = "adipiscing";
            var query = new Query(searchSpace, searchQuery0);
            var expectedResult = new List<List<Token>>();

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchWithSingleWildcard()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var querySubstring0 = "m i";
            var querySubstring1 = "et. ";
            var wildcardToken0 = "psum dolor sit am";
            var searchQuery = querySubstring0 + wildcardString + querySubstring1;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>> {
                 new List<Token> {
                     new RecordToken(querySubstring0, 4),
                     new WildcardToken(wildcardToken0, 7),
                     new RecordToken("et. ", 24) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchWithMultipleWildcards()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var querySubstring0 = "lo";
            var querySubstring1 = "i";
            var querySubstring2 = "et";
            var wildcardToken0 = "rem ";
            var wildcardToken1 = "psum dolor sit am";
            var searchQuery = querySubstring0 + wildcardString + querySubstring1 + wildcardString + querySubstring2;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>> {
                new List<Token> {
                    new RecordToken(querySubstring0, 0),
                    new WildcardToken(wildcardToken0, 2),
                    new RecordToken(querySubstring1, 6),
                    new WildcardToken(wildcardToken1, 7),
                    new RecordToken(querySubstring2, 24) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_WildcardPlacedAtTheStartOfSearchQuery()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var querySubstring0 = "dolor";
            var searchQuery = wildcardString + querySubstring0;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>> {
                new List<Token> { new RecordToken(querySubstring0, 12) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_WildcardPlacedAtTheEndOfSearchQuery()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var querySubstring0 = "dolor";
            var searchQuery = querySubstring0 + wildcardString;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>> {
                new List<Token> { new RecordToken(querySubstring0, 12) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_TwoWildcardsPlacedConsecutively()
        {
            var searchSpace = "lorem ipsum dolor sit amet. Consectetuloer";
            var querySubstring0 = "lorem";
            var querySubstring1 = "dolor";
            var wildcardToken0 = " ipsum ";
            var searchQuery = querySubstring0 + wildcardString + wildcardString + querySubstring1;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>> {
                new List<Token> {
                    new RecordToken(querySubstring0, 0),
                    new WildcardToken(wildcardToken0, 5),
                    new RecordToken(querySubstring1, 12) }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchqueryOverlapsInSearchSpace()
        {
            var searchSpace = "someaningmeaningoodgood";
            var querySubstring0 = "some";
            var querySubstring1 = "meaning";
            var querySubstring2 = "good";
            var wildcardToken0 = "aning";
            var wildcardToken1 = "ood";
            var searchQuery = querySubstring0 + wildcardString + querySubstring1 + wildcardString + querySubstring2;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>> {
                new List<Token> { new RecordToken(querySubstring0, 0),
                                  new WildcardToken(wildcardToken0, 4),
                                  new RecordToken(querySubstring1, 9),
                                  new WildcardToken(wildcardToken1, 16),
                                  new RecordToken(querySubstring2, 19)
                }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GenericWildcardSearch_SearchQueryFromPractice()
        {
            var searchSpace = "1. A;sb. ";
            var querySubstring = ";";
            var wildcardToken0 = "1. A";
            var wildcardToken1 = "sb. ";
            var searchQuery = wildcardString + querySubstring + wildcardString;
            var query = new Query(searchSpace, searchQuery);
            var expectedResult = new List<List<Token>>
            {
                new List<Token>
                {
                    new WildcardToken(wildcardToken0, 0),
                    new RecordToken(querySubstring, 4),
                    new WildcardToken(wildcardToken1, 5)
                }
            };

            var actualResult = Search.WildcardSearch(query, new GenericWildcardSolver());

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }

    [TestFixture]
    public class Search_TestFindStrings
    {
        [Test]
        public void Ttest()
        {

        }
    }

    [TestFixture]
    public class Search_TestSemanticWildcardSearch
    {
        [Test]
        public void Ttest()
        {

        }
    }
}