using Connector.Logs;
using QuikControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Managers
{
    public class MThread
    {
        /// <summary>
        /// Список всех потоков
        /// </summary>
        private static List<Thread> ListThread = new List<Thread>();
        /// <summary>
        /// Locker
        /// </summary>
        private static readonly object syncLockMT = new object();

        /// <summary>
        /// Count
        /// </summary>
        public static int Count
        {
            get
            {
                lock (syncLockMT)
                {
                    return MThread.ListThread.Count;
                }
            }
        }
        /// <summary>
        /// Массив потоков
        /// </summary>
        /// <returns></returns>
        public static Thread[] ToArray()
        {
            lock (syncLockMT)
            {
                return MThread.ListThread.ToArray();
            }
        }
        /// <summary>
        /// Возвращает массив не активных
        /// </summary>
        /// <returns></returns>
        public static Thread[] GetAllStopped()
        {
            lock (syncLockMT)
            {
                return MThread.ListThread.Where(t => t.ThreadState == ThreadState.Stopped).ToArray();

            }
        }


        public static Thread InitThread(ThreadStart action)
        {
            return MThread.InitThread(ThreadPriority.Normal, action);
        }
        /// <summary> Создать поток </summary>
        /// <param name="priority"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Thread InitThread(ThreadPriority priority, ThreadStart action)
        {
            var crThread = Qlog.CatchException(() =>
            {
                MThread.KillAllNoActive();
                Thread thread = new Thread(action);
                thread.Priority = priority;
                thread.Start();
                lock (syncLockMT)
                {
                    MThread.ListThread.Add(thread);
                }
                return thread;
            });
            if (crThread is Thread)
            {
                return (Thread)crThread;
            }
            return null;
        }
        /// <summary>
        /// Создать поток с параметрами
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="transferObj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Thread InitThread(ThreadPriority priority, object transferObj, ParameterizedThreadStart action)
        {
            var crThread = Qlog.CatchException(() =>
            {
                MThread.KillAllNoActive();
                Thread thread = new Thread(action);
                thread.Priority = priority;
                thread.Start(transferObj);
                lock (syncLockMT)
                {
                    MThread.ListThread.Add(thread);
                }
                return thread;
            });
            if (crThread is Thread)
            {
                return (Thread)crThread;
            }
            return null;
        }

        /// <summary>
        /// Остановить поток из коллекции
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public static bool AbortAt(Thread thread)
        {
            var res = Qlog.CatchException(() =>
            {
                if (thread.NotIsNull())
                {
                    if (thread.ThreadState == ThreadState.Running)
                    {
                        thread.Abort();
                    }
                    lock (syncLockMT)
                    {
                        return MThread.ListThread.Remove(thread);
                    }
                }
                return false;
            });
            if (res is bool)
            {
                return (bool)res;
            }
            return false;
        }
        /// <summary>
        /// Остановить поток из коллекции, и выполнить действие в конце.
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="afterAction"></param>
        /// <returns></returns>
        public static bool AbortAt(Thread thread, Action afterAction)
        {
            var res = Qlog.CatchException(() =>
            {
                if (thread.NotIsNull())
                {
                    if (thread.ThreadState == ThreadState.Running)
                    {
                        thread.Abort();
                    }
                    bool resultRemove = false;
                    lock (syncLockMT)
                    {
                        resultRemove = MThread.ListThread.Remove(thread);
                    }
                    if (afterAction.NotIsNull())
                    {
                        afterAction();
                    }
                    return resultRemove;
                }
                return false;
            });
            if (res is bool)
            {
                return (bool)res;
            }
            return false;
        }
        /// <summary>
        /// Возвращает соостояние потока, true - поток выполняется.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public static bool IsActive(Thread thread)
        {
            if (thread.IsNull())
            {
                return false;
            }
            if (thread.ThreadState == ThreadState.Running)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Остановить все не активные потоки
        /// </summary>
        private static void KillAllNoActive()
        {
            if (MThread.Count > 0)
            {
                foreach (var t in MThread.GetAllStopped())
                {
                    MThread.AbortAt(t);
                }
            }
        }
        /// <summary>
        /// Sleep
        /// </summary>
        /// <param name="milisecond"></param>
        public static void Sleep(int milisecond)
        {
            Thread.Sleep(milisecond);
        }

        /// <summary>
        /// Остановить все потоки из коллекции
        /// </summary>
        public static void AbortAll()
        {
            if (MThread.Count > 0)
            {
                foreach (var t in MThread.ToArray())
                {
                    MThread.AbortAt(t);
                }
            }
        }
    }
}
