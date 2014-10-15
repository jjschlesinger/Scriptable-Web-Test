using System;
using System.IO;
using Common.Logging;
using Common.Logging.Simple;
using HttpFacade;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using ScriptCs.Hosting;
using LogLevel = ScriptCs.Contracts.LogLevel;

namespace WebTestDsl.Scripting
{
    internal class ScriptCsHost
    {
        private readonly string[] _importPaths;
        private readonly IScriptPack[] _scriptPacks;
        private string _scriptCsCode;
        private string _scriptCsxPath;
        private string[] _scriptArgs;
        private readonly ILog _logger;

        public ScriptCsHost(ILog logger = null, string[] importPaths = null, IScriptPack[] scriptPacks = null)
        {
            _importPaths = importPaths ?? new String[] { };
            _scriptPacks = scriptPacks ?? new IScriptPack[] { };
            _logger = logger ?? new ConsoleOutLogger("ScriptCsHost", Common.Logging.LogLevel.Info, true, true, true, "YYYY-MM-DD HH:MM:SS");
        }

        public void Load(string scriptCsCode, params string[] scriptArgs)
        {
            _scriptCsCode = scriptCsCode;
            _scriptArgs = scriptArgs;
            _scriptCsxPath = null;
        }

        public void LoadFile(string scriptCsxPath, params string[] scriptArgs)
        {
            _scriptCsxPath = scriptCsxPath;
            _scriptArgs = scriptArgs;
            _scriptCsCode = null;
        }

        public void Execute()
        {
            var scriptServicesBuilder = new ScriptServicesBuilder(new ScriptConsole(), _logger)
                .LogLevel(LogLevel.Info)
                .Repl(false)
                .ScriptEngine<RoslynScriptEngine>();

            var svc = scriptServicesBuilder.Build();
            svc.Executor.Initialize(_importPaths, _scriptPacks);
            svc.Executor.AddReferenceAndImportNamespaces(new[] { typeof(Http) });
            ScriptResult result;
            if(_scriptCsCode != null)
                result = svc.Executor.ExecuteScript(_scriptCsCode, _scriptArgs);
            else if(_scriptCsxPath != null)
                result = svc.Executor.Execute(_scriptCsxPath, _scriptArgs);
            else
                throw new InvalidOperationException("Neither script path nor script code were set");

            svc.Executor.Terminate();

            if(result.CompileExceptionInfo != null)
                result.CompileExceptionInfo.Throw();

            if (result.ExecuteExceptionInfo != null)
                result.ExecuteExceptionInfo.Throw();
        }
    }
}
