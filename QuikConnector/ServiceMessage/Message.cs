using System;

namespace ServiceMessage
{
    /// <summary>
    ///  Структура входящего сообщения, с указанием приоритета. Разбитая на массив.
    /// </summary>
    public struct Message
    {
        public struct Report
        {
            public Report(object resObj)
            {
                Answer = null;
                ResultObject = resObj;
            }
            public string Answer;
            public object ResultObject;
        }
        /// <summary>
        /// Разделитель данных
        /// </summary>
        public const char SP_DATA = '#';
        /// <summary> Разделитель данных в сообщении для сервера </summary>
        public const char SP_FORSERVER = '|';
        /// <summary>
        /// Приоритет сообщения
        /// </summary>
        public int priority;
        /// <summary>
        /// Структура сообщения
        /// </summary>
        public string[] Struct;
        /// <summary>
        /// Флаг на проверку дублей
        /// </summary>
        public bool DelDouble;
        /// <summary>
        /// Условие удаления дубликатов
        /// </summary>
        public Func<Message, bool> ConditionDelDouble;
        /// <summary>
        /// Кол-во попыток обработки сообщения
        /// </summary>
        public int CountTry;

        public override string ToString()
        {
            return String.Join(SP_DATA.ToString(), Struct);
        }
    }
}
