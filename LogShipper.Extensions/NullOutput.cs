using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using biz.dfch.CS.LogShipper.Contracts;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperOutput))]
    [ExportMetadata("Name", "NullOutput")]
    public class NullOutput : ILogShipperOutput
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
                var fReturn = VerifyAndSetConfiguration(value);
                if (!fReturn)
                {
                    throw new ArgumentNullException("Configuration", "NullOutput.Configuration: Parameter validation FAILED. Parameter must not be null.");
                }
            }
        }

        public bool Log(string data)
        {
            if (null == _configuration)
            {
                throw new ArgumentNullException("Configuration", "NullOutput.Configuration: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data", "data: Parameter validation FAILED. Parameter must not be null or empty.");
            }
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
            catch(Exception ex)
            {
                fReturn = false;
            }
            return fReturn;
        }
    }
}