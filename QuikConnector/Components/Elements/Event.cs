using System;
using System.Collections.Generic;
using System.Threading;

namespace QuikConnector.Components.Elements
{
    public class Event<T>
    {
        public delegate void ActionEvent(IEnumerable<T> sender);
        /// <summary> Событие нового элемента </summary>
        public event ActionEvent OnEvent;
        /// <summary>
        /// Список элементов накопленных для события
        /// </summary>
        private List<T> list = new List<T>();
        /// <summary>
        /// потоковая синхронизация
        /// </summary>
        private readonly object sync = new object();
        /// <summary>
        /// 
        /// </summary>
        private int LimitOverFlow = 1000;
        /// <summary>
        /// Время последнего изменения коллекции
        /// </summary>
        private DateTime change = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        private int count = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            lock (sync)
            {
                list.Add(element);
                count++;
                change = DateTime.Now;
            }
        }

        public void generating()
        {
            bool activate = false;
            lock (sync)
            {
                if (count > 0)
                {
                    if (DateTime.Now.Ticks - change.Ticks > 500)
                    {
                        activate = true;
                    }
                    else if (count > LimitOverFlow)
                    {
                        activate = true;
                    }
                }
            }
            if (activate)
            {
                CallEvent();
            }
        }
        /// <summary>
        /// Вызвать срабатывание события
        /// </summary>
        public void CallEvent()
        {
            T[] listEvent = null;
            lock (sync)
            {
                if (count == 0)
                {
                    return;
                }
                listEvent = list.ToArray();
                list.Clear();
                count = 0;
            }
            if (OnEvent.NotIsNull())
            {
                new Thread(() =>
                {
                    OnEvent(listEvent);
                }).Start();
            }
        }
    }
}
