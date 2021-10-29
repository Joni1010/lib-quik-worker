using System;
using System.Collections.Generic;
using System.Linq;

namespace Events
{
    /// <summary>
    /// Класс рыночных элементов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MarketElement<T> : EventsBase<T>
    {
        /// <summary> Коллекция элементов </summary>
        protected List<T> collection = new List<T>();
        /// <summary> Locker </summary>
        private readonly object syncObj = new object();
        /// <summary> Флаг определяющий, хранить коллекцию или нет. </summary>
        private bool keepCollection = true;
        /// <summary> Счетчик новых </summary>
        private long countNew = 0;
        /// <summary> Счетчик изменений </summary>
        private long countChange = 0;
        /// <summary>
        /// Флаг генерирования событий
        /// </summary>
        private bool generateEvent = true;
        /// <summary> Событие переполнения стека </summary>
        public Action OnOverFlow = null;

        /// <summary> Получить список коллекции в виде Array </summary>
        public T[] ToArray()
        {
            lock (syncObj)
            {
                return collection.ToArray();
            }
        }

        /// <summary> Кол-во элементов в коллекции </summary>
        public decimal Count
        {
            get
            {
                lock (syncObj)
                {
                    return collection.Count;
                }
            }
        }
        /// <summary>
        /// Кол-во событий новое
        /// </summary>
        /// <returns></returns>
        public long CountNew()
        {
            lock (syncObj)
            {
                return countNew;
            }
        }
        /// <summary>
        /// Кол-во событий изменения
        /// </summary>
        /// <returns></returns>
        public long CountChange()
        {
            lock (syncObj)
            {
                return countChange;
            }
        }

        /// <summary> Объект запуска отложенных событий  </summary>
        private ActivatorEvent ActEvents = new ActivatorEvent();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ListAcEvents"></param>
        public MarketElement(List<ActivatorEvent> ListAcEvents)
        {
            lock (syncObj)
            {
                ActEvents.TypeObject = GetType().ToString();
                ActEvents.NewEvent = GenerateEventOnNew;
                ActEvents.ChangeEvent = GenerateEventOnChange;
                ListAcEvents.Add(ActEvents);
            }
        }
        /// <summary>
        /// Отключить хранение коллекции
        /// </summary>
        public void DisabledCollection()
        {
            keepCollection = false;
        }
        /// <summary>
        /// Отключает события
        /// </summary>
        /// <param name="useEvents"></param>
        public void DisabledEvents()
        {
            generateEvent = false;
        }
        /// <summary> Добавить в коллекцию новый элемент. </summary>
        /// <param name="elem">Элемент коллекции</param>
        /// <param name="generateEvent">true - генерировать событие OnNew</param>
        public void Add(T elem, bool useEvent = true)
        {
            lock (syncObj)
            {
                countNew++;
                if (keepCollection)
                {
                    collection.Add(elem);
                }
                if (generateEvent)
                {
                    AddEventNew(elem);
                    if (useEvent)
                    {
                        GenerateEventOnNew();
                    }
                    else if (ListEventNew.Count >= MaxElementInList)
                    {
                        if (OnOverFlow.NotIsNull())
                        {
                            OnOverFlow();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Изменение элемента в коллекции
        /// </summary>
        /// <param name="elem">Изменяемый элемент</param>
        /// <param name="functBeforeEvent">Действие перед вызовом события OnChange.</param>
        /// <param name="generateEvent">true - генерировать событие OnChange, иначе накапливать список событий.</param>
        public void Change(T elem, bool useEvent = true)
        {
            lock (syncObj)
            {
                countChange++;
                if (generateEvent)
                {
                    AddEventChange(elem);
                    if (useEvent)
                    {
                        GenerateEventOnChange();
                    }
                    else if (ListEventChange.Count >= MaxElementInList)
                    {
                        if (OnOverFlow.NotIsNull())
                        {
                            OnOverFlow();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Возвращает первый найденный элемент
        /// </summary>
        /// <param name="predicat"></param>
        /// <returns></returns>
        public T SearchFirst(Func<T, bool> predicat)
        {
            lock (syncObj)
            {
                return collection.FirstOrDefault(predicat);
            }
        }
        /// <summary>
        ///  Возвращает все найденные элементы
        /// </summary>
        /// <param name="predicat"></param>
        /// <returns></returns>
        public T[] SearchAll(Func<T, bool> predicat)
        {
            lock (syncObj)
            {
                return collection.Where(predicat).ToArray();
            }
        }

    }
}
