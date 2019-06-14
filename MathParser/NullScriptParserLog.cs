using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathos.Parser
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
