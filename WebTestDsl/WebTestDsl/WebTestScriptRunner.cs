using System.IO;
using System.Linq;
using WebTestDsl.Scripting;

namespace WebTestDsl
{
    public class WebTestScriptRunner
    {
        private readonly ScriptCsHost _scriptHost = new ScriptCsHost();

        public void RunCsxScript(string scriptPath, params string[] scriptArgs)
        {
            _scriptHost.LoadFile(scriptPath, scriptArgs);
            _scriptHost.Execute();
        }

        public void RunDslScript(string scriptPath)
        {
            var scriptCsCode = DSLParser.ToCSharp(File.ReadAllLines(scriptPath)).Aggregate((x, y) => x + "\r\n" + y);
            _scriptHost.Load(scriptCsCode);
            _scriptHost.Execute();
        }
    }
}