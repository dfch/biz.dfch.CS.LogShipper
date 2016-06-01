/**
 * Copyright 2015-2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using biz.dfch.CS.LogShipper.Public;
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
                if (!VerifyAndSetConfiguration(value))
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
            catch(Exception ex)
            {
                fReturn = false;
            }
            return fReturn;
        }
    }
}
