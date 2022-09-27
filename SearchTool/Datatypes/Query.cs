namespace SearchTool.Datatypes
{
    public record Query
    {
        private readonly string searchSpace;
        private readonly string searchQuery;

        public Query(string searchSpace, string searchQuery)
        {
            this.searchSpace = searchSpace;
            this.searchQuery = searchQuery;
        }

        public string SearchSpace { get => searchSpace; }
        public string SearchQuery { get => searchQuery; }
    }
}
