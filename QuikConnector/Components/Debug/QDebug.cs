namespace QuikConnector.Components.Debug
{
    class QDebug
    {
        const string MARKER = "qdebug";
        public static bool Enabled = false;
        public static void Output(string text, string mark = MARKER)
        {
            if (Enabled)
            {
                System.Diagnostics.Debug.WriteLine((mark.Length > 0 ? mark + ": " : "") + text);
            }
        }
        public static void OutputF(string text, string mark = MARKER)
        {
            System.Diagnostics.Debug.WriteLine((mark.Length > 0 ? mark + ": " : "") + text);
        }
    }
}
