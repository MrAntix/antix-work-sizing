﻿using System;

namespace Antix.Logging
{
    public static class LoggerExtensionsFatal
    {
        public static void Fatal(
            this ILogAdapter logAdapter,
            IFormatProvider formatProvider, Func<LogMessageDelegate, string> getMessage,
            Exception ex)
        {
            if (logAdapter == null) return;

            logAdapter.Log(LogLevel.Fatal, formatProvider, getMessage, ex);
        }

        public static void Fatal(
            this ILogAdapter logAdapter,
            IFormatProvider formatProvider, Func<LogMessageDelegate, string> getMessage)
        {
            if (logAdapter == null) return;

            logAdapter.Log(LogLevel.Fatal, formatProvider, getMessage, null);
        }

        public static void Fatal(
            this ILogAdapter logAdapter,
            Func<LogMessageDelegate, string> getMessage)
        {
            if (logAdapter == null) return;

            logAdapter.Log(LogLevel.Fatal, null, getMessage, null);
        }

        public static void Fatal(
            this ILogAdapter logAdapter,
            Func<LogMessageDelegate, string> getMessage, Exception ex)
        {
            if (logAdapter == null) return;

            logAdapter.Log(LogLevel.Fatal, null, getMessage, ex);
        }
    }
}