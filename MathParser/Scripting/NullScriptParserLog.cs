namespace Mathos.Parser.Scripting
{
    /// <summary>
    /// An implementation of <see cref="IScriptParserLog"/> that doesn't do anything with logs
    /// </summary>
    public sealed class NullScriptParserLog : IScriptParserLog
    {
        public void Log(string log)
        {
            //don't do anything with logs in this class
        }
    }
}
