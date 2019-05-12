namespace Indicium.Tokens
{
    public struct Line
    {
        public string Text { get; }

        public Line(string text)
        {
            Text = text.Trim();
        }
    }
}