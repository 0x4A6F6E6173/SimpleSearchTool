namespace SearchTool.Datatypes
{
    public record RecordToken : Token
    {
        public RecordToken(string value, int position) : base(value, position) { }
    }
}
