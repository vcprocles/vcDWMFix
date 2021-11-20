using System;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace vcDWMFix
{
    class Program
    {
        static LogWriter log = new LogWriter();//create a logger
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
                log.WriteToLog("right now DWM allocated " + (processPagedMemory / 1048576) + "MiB, killing");
                killDWM.Kill();
            }

        }
    }
    class LogWriter
    {
        readonly string logLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log-" + TimeNow.Replace(':', '-') + ".txt";

        public LogWriter()
        {
            WriteToLog("Starting log");
        }
        public void WriteToLog(string Message)
        {
            StackTrace stackTrace = new StackTrace();
            string callerFunction = stackTrace.GetFrame(1).GetMethod().Name;
            string composedMessage = TimeNow + " - " + callerFunction + ": " + Message;
            SWWrite(composedMessage);
        }
        private void SWWrite(string text)
        {
            using (StreamWriter writeLog = new StreamWriter(logLocation, true))
            {
                writeLog.WriteLine(text);
                writeLog.Flush();
            }
        }
        static string TimeNow => DateTime.Now.ToString();
    }
}

