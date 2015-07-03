using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using biz.dfch.CS.LogShipper.Contracts;
using System.Management.Automation;
using System.IO;
using System.Collections.Specialized;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperParser))]
    [ExportMetadata("Name", "PowerShellParser")]
    public class PowerShellParser : ILogShipperParser
    {
        readonly PowerShell _ps = PowerShell.Create();

        private String _scriptFile;
        private NameValueCollection _configuration;
        public NameValueCollection Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (!VerifyAndSetConfiguration(value))
                {
                    throw new ArgumentNullException("Configuration", "DefaultTextParser.Configuration: Parameter validation FAILED. Parameter must not be null.");
                }
            }
        }

        // DFTODO Implement OffsetParsed
        private UInt32 _offsetParsed;
        public UInt32 OffsetParsed
        {
            get
            {
                return _offsetParsed;
            }
            set
            {
                _offsetParsed = value;
            }
        }

        public List<String> Parse(String data)
        {
            if (null == _configuration)
            {
                throw new ArgumentNullException("Configuration", "PowerShellParser.Configuration: Parameter validation FAILED. Parameter must not be null.");
            }
            _offsetParsed = 0;
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
        public bool UpdateConfiguration(NameValueCollection configuration)
        {
            return VerifyAndSetConfiguration(configuration);
        }
        private bool VerifyAndSetConfiguration(NameValueCollection configuration)
        {
            var fReturn = false;
            try
            {
                if (null == configuration)
                {
                    throw new ArgumentNullException("configuration", "configuration: Parameter validation FAILED. Parameter must not be null.");
                }
                var scriptFile = configuration.Get("ScriptFile");
                if (String.IsNullOrWhiteSpace(scriptFile))
                {
                    throw new ArgumentNullException("ScriptFile", "PowerShellParser.Configuration: Parameter validation FAILED. ScriptFile must not be null.");
                }

                if (null == scriptFile || String.IsNullOrWhiteSpace(scriptFile)) throw new ArgumentNullException("scriptFile", String.Format("scriptFile: Parameter validation FAILED. Parameter cannot be null or empty."));
                if (String.IsNullOrWhiteSpace(Path.GetDirectoryName(scriptFile)))
                {
                    scriptFile = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(scriptFile));
                }
                var achFileName = Path.GetFileName(scriptFile).ToCharArray();
                var achFileInvalidChars = Path.GetInvalidFileNameChars();
                if ('\0' != achFileName.Intersect(achFileInvalidChars).FirstOrDefault())
                {
                    throw new ArgumentException("ScriptFile: Parameter validation FAILED. ScriptFile name contains invalid characters.", scriptFile);
                }
                if (!File.Exists(scriptFile)) throw new FileNotFoundException(String.Format("scriptFile: Parameter validation FAILED. File '{0}' does not exist.", scriptFile), scriptFile);

                _configuration = new NameValueCollection(configuration);
                _scriptFile = scriptFile;

                fReturn = true;
            }
            catch(Exception ex)
            {
                fReturn = false;
            }
            return fReturn;
        }
    }
}
