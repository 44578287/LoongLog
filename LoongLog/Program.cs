using LoongEgg.LoongLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LoongLog
{
    class Program
    {
        // https://github.com/loongEgg/LoongLog
        static void Main(string[] args) {

            // 激活Logger
            Logger.Enable(LoggerType.Console | LoggerType.Debug | LoggerType.File, LoggerLevel.Debug);


            Logger.WriteDebug("这是debug");
            Logger.WriteInfor("这是infor ...");

            Logger.WriteError("这是error ...");
            Logger.WriteFatal("这是fatal ...");

            // 注销logger
            Logger.Disable();

            Console.ReadKey();// 暂停的一种方法
        }

    }
}
