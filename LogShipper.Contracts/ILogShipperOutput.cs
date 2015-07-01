using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.LogShipper.Contracts
{
    public interface ILogShipperOutput
    {
        Object Context { get; set;}
        bool Log(String data);
    }
}
