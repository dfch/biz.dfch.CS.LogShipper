using System;
using System.Collections.Specialized;
using System.Linq;

namespace biz.dfch.CS.LogShipper.Contracts
{
    public interface ILogShipperOutput
    {
        NameValueCollection Configuration { get; set; }
        bool Log(String data);
    }
}
