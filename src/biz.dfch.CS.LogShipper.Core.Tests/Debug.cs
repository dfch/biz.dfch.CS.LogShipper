
using System;
namespace biz.dfch.CS.LogShipper.Core.Tests
{
    public class Debug : LogBase
    {
        public static void WriteLine(string message, params object[] args)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat(message, args);
        }
        public static void WriteLine(string message, string category)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("{0}|{1}", category, message);
        }
        public static void WriteLine(string message)
        {
            if (Log.IsDebugEnabled) Log.Debug(message);
        }
        public static void WriteLine(object value, string category)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("{0}|{1}", category, value.ToString());
        }
        public static void WriteLine(object value)
        {
            if (Log.IsDebugEnabled) Log.Debug(value.ToString());
        }
        public static void WriteLine(Exception ex)
        {
            if (Log.IsDebugEnabled) WriteException(ex);
        }
        public static void Write(string message, string category)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("{0}|{1}", category, message);
        }
        public static void Write(string message)
        {
            if (Log.IsDebugEnabled) Log.Debug(message);
        }
        public static void Write(object value, string category)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("{0}|{1}", category, value.ToString());
        }
        public static void Write(object value)
        {
            if (Log.IsDebugEnabled) Log.Debug(value.ToString());
        }
        public static void Assert(bool condition, string message, string detailMessage)
        {
            System.Diagnostics.Debug.Assert(condition, message, detailMessage);
        }
        public static void Assert(bool condition, string message)
        {
            System.Diagnostics.Debug.Assert(condition, message);
        }
        public static void Assert(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition);
        }
    }
}

/**
 *
 *
 * Copyright 2015 Ronald Rink, d-fens GmbH
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
 *
 */
