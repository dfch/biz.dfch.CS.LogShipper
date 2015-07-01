using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using biz.dfch.CS.LogShipper.Contracts;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperOutput))]
    [ExportMetadata("Name", "DefaultConsoleOutput")]
    public class DefaultConsoleOutput : ILogShipperOutput
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

        public bool Log(string data)
        {
            if (null == this.context)
            {
                throw new ArgumentNullException("Context", "Context: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("Data", "Data: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            Console.WriteLine(String.Format("{0:yyyy-MM-dd HH:mm:ss.fffzzz}: {1}", DateTime.UtcNow, data));
            return true;
        }
    }
}