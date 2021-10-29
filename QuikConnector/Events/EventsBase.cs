using Managers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Events
{
    /// <summary> Класс обработки событий элементов (изменение, добавление) </summary>
    /// <typeparam name="T"></typeparam>
    public class EventsBase<T>
    {
        public delegate void eventElement(IEnumerable<T> sender);
        /// <summary> Событие нового элемента </summary>
        public event eventElement OnNew;
        /// <summary> Событие изменения элемента </summary>
        public event eventElement OnChange;

        /// <summary> Список для нового элемента </summary>
        protected List<T> ListEventNew = new List<T>();
        /// <summary> Список для измененного элемента </summary>
        protected List<T> ListEventChange = new List<T>();

        private readonly object syncNew = new object();
        private readonly object syncChange = new object();

        /// <summary> Максимальное кол-во событий накапливаемых в списке для выгрузки</summary>
        protected int MaxElementInList = 1000;
        /// <summary>
        /// Добавить событие о новом обьекте
        /// </summary>
        /// <param name="element"></param>
        protected void AddEventNew(T element)
        {
            lock (syncNew)
            {
                ListEventNew.Add(element);
            }
        }
        /// <summary>
        /// Добавить событие о изменении обьекта
        /// </summary>
        /// <param name="element"></param>
        protected void AddEventChange(T element)
        {
            lock (syncChange)
            {
                ListEventChange.Add(element);
            }
        }


        /// <summary> Поток обработки новых объектов </summary>
        protected Thread ThreadEventNew = null;
        /// <summary> Событие нового элемента</summary>
        protected void GenerateEventOnNew()
        {
            if (ThreadEventNew.NotIsNull())
            {
                ThreadEventNew.Join(new TimeSpan(1000));
            }
            T[] list = null;
            lock (syncNew)
            {
                if (ListEventNew.Count == 0)
                {
                    return;
                }
                list = ListEventNew.ToArray();
                ListEventNew.Clear();
            }
            ThreadEventNew = MThread.InitThread(() =>
            {
                if (OnNew.NotIsNull() && list.Length > 0)
                {
                    OnNew(list);
                }
            });
        }

        /// <summary> Поток обработки измененных объектов </summary>
        protected Thread ThreadEventChange = null;
        /// <summary> Событие измененного элемента</summary>
        protected void GenerateEventOnChange()
        {
            if (ThreadEventChange.NotIsNull())
            {
                ThreadEventChange.Join(new TimeSpan(1000));
            }
            T[] list = null;
            lock (syncChange)
            {
                if (ListEventChange.Count == 0)
                {
                    return;
                }
                list = ListEventChange.ToArray();
                ListEventChange.Clear();
            }
            ThreadEventChange = MThread.InitThread(() =>
            {
                if (OnChange.NotIsNull() && list.Length > 0)
                {
                    OnChange(list);
                }
            });
        }
    }
}
