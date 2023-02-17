using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace LoongEgg.LoongLogger
{
    /* 
	| WeChat: InnerGeek
	| LoongEgg@163.com 
	| https://github.com/loongEgg/LoongLog
	*/

    /// <summary>
    /// 控制台版的Logger
    /// </summary>
    internal class ConsoleLogger : BaseLogger, ILogger
    { 
        // TODO: 04-A 过期代码警告
        [Obsolete("这是一个演示方法，不要乱用", false)]
        // TODO: 02 获取调用方法名、调用文件名和调用代码所在行
        public bool WriteLine(
            [CallerMemberName] string callerName = null,
            [CallerFilePath] string path = null,
            [CallerLineNumber] int line = 0) {

            string sth = $"{callerName} > {path} > {line}";
            Console.WriteLine(sth);

            // TODO: 03 任务列表、精简文件名、指定长度输出不足补空格
            string msg = $"{Path.GetFileName(path)} > {callerName}() > in line [" + line.ToString().PadLeft(3, ' ') + "]";
            Console.WriteLine(msg);
            return true;
        }
         
        /// <summary>
        /// 打印一条新的消息
        /// </summary>
        ///     <param name="type">消息类型</param>
        ///     <param name="message">消息内容</param>
        ///     <param name="callerName">调用的方法的名字</param>
        ///     <param name="path">调用方法所在的文件名</param>
        ///     <param name="line">调用的代码所在行</param>
        /// <returns>[true]->打印成功</returns>
        public bool WriteLine(
            MessageType type,
            string message,
            [CallerMemberName] string callerName = null,
            [CallerFilePath] string path = null,
            [CallerLineNumber] int line = 0) {

            string msg =
                DateTime.Now.ToString()
                + $" [ {type.ToString()} ] -> "  
                + $"{Path.GetFileName(path)} > {callerName}() > in line [{line.ToString().PadLeft(3, ' ')}]: "
                + message;

            Console.WriteLine(msg);
            return true;
        }

        // TODO: 08-A 构造器
        public ConsoleLogger(LoggerLevel level = LoggerLevel.Debug) : base(level) { }

        // TODO: 08-B 实现抽象类BaseLogger的WriteLine()方法
        /// <summary>
        /// <see cref="BaseLogger.WriteLine(string, MessageType)"/>
        /// </summary> 
        public override bool WriteLine(string fullMessage, MessageType type) {
            if ((int)type < (int)Level)
                return false;

            var BoldColor = Console.BackgroundColor;
            var oldColor = Console.ForegroundColor;
            
            switch (type) {
                case MessageType.Trace://追踪
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;

                case MessageType.Debug://调试
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;

                case MessageType.Infor://一般
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;

                case MessageType.Warn://警告
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;

                case MessageType.Error://错误
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case MessageType.Fatal://致命
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case MessageType.Critical://关键
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.WriteLine(fullMessage);
            Console.BackgroundColor = BoldColor;
            Console.ForegroundColor = oldColor;
            return true;
        }
    }
}
