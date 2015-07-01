using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using biz.dfch.CS.LogShipper.Contracts;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperParser))]
    [ExportMetadata("Name", "DefaultTextParser")]
    public class DefaultTextParser : ILogShipperParser
    {
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
            if(null == this.context)
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
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
