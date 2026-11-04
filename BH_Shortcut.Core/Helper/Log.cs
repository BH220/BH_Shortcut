using BH_Shortcut.Core.Common;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Core.Helper
{
    public static class Log
    {
        private static ILoggerFactory? _loggerFactory;
        private static ILogger? _logger;

        public static void Configure(string projectName)
        {
            if (_loggerFactory == null)
            {
                NLog.LogManager.Configuration = CreateFallbackConfiguration();

                _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    builder.AddNLog();
                });
                _logger = _loggerFactory.CreateLogger(projectName);
            }
        }

        private static LoggingConfiguration CreateFallbackConfiguration()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = $"{FilePathHelper.LogRoot}/${{logger}}/log_${{logger}}_${{shortdate}}_00001.log",
                Layout = "${longdate} | ${level:uppercase=true} | ${message} ${exception:format=tostring}",
                ArchiveAboveSize = 104857600,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveSuffixFormat = "_{0:00000}",
                KeepFileOpen = false
            };
            var fileAsync = new AsyncTargetWrapper("fileAsync", fileTarget);

            var jsonTarget = new FileTarget("jsonFile")
            {
                FileName = $"{FilePathHelper.LogRoot}/${{logger}}/log_${{logger}}_${{shortdate}}_00001.json",
                ArchiveAboveSize = 104857600,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveSuffixFormat = "_{0:00000}",
                KeepFileOpen = false,
                Layout = new JsonLayout { IncludeEventProperties = true }
            };
            var jsonAsync = new AsyncTargetWrapper("jsonAsync", jsonTarget);

            config.AddTarget(fileAsync);
            config.AddTarget(jsonAsync);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, fileAsync, "*");
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, jsonAsync, "*");

            return config;
        }

        public static async void ImageTransfer(string message)
        {
            Console.WriteLine("ImageTransfer:" + message);
            if (_logger != null)
                _logger.LogTrace("{Message}", message);
        }

        public static async void Debug(string message)
        {
            Console.WriteLine("Debug:" + message);
            if (_logger != null)
                _logger.LogDebug("{Message}", message);
        }

        public static async void Info(string message)
        {
            Console.WriteLine("Info:" + message);
            if (_logger != null)
                _logger.LogInformation("{Message}", message);
        }

        public static async void Warn(string message)
        {
            Console.WriteLine("Warn:" + message);
            if (_logger != null)
                _logger.LogWarning("{Message}", message);
        }

        public static async void ErrorLog(string message, string endPoint = "", ulong? userNo = null, string bagId = "", Exception? ex = null)
        {
            string msg = $"Message: {message}, EndPoint: {endPoint}, UserNo: {(userNo == null ? "" : userNo.Value)}, BagId: {bagId}, Exception: {(ex == null ? "" : ex.Message)}";
            Console.WriteLine("Error:" + msg);
            if (_logger != null)
                _logger.LogError(ex, "{Message}", msg);
        }

        public static void Exception(Exception ex, string message)
        {
            ErrorLog(message, "", null, "", ex);
        }

        public static void Flush()
        {
            NLog.LogManager.Flush(TimeSpan.FromSeconds(1));

        }
    }
}
