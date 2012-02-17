namespace Bike.Parser
{
    public class SourceLocation
    {
        public string FilePath;
        public int Line;
        public int Column;

        public SourceLocation(string filePath, int line, int column)
        {
            FilePath = filePath;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", FilePath, Line, Column);
        }
    }
}
