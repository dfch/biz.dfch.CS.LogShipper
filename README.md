# biz.dfch.CS.LogShipper
Windows Service that forwards log files to a target via SYSLOG, GELF or other protocols.

With this service you can read any log files or other sources, process them and send them to arbitrary destinations via pre-defined or customer defined plugins.

Currently supported inputs/parsers:

* DefaultTextParser
* PowerShellParser

Currently supported outputs:

* ConsoleOutput
* NullOutput

All plugins are loaded via the [Microsoft Extensibility Framework](https://mef.codeplex.com/) and must adhere to the following interfaces:

``` csharp

public interface ILogShipperParser
{
  NameValueCollection Configuration { get; set; }
  UInt32 OffsetParsed { get; }
  List<String> Parse(String data);
  bool UpdateConfiguration(NameValueCollection configuration);
}

public interface ILogShipperOutput
{
  NameValueCollection Configuration { get; set; }
  bool Log(String data);
  bool UpdateConfiguration(NameValueCollection configuration);
}
```

Assembly: biz.dfch.CS.LogShipper

d-fens GmbH, General-Guisan-Strasse 6, CH-6300 Zug, Switzerland

## Download

* See [Releases](https://github.com/dfch/biz.dfch.CS.LogShipper/releases) and [Tags](https://github.com/dfch/biz.dfch.CS.LogShipper/tags) on [GitHub](https://github.com/dfch/biz.dfch.CS.LogShipper)
