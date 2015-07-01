using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.LogShipper.Contracts
{
    public interface ILogShipperParser
    {
        Object Context { get; set; }
        String Data { get; set;}
        List<String> Parse(String data);
        bool RefreshContext();
    }

}
