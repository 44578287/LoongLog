using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoongEgg.LoongLogger
{
    /// <summary>
    /// Memory版的<see cref="BaseLogger"/>
    /// </summary>
    internal class MemoryLogger : BaseLogger
    {
        private static Log[] _buffer;    // 环形缓冲区存储 Log 实体
        private static int _writeIndex = 0;       // 当前写入位置
        private static bool _isBufferFull = false; // 是否缓冲区已满
        private static int _capacity;    // 最大容量

        /// <summary>
        /// 日志变化事件
        /// </summary>
        public static Action<Log> OnLogChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="maxCapacity"></param>
        /// <exception cref="ArgumentException"></exception>
        public MemoryLogger(LoggerLevel level, int maxCapacity = 100) : base(level) 
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentException("日志容量必须大于0");
            }
            _capacity = maxCapacity;
            _buffer = new Log[_capacity];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullMessage"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool WriteLine(string fullMessage, MessageType type)
        {
            return false;
        }

        /// <summary>
        /// 打印一条新的消息
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="message">消息的具体内容</param>
        /// <param name="callerName">调用方法的名字</param>
        /// <param name="fileName">调用的文件名</param>
        /// <param name="line">调用代码所在行</param>
        /// <returns></returns>
        public bool WriteLine(
            MessageType type,
            string message,
            string callerName,
            string fileName,
            int line)
        {
            var log = new Log
            {
                Time = DateTime.Now,
                Type = type,
                Message = message,
                CallerName = callerName,
                FileName = fileName,
                Line = line
            };

            _buffer[_writeIndex] = log; // 写入日志实体
            _writeIndex = (_writeIndex + 1) % _capacity; // 更新写入索引
            if (_writeIndex == 0)
                _isBufferFull = true; // 缓冲区写满后标记为满

            OnLogChanged?.Invoke(log); // 触发日志变化事件

            return true;
        }

        /// <summary>
        /// 获取所有日志
        /// </summary>
        /// <returns>所有日志</returns>
        public static List<Log> GetAllLogs()
        {
            if (!_isBufferFull)
            {
                return _buffer.Take(_writeIndex).ToList(); // 缓冲区未满时，返回已有日志
            }
            else
            {
                // 缓冲区已满时，返回按照时间顺序的日志
                return _buffer.Skip(_writeIndex).Concat(_buffer.Take(_writeIndex)).ToList();
            }
        }

        /// <summary>
        /// 按消息类型筛选日志
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByType(MessageType type)
        {
            return GetAllLogs().Where(log => log.Type == type).ToList();
        }

        /// <summary>
        /// 按时间范围筛选日志
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByTimeRange(DateTime startTime, DateTime endTime)
        {
            return GetAllLogs().Where(log => log.Time >= startTime && log.Time <= endTime).ToList();
        }

        /// <summary>
        /// 按文件名筛选日志
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByFileName(string fileName)
        {
            return GetAllLogs().Where(log => string.Equals(log.FileName, fileName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// 按调用方法名筛选日志
        /// </summary>
        /// <param name="callerName">调用方法名</param>
        /// <returns>匹配的日志列表</returns>
        public static List<Log> GetLogsByCallerName(string callerName)
        {
            return GetAllLogs().Where(log => string.Equals(log.CallerName, callerName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        public static void ClearLogs()
        {
            Array.Clear(_buffer, 0, _buffer.Length); // 清空缓冲区
            _writeIndex = 0;
            _isBufferFull = false;
        }
    }

    /// <summary>
    /// 日志内容实体
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 消息时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 调用方法的名字
        /// </summary>
        public string CallerName { get; set; }

        /// <summary>
        /// 调用的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 调用代码所在行
        /// </summary>
        public int Line { get; set; }
    }
}
