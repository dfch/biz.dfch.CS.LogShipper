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
        private NameValueCollection configuration;
        public NameValueCollection Configuration
        {
            get
            {
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

        public bool Log(string data)
        {
            if (null == this.configuration)
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
    }
}