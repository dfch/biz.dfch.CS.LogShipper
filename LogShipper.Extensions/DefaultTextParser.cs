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

        // DFTODO Implement OffsetParsed
        private UInt32 offsetParsed;
        public UInt32 OffsetParsed
        {
            get
            {
                return offsetParsed;
            }
        }

        public List<String> Parse(String data)
        {
            if(null == this.configuration)
            {
                throw new ArgumentNullException("Configuration", "Configuration: Parameter validation FAILED. Parameter must not be null or empty.");
            }

            offsetParsed = 0;
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

        public bool RefreshContext()
        {
            // DFTODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
