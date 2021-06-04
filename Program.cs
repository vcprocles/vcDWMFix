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
            const int loopTimer = 10000; //где-то 10 сек
            const int memtoKill = 1000000000; //где-то 1 гб
            while (true) //бесконечный цикл, фу
            {
                DWMLoop(memtoKill); 
                Thread.Sleep(loopTimer);
            }
        }
        static void DWMLoop(int memToKill)
        {
            Process killDWM = Process.GetProcessesByName("dwm")[0];//Получаем процесс как экземпляр класса.
                                                                   //Хватаю только первый процесс в массиве,
                                                                   //хотя на самом деле их в системе может быть и несколько
            long processPagedMemory = killDWM.PagedMemorySize64; //Получаем выделенную процессом память.
                                                                 //Можно было бы встроить это в if, но и так сойдёт
            if (processPagedMemory >= memToKill) 
            {
                //пишем в лог и прибиваем процесс
                log.WriteToLog(DateTime.Now.ToString() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name + " - right now allocated " + (processPagedMemory / 1048576) + "MiB, killing");
                killDWM.Kill();
            }

        }
    }
    class LogWriter //Простенький класс для написания логов. Пишет в ту же директорию, где лежит программа.
                    //Изначально я писал это под .net 5 и экземляр логгера там создаётся только при первом прибитии процесса,
                    //это немного экономит память. Не знаю как будет вести себя этот проект на .net 4.7
    {
        public LogWriter()
        {
            String fullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String dir = System.IO.Path.GetDirectoryName(fullPath);
            String logFullPath = dir + "\\log-" + DateTime.Now.ToString().Replace(':', '-') + ".txt";
            logLocation = logFullPath;
        }
        String logLocation;
        public void WriteToLog(String text)
        {
            using (StreamWriter writeLog = new StreamWriter(logLocation, true))
            {
                writeLog.WriteLine(text);
                writeLog.Flush();
            }
        }
    }
}