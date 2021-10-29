using Connector.Logs;
using QuikControl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

/// <summary>
/// Пространство имен менеджеров управления
/// </summary>
namespace Managers
{
    /// <summary>
    /// Менеджет таймеров
    /// </summary>
	public class MTimer
	{
        /// <summary>
        /// List
        /// </summary>
		private static List<DispatcherTimer> ListTimers = new List<DispatcherTimer>();
        /// <summary>
        /// Locker
        /// </summary>
        private static readonly object syncLockM = new object();

        /// <summary> Создает новый таймер </summary>
        /// <param name="interval"></param>
        /// <param name="eventTimer"></param>
        /// <returns></returns>
        public static DispatcherTimer InitTimer(TimeSpan interval, EventHandler eventTimer)
		{
			var crTimer = Qlog.CatchException(() =>
			{
				DispatcherTimer timer = new DispatcherTimer();
				timer.Tick += new EventHandler(eventTimer);
				timer.Interval = interval;
				timer.Start();
                lock (syncLockM)
                {
                    MTimer.ListTimers.Add(timer);
                }
				return timer;
			});
            if (crTimer is DispatcherTimer)
            {
                return (DispatcherTimer)crTimer;
            }
			return null;
		}

		/// <summary>
		/// Остановить таймер из коллекции, если она там есть.
		/// </summary>
		public static void StopAt(DispatcherTimer timer)
		{
			Qlog.CatchException(() =>
			{
				if (timer.NotIsNull())
				{
					timer.Stop();
                    lock (syncLockM)
                    {
                        MTimer.ListTimers.Remove(timer);
                    }
				}
			});
		}
		/// <summary>
		/// Остановить все таймеры
		/// </summary>
		public static void StopAll()
		{
			if (MTimer.ListTimers.Count > 0)
			{
				foreach (var t in MTimer.ListTimers.ToArray())
				{
					MTimer.StopAt(t);
				}
			}
		}

	}
}
