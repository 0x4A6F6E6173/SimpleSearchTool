namespace SearchTool.Datatypes
{
    public record WildcardToken : Token
    {
        public WildcardToken(string value, int position) : base(value, position) { }
    }
}
