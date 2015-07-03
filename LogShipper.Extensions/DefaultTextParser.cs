using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using biz.dfch.CS.LogShipper.Contracts;
using System.Collections.Specialized;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperParser))]
    [ExportMetadata("Name", "DefaultTextParser")]
    public class DefaultTextParser : ILogShipperParser
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
                if (VerifyAndSetConfiguration(value))
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
        }

        public List<String> Parse(String data)
        {
            if(null == _configuration)
            {
                throw new ArgumentNullException("Configuration", "Configuration: Parameter validation FAILED. Parameter must not be null or empty.");
            }

            _offsetParsed = 0;
            var list = new List<String>();

            if (String.IsNullOrEmpty(data))
            {
                list.Clear();
                return list;
            }
            var normalisedString = String.Empty;
            if(32 * 1024 < data.Length)
            {
                var sb = new StringBuilder(data);
                sb = sb.Replace("\r\n", "\n").Replace("\r", "\n");
                normalisedString = sb.ToString();
                sb.Clear();
            }
            else
            {
                normalisedString = data.Replace("\r\n", "\n").Replace("\r", "\n");
            }
            var strings = normalisedString.Split(System.Environment.NewLine.ToCharArray());
            list = strings.ToList();
            list.RemoveAt(list.Count-1);
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
