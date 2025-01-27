﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/4/10 23:37:38
 | 主要用途：
 | 更改记录：
 |			 时间		版本		更改
 */
namespace LoongEgg.LoongLogger
{
    // TODO: 编译的时候不要Release不然不会有Debug版本
    /// <summary>
    /// Logger调度器，编译的时候不要Release不然不会有Debug版本
    /// </summary>
    public class Logger
    {
        static List<BaseLogger> Loggers = new List<BaseLogger>();

        /*/// <summary>
        /// 打开所有的Logger记录器
        /// </summary>
        /// <param name="level"><see cref="LoggerType"/></param>
        public static void EnableAll(LoggerLevel level = LoggerLevel.Debug) {
            Enable(LoggerType.Console | LoggerType.Debug | LoggerType.File, level);
        }*/

        // TODO: 09-B 注册Logger
        /// <summary>
        /// 使能各个Logger
        /// </summary>
        /// <param name="type">需要开启的Logger类型，可以使用“|”位域操作</param>
        /// <param name="level">开启的Logger的级别</param> 
        /// <param name="logPath">指定log存放位置</param> 
        /// <param name="maxCapacity">最大日志数量</param> 
        /// <example>
        ///     // 开启调试输出和控制台的Logger，消息级别为Error
        ///     LoggerManager.Enable(LoggerType.Debug | LoggerType.Console,  LoggerLevel.Error);
        /// </example>
        /// <code>
        ///     LoggerManager.Enable(LoggerType.Debug | LoggerType.Console,  LoggerLevel.Error);
        /// </code>
        public static void Enable(LoggerType type, LoggerLevel level = LoggerLevel.Debug, string logPath = null, int maxCapacity = 100)
        {
            Loggers.Clear();

            if (type.HasFlag(LoggerType.Console))
                Loggers.Add(new ConsoleLogger(level));

            if (type.HasFlag(LoggerType.Debug))
                Loggers.Add(new DebugLogger(level));

            if (type.HasFlag(LoggerType.File))
            {
                Loggers.Add(new FileLogger(logPath, level: level));
                //WriteCritical("Logger File is Created... Check at this path: [ROOT_of_Your_Application]/log/", false);
            }

            if (type.HasFlag(LoggerType.Memory))
            {
                Loggers.Add(new MemoryLogger(level,maxCapacity));
            }    

            WriteInfor(
                //Environment.NewLine
                "Log Start...\r\n"
                + "".ToHeader(120)
                + "  ".ToContent2(120)
                + "快易享".ToContent2(120)
                + "项目发布地址 @ https://445720.xyz and https://github.com/44578287".ToContent2(120)
                + "ck呵呵 QQ:2407896713 mail:g9964957@gmail.com".ToContent2(120)
                + "  ".ToContent2(120)
                + "".ToHeader(120),
                false);
        }

        // TODO: 09-C 销毁Logger
        /// <summary>
        /// 销毁所有的Logger
        /// </summary>
        public static void Disable()
        {
            //Loggers.ForEach(
            //    log =>
            //    {
            //        if (log.GetType() == typeof(FileLogger))
            //        {
            //            WriteInfor("", false);
            //            WriteCritical("Logger File is Saved... Check at this path: [ROOT_of_Your_Application]/log/", true);
            //        }

            //    }
            //);
            WriteInfor(
                /*Environment.NewLine
                +*/
                "Log End...\r\n"
                + "".ToHeader(120)
                + "  ".ToContent2(120)
                + "Log记录结束".ToContent2(120)
                + "快易享".ToContent2(120)
                + "ck呵呵 QQ:2407896713 mail:g9964957@gmail.com".ToContent2(120)
                + "  ".ToContent2(120)
                + " Good  Luck ".ToHeader(120),
                false);
            Loggers.Clear();
        }

        private static readonly object _Lock = new object();

        /// <summary>
        /// 日志变化事件
        /// </summary>
        public static Action<Log> OnLogChanged 
        {
            get => MemoryLogger.OnLogChanged;
            set => MemoryLogger.OnLogChanged = value;
        }

        /// <summary>
        /// 获取所有的日志
        /// </summary>
        /// <returns></returns>
        public static List<Log> GetLogs() => MemoryLogger.GetAllLogs();

        /// <summary>
        /// 获取指定类型的日志
        /// </summary>
        /// <param name="type">日志等级</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByType(MessageType type) => MemoryLogger.GetLogsByType(type);

        /// <summary>
        /// 按时间范围筛选日志
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByTimeRange(DateTime startTime, DateTime endTime) => MemoryLogger.GetLogsByTimeRange(startTime, endTime);

