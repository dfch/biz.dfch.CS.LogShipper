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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using biz.dfch.CS.LogShipper.Public;

namespace biz.dfch.CS.LogShipper.Extensions
{
    [Export(typeof(ILogShipperOutput))]
    [ExportMetadata("Name", "NullOutput")]
    public class NullOutput : ILogShipperOutput
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
                var fReturn = VerifyAndSetConfiguration(value);
                if (!fReturn)
                {
                    throw new ArgumentNullException("Configuration", "NullOutput.Configuration: Parameter validation FAILED. Parameter must not be null.");
                }
            }
        }

        public bool Log(string data)
        {
            if (null == _configuration)
            {
                throw new ArgumentNullException("Configuration", "NullOutput.Configuration: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data", "data: Parameter validation FAILED. Parameter must not be null or empty.");
            }
            return true;
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
