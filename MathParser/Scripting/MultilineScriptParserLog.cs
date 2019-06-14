using System;
using System.Text;

namespace Mathos.Parser.Scripting
{
    /// <summary>
    /// An implementation of <see cref="IScriptParserLog"/> that appends logs to a multiline string
    /// </summary>
    public class MultilineScriptParserLog : IScriptParserLog
    {
        private StringBuilder sb;
        public MultilineScriptParserLog()
        {
            sb = new StringBuilder();
        }
        public string Output { get { return sb.ToString(); } }
        public void Log(string log)
        {
            if (Output.Length > 0)
            {
                sb.Append(Environment.NewLine);
            }
            sb.Append(log);
        }

        public void Clear()
        {
            sb.Clear();
        }
    }
}
