using System;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace vcDWMFix
{
    class Program
    {
        static LogWriter log = new LogWriter();//логгер чтобы запоминать каждый случай когда пришлось чистить ОЗУ от этого недоразумения

        static void Main(string[] args)
        {
            const int loopTimer = 10000; //10 sec.
            const int memtoKill = 1073741824; //1 GiB
            while (true)
            {
                DWMLoop(memtoKill);
                Thread.Sleep(loopTimer);
            }
        }
        static void DWMLoop(int memToKill)
        {
            Process killDWM = Process.GetProcessesByName("dwm")[0];//Use only the first process in the array,
                                                                   //even though there may be multiple of them (e.g. on shared family PC)
            long processPagedMemory = killDWM.PagedMemorySize64; //Getting process paged memory size
            if (processPagedMemory >= memToKill)
            {
                //Making a log entry and killing the process. System restarts it automatically.
                log.WriteToLog("right now DWM allocated " + (processPagedMemory / 1048576) + "MiB, killing",2);
                killDWM.Kill();
            }

        }
    }
    class LogWriter //ported it to old C# version from my other project, so this program can run using system preinstalled .NET on W10
    {
        /*
         * Log Levels
         * 0 - info
         * 1 - warnings
         * 2 - important warnings
         * 3 - errors
         */
        int sLogLevel = 2;
        readonly String logLocation;

        public LogWriter()
        {
            String fullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String dir = System.IO.Path.GetDirectoryName(fullPath);
            String logFullPath = dir + "\\log-" + DateTime.Now.ToString().Replace(':', '-') + ".txt";//make a file in the same directory where an executable is
            logLocation = logFullPath;
            WriteToLog("Starting log", 0);
        }
        public void WriteToLog(String Message, int logLevel)
        {
            StackTrace stackTrace = new StackTrace();
            String callerFunction = stackTrace.GetFrame(1).GetMethod().Name;
            String timeNow = TimeNow();
            String composedMessage = timeNow + " - " + callerFunction + ": " + Message;
            if (logLevel >= sLogLevel) SWWrite(composedMessage);
        }
        private void SWWrite(String text)
        {
            using (StreamWriter writeLog = new StreamWriter(logLocation, true))
            {
                writeLog.WriteLine(text);
                writeLog.Flush();
            }
        }
        private string TimeNow() => DateTime.Now.ToString();
    }
}

