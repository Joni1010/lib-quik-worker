using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Components.Elements
{
    public class Element<T>
    {
        public event Event<T>.ActionEvent OnNew;
        public event Event<T>.ActionEvent OnChange;

        private Event<T> eventNew = new Event<T>();
        private Event<T> eventChange = new Event<T>();
        protected List<T> collection = new List<T>();
        /// <summary>
        /// 
        /// </summary>
        private readonly object sync = new object();
        /// <summary> Флаг определяющий, хранить коллекцию или нет. </summary>
        private bool keepCollection = true;
        /// <summary> Счетчик новых </summary>
        private long count = 0;
        /// <summary>
        /// Флаг разрешающий накопление элементов в пакет в одном событии
        /// </summary>
        private bool packEvent = false;

        public T[] ToArray()
        {
            lock (sync)
            {
                return collection.ToArray();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal Count
        {
            get
            {
                lock (sync)
                {
                    return count;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack">Пакетный режим выгрузки элементов в событии</param>
        public Element(bool pack)
        {
            packEvent = pack;
        }

        public void generating()
        {
            lock (sync)
            {
                if (OnNew.NotIsNull())
                {
                    eventNew.OnEvent += OnNew;
                    OnNew = null;
                }
                if (OnChange.NotIsNull())
                {
                    eventChange.OnEvent += OnChange;
                    OnChange = null;
                }
                if (packEvent)
                {
                    eventNew.generating();
                    eventChange.generating();
                }
                else
                {
                    eventNew.CallEvent();
                    eventChange.CallEvent();
                }
            }
        }

        public void KeepDisabled()
        {
            lock (sync)
            {
                keepCollection = false;
            }
        }

        public void Add(T elem, bool useEvent = true)
        {
            lock (sync)
            {
                if (keepCollection)
                {
                    collection.Add(elem);
                }
                count++;
                eventNew.Add(elem);
            }
        }
        public void Change(T elem, bool useEvent = true)
        {
            lock (sync)
            {
                eventChange.Add(elem);
                if (useEvent)
                {
                    eventChange.CallEvent();
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
            lock (sync)
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
            lock (sync)
            {
                return collection.Where(predicat).ToArray();
            }
        }
    }
}
