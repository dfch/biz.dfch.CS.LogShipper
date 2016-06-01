﻿using System;
using System.Collections.Specialized;
using System.Linq;

namespace biz.dfch.CS.LogShipper.Public
{
    public interface ILogShipperOutput
    {
        NameValueCollection Configuration { get; set; }
        bool Log(String data);
        bool UpdateConfiguration(NameValueCollection configuration);
    }
}