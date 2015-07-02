﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace biz.dfch.CS.LogShipper.Contracts
{
    public interface ILogShipperParser
    {
        NameValueCollection Configuration { get; set; }
        UInt32 OffsetParsed { get; }
        List<String> Parse(String data);
        bool RefreshContext();
    }

}
