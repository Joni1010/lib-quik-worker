using QuikConnector.Components.Log;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

/// <summary>
/// Пространство имен менеджеров управления
/// </summary>
namespace QuikConnector.Components.Controllers
{
    /// <summary>
    /// Менеджет таймеров
    /// </summary>
	public class TimersController
    {
        /// <summary>
        /// List
        /// </summary>
		private static readonly List<DispatcherTimer> ListTimers = new List<DispatcherTimer>();
        /// <summary>
        /// Locker
        /// </summary>
        private static readonly object syncLockM = new object();

        /// <summary> Создает новый таймер </summary>
        /// <param name="interval"></param>
        /// <param name="eventTimer"></param>
        /// <returns></returns>
        public static DispatcherTimer Timer(TimeSpan interval, EventHandler eventTimer)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(eventTimer);
            timer.Interval = interval;
            timer.Start();
            lock (syncLockM)
            {
                TimersController.ListTimers.Add(timer);
            }
            return timer;
        }

        /// <summary>
        /// Остановить таймер из коллекции, если она там есть.
        /// </summary>
        public static void StopAt(DispatcherTimer timer)
        {
            if (timer.NotIsNull())
            {
                timer.Stop();
                lock (syncLockM)
                {
                    TimersController.ListTimers.Remove(timer);
                }
            }
        }
        /// <summary>
        /// Остановить все таймеры
        /// </summary>
        public static void StopAll()
        {
            if (TimersController.ListTimers.Count > 0)
            {
                foreach (var t in TimersController.ListTimers.ToArray())
                {
                    TimersController.StopAt(t);
                }
            }
        }

    }
}
