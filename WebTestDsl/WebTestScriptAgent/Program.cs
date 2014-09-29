using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTestDsl;

namespace WebTestScriptAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = Args.Configuration.Configure<ScriptExecuteCommand>().CreateAndBind(args);
            var runner = new WebTestScriptRunner();
            if(cmd.Type.Equals("csx", StringComparison.OrdinalIgnoreCase))
                runner.RunCsxScript(cmd.Path);
            else
                runner.RunDslScript(cmd.Path);
        }
    }
}
