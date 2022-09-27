namespace SearchTool.Datatypes
{
    public abstract record Token : IFormattable
    {
        public Token(string value, int position)
        {
            this.value = value;
            this.position = position;
        }

        public string value { get; init; }
        public int position { get; init; }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return String.Format("Position: {0}, Value: {1}", position, value);
        }
    }
}
