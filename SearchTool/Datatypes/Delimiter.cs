
namespace SearchTool.Datatypes
{
    public class Delimiter
    {
        private readonly string preDelimiter;
        private readonly string postDelimiter;

        public Delimiter(string preDelimiter, string postDelimiter) => (this.preDelimiter, this.postDelimiter) = (preDelimiter, postDelimiter);

        public string PreDelimiter { get => preDelimiter; }
        public string PostDelimiter { get => postDelimiter; }
    }
}
