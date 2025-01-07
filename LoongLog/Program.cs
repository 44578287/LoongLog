using LoongEgg.LoongLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Yitter.IdGenerator;

namespace LoongLog
{
    class Program
    {
        // https://github.com/loongEgg/LoongLog
        static void Main(string[] args) {

            // 激活Logger
            Logger.Enable(LoggerType.Console | LoggerType.Debug | LoggerType.File | LoggerType.Memory, LoggerLevel.Debug);

            var options = new IdGeneratorOptions(1);
            YitIdHelper.SetIdGenerator(options);

            //Logger.OnLogChanged += (log) =>
            //{
            //    Console.WriteLine($"[{log.Time:yyyy-MM-dd HH:mm:ss}] [{log.Type}] {log.Message}");
            //};
            Logger.WriteTrace("这是Trace", TaskID: YitIdHelper.NextId().ToString());
            Logger.WriteDebug("这是debug", TaskName: "debug", TaskID: YitIdHelper.NextId().ToString());
            Logger.WriteInfor("这是infor ...", TaskName: "infor", TaskID: YitIdHelper.NextId().ToString());
            Logger.WriteWarn("这是Warn ...", TaskName: "Warn", TaskID: YitIdHelper.NextId().ToString());
            Logger.WriteError("这是error ...", TaskName: "error", TaskID: YitIdHelper.NextId().ToString());
            Logger.WriteFatal("这是fatal ...", TaskName: "fatal", TaskID: YitIdHelper.NextId().ToString());
            Logger.WriteCritical("这是Critical ...", TaskName: "Critical", TaskID: YitIdHelper.NextId().ToString());
            //Console.WriteLine("Hello World!");
            //Console.WriteLine("Hello World!");
            var data = Logger.GetLogs();
            // 注销logger
            Logger.Disable();

            Console.ReadKey();// 暂停的一种方法
            //Console.ForegroundColor
        }

    }
}