        /// <summary>
        /// 按调用方法名筛选日志
        /// </summary>
        /// <param name="callerName">调用方法名</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByCallerName(string callerName) => MemoryLogger.GetLogsByCallerName(callerName);

        /// <summary>
        /// 清空所有的日志
        /// </summary>
        public static void ClearLogs() => MemoryLogger.ClearLogs();

        // TODO: 09-D 打印日志 WriteDebug, WriteInfo, WriteError, WriteFatal
        /// <summary>
        /// 打印一条新的日志消息
        /// </summary>
        ///     <param name="type">消息类型</param>
        ///     <param name="message">消息的具体内容</param>
        ///     <param name="isDetailMode">详细模式？</param>
        ///     <param name="callerName">调用的方法的名字</param>
        ///     <param name="fileName">调用方法所在的文件名</param>
        ///     <param name="line">调用代码所在行</param>
        ///     <param name="TaskName">任务名</param>
        ///     <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        private static bool WriteLine
            (
                MessageType type,
                string message,
                bool isDetailMode,
                string callerName,
                string fileName,
                int line,
                string TaskName = null,
                string TaskID = null
            )
        {
            bool isWrited = false;

            // TODO: 2020-04-26 增加了线程锁
            lock (_Lock)
            {
                string msg = BaseLogger.FormatMessage(type, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

                if (Loggers.Any())
                {
                    isWrited = true;
                    Loggers.ForEach(logger => 
                    {
                        if (logger.GetType() == typeof(MemoryLogger))
                        {
                            var MemoryLogger = (MemoryLogger)logger;

                            isWrited &= MemoryLogger.WriteLine(type, message, callerName, fileName, line);
                        }
                        else
                            isWrited &= logger.WriteLine(msg, type);
                    });
                }
            }
            return isWrited;
        }

        /// <summary>
        /// 打印一条新的追踪信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteTrace
            (
                string message,
                bool isDetailMode = true,
                [CallerMemberName] string callerName = null,
                [CallerFilePath] string fileName = null,
                [CallerLineNumber] int line = 0,
                string TaskName = null,
                string TaskID = null
            ) => WriteLine(MessageType.Trace, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

        /// <summary>
        /// 打印一条新的调试信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteDebug
            (
                string message,
                bool isDetailMode = false,
                [CallerMemberName] string callerName = null,
                [CallerFilePath] string fileName = null,
                [CallerLineNumber] int line = 0,
                string TaskName = null,
                string TaskID = null
            ) => WriteLine(MessageType.Debug, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

        /// <summary>
        /// 打印一条新的一般信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteInfor
            (
                string message,
                bool isDetailMode = false,
                [CallerMemberName] string callerName = null,
                [CallerFilePath] string fileName = null,
                [CallerLineNumber] int line = 0,
                string TaskName = null,
                string TaskID = null
            ) => WriteLine(MessageType.Infor, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

        /// <summary>
        /// 打印一条新的警告信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteWarn
            (
                string message,
                bool isDetailMode = false,
                [CallerMemberName] string callerName = null,
                [CallerFilePath] string fileName = null,
                [CallerLineNumber] int line = 0,
                string TaskName = null,
                string TaskID = null
            ) => WriteLine(MessageType.Warn, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

        /// <summary>
        /// 打印一条新的故障信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteError
            (
                string message,
                bool isDetailMode = true,
                [CallerMemberName] string callerName = null,
                [CallerFilePath] string fileName = null,
                [CallerLineNumber] int line = 0,
                string TaskName = null,
                string TaskID = null
            ) => WriteLine(MessageType.Error, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

        /// <summary>
        /// 打印一条新的崩溃信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteFatal
           (
               string message,
               bool isDetailMode = true,
               [CallerMemberName] string callerName = null,
               [CallerFilePath] string fileName = null,
               [CallerLineNumber] int line = 0,
               string TaskName = null,
               string TaskID = null
            ) => WriteLine(MessageType.Fatal, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

        /// <summary>
        /// 打印一条新的关键信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isDetailMode">详细模式？</param>
        /// <param name="callerName">调用的方法的名字</param>
        /// <param name="fileName">调用方法所在的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <param name="TaskName">任务名</param>
        /// <param name="TaskID">任务ID</param>
        /// <returns>[true]->打印成功</returns>
        public static bool WriteCritical
            (
                string message,
                bool isDetailMode = true,
                [CallerMemberName] string callerName = null,
                [CallerFilePath] string fileName = null,
                [CallerLineNumber] int line = 0,
                string TaskName = null,
                string TaskID = null
            ) => WriteLine(MessageType.Critical, message, isDetailMode, callerName, fileName, line, TaskName, TaskID);

    }
}
