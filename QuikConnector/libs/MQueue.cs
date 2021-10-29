using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 
/// </summary>
namespace QuikConnector.libs
{
    /// <summary> Класс очереди сообщений. </summary>
	public class MQueue<T>
    {
        /// <summary>
        /// Locker
        /// </summary>
        private readonly object syncLock = new object();
        /// <summary> Список сообщений. </summary>
        private List<T> Stack = new List<T>();
        /// <summary> Кол-во сообщений в стеке. </summary>
        public int Count
        {
            get
            {
                lock (syncLock)
                {
                    return Stack.Count;
                }
            }
        }
        /// <summary> Последее добавленное сообщение </summary>
        public T Last = default;

        public delegate T PriorityAction(T msg);
        public delegate IEnumerable<T> IEPriorityAction(T msg, IEnumerable<T> collection);
        /// <summary>
        /// Метод для переопределения приоритета
        /// </summary>
        public PriorityAction Prioritizing = null;
        /// <summary>
        /// Метод поиска дубликатов
        /// </summary>
        public IEPriorityAction HandlerDouble = null;

        public MQueue()
        {

        }

        /// <summary> Получает список хранящих в стеке данных </summary>
        /*public IEnumerable<string> DataStack
        {
            get
            {
                return Stack;
            }
        }*/
        /// <summary> Добавить сообщение в стек. </summary>
        public void Add(T msg)
        {
            lock (syncLock)
            {
                if (Prioritizing.NotIsNull())
                {
                    msg = Prioritizing(msg);
                }
                if (HandlerDouble.NotIsNull() && Stack.Count > 0) {
                    var list = HandlerDouble(msg, Stack).ToArray();
                    if (list.Count() > 0) {
                        foreach (var el in list)
                        {
                            Stack.Remove(el);
                        }
                    }
                }
                Stack.Add(msg);
                Last = msg;
            }
        }
        /// <summary>
        /// Сортирует сообщения по приоритету
        /// </summary>
        /// <param name="selector"></param>
        public void SortByPriority(Func<T, int> selector)
        {
            lock (syncLock)
            {
                Stack = Stack.OrderBy(selector).ToList();
            }
        }
        /// <summary> Получить первое сообщение в стеке. </summary>
        public T getFirst
        {
            get
            {
                lock (syncLock)
                {
                    if (Count > 0)
                    {
                        return Stack.ElementAt(0);
                    }
                    return default;
                }
            }
        }
        /// <summary> Удаляет первый элемент из стека </summary>
        public void DeleteItem(T item)
        {
            lock (syncLock)
            {
                if (Stack.Count > 0)
                {
                    Stack.Remove(item);
                    if (Count == 0)
                    {
                        Last = default;
                    }
                }
            }
        }
        /// <summary> Очищает стек сообщений </summary>
        public void Clear()
        {
            lock (syncLock)
            {
                Stack.Clear();
                Last = default;
            }
        }
    }
}
