using System;

namespace Mathos.Parser.Scripting
{
    public sealed class ScriptParserException : Exception
    {
        public ScriptParserException() : base()
        {
        }

        public ScriptParserException(string message) : base(message)
        {
        }

        public ScriptParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ScriptParserException(int line, Exception innerException) : base(innerException.GetType().Name + " on line " + line + ", " + Environment.NewLine + innerException.Message, innerException)
        {
        }
    }
}