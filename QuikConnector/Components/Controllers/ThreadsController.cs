using QuikConnector.Components.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QuikConnector.Components.Controllers
{
    public class ThreadsController
    {
        /// <summary>
        /// Список всех потоков
        /// </summary>
        private static readonly List<Thread> ListThread = new List<Thread>();
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
                    return ThreadsController.ListThread.Count;
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
                return ThreadsController.ListThread.ToArray();
            }
        }
        /// <summary>
        /// Возвращает массив не активных
        /// </summary>
        /// <returns></returns>
        private static Thread[] GetAllStopped()
        {
            lock (syncLockMT)
            {
                return ThreadsController.ListThread.Where(t =>
                t.ThreadState == ThreadState.Stopped || t.ThreadState == ThreadState.Aborted
                ).ToArray();
            }
        }


        public static Thread Thread(ThreadStart action)
        {
            return ThreadsController.Thread(ThreadPriority.Normal, action);
        }
        /// <summary> Создать поток </summary>
        /// <param name="priority"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Thread Thread(ThreadPriority priority, ThreadStart action)
        {
            ThreadsController.KillAllNoActive();
            Thread thread = new Thread(action)
            {
                Priority = priority
            };
            thread.Start();
            lock (syncLockMT)
            {
                ThreadsController.ListThread.Add(thread);
            }
            return thread;
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
            ThreadsController.KillAllNoActive();
            Thread thread = new Thread(action)
            {
                Priority = priority
            };
            thread.Start(transferObj);
            lock (syncLockMT)
            {
                ThreadsController.ListThread.Add(thread);
            }
            return thread;
        }

        /// <summary>
        /// Остановить поток из коллекции
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public static bool AbortAt(Thread thread)
        {
            if (thread.NotIsNull())
            {
                if (thread.ThreadState == ThreadState.Running || thread.ThreadState == ThreadState.Stopped)
                {
                    thread.Abort();
                }
                lock (syncLockMT)
                {
                    return ThreadsController.ListThread.Remove(thread);
                }
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
            if (thread.NotIsNull())
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    thread.Abort();
                }
                bool resultRemove = false;
                lock (syncLockMT)
                {
                    resultRemove = ThreadsController.ListThread.Remove(thread);
                }
                if (afterAction.NotIsNull())
                {
                    afterAction();
                }
                return resultRemove;
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
            lock (syncLockMT)
            {
                if (ThreadsController.Count > 0)
                {
                    foreach (var t in ThreadsController.GetAllStopped())
                    {
                        ThreadsController.AbortAt(t);
                        ThreadsController.ListThread.Remove(t);
                    }
                }
            }
        }
        /// <summary>
        /// Sleep
        /// </summary>
        /// <param name="milisecond"></param>
        public static void Sleep(int milisecond)
        {
            System.Threading.Thread.Sleep(milisecond);
        }

        /// <summary>
        /// Остановить все потоки из коллекции
        /// </summary>
        public static void AbortAll()
        {
            if (ThreadsController.Count > 0)
            {
                foreach (var t in ThreadsController.ToArray())
                {
                    ThreadsController.AbortAt(t);
                }
            }
        }
    }
}
