using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using biz.dfch.CS.LogShipper.Contracts;
using System.Management.Automation;
using System.IO;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperParser))]
    [ExportMetadata("Name", "PowerShellParser")]
    public class PowerShellParser : ILogShipperParser
    {
        readonly PowerShell _ps = PowerShell.Create();

        private String _scriptFile;

        private Object context;
        public Object Context
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
            }
        }

        private String data;
        public String Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public List<String> Parse(String data)
        {
            if (null == this.context)
            {
                throw new ArgumentNullException("Context", "Context: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            if (String.IsNullOrWhiteSpace(this.data))
            {
                throw new ArgumentNullException("Data", "Data: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            var list = new List<String>();

            if (String.IsNullOrEmpty(data))
            {
                list.Clear();
                return list;
            }

            _ps.Commands.Clear();
            _ps
                .AddScript(String.Format("[System.Diagnostics.Trace]::WriteLine('{0}')", data))
                .AddScript(String.Format("return @('and the result is: {0}')", data))
                .AddCommand(_scriptFile)
                .AddParameter("InputObject", data)
            ;
            var results = _ps
                .Invoke();
            foreach (var result in results)
            {
                var line = result.BaseObject.ToString();
                System.Diagnostics.Trace.WriteLine(String.Format("result: '{0}'", line));
                list.Add(line);
            }
            
            return list;
        }
        public bool RefreshContext()
        {
            var externalParameterScriptFile = "";
            if (null == externalParameterScriptFile || String.IsNullOrWhiteSpace(externalParameterScriptFile)) throw new ArgumentNullException("externalParameterScriptFile", String.Format("externalParameterScriptFile: Parameter validation FAILED. Parameter cannot be null or empty."));
            if (String.IsNullOrWhiteSpace(Path.GetDirectoryName(externalParameterScriptFile)))
            {
                externalParameterScriptFile = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(externalParameterScriptFile));
            }
            var achFileName = Path.GetFileName(externalParameterScriptFile).ToCharArray();
            var achFileInvalidChars = Path.GetInvalidFileNameChars();
            if ('\0' != achFileName.Intersect(achFileInvalidChars).FirstOrDefault())
            {
                throw new ArgumentException("ScriptFile: Parameter validation FAILED. ScriptFile name contains invalid characters.", externalParameterScriptFile);
            }
            if (!File.Exists(externalParameterScriptFile)) throw new FileNotFoundException(String.Format("externalParameterScriptFile: Parameter validation FAILED. File '{0}' does not exist.", externalParameterScriptFile), externalParameterScriptFile);
            _scriptFile = externalParameterScriptFile;

            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
