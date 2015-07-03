using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using biz.dfch.CS.LogShipper.Contracts;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperOutput))]
    [ExportMetadata("Name", "DefaultConsoleOutput")]
    public class DefaultConsoleOutput : ILogShipperOutput
    {
        private NameValueCollection _configuration;
        public NameValueCollection Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException("Configuration", "DefaultConsoleOutput.Configuration: Parameter validation FAILED. Parameter must not be null.");
                }
                _configuration = value;
            }
        }

        public bool Log(string data)
        {
            if (null == _configuration)
            {
                throw new ArgumentNullException("Configuration", "Configuration: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data", "data: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            Console.WriteLine(String.Format("{0:yyyy-MM-dd HH:mm:ss.fffzzz}: {1}", DateTime.Now, data));
            return true;
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
                _configuration = new NameValueCollection(configuration);
                fReturn = true;
            }
            catch
            {
                fReturn = false;
            }
            return fReturn;
        }
    }
}