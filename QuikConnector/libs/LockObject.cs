namespace QuikConnector.libs
{
    /// <summary> Объект защищенный мутексом, на единичный доступ</summary>
	/// <typeparam name="T"></typeparam>
	public class LockObject<T>
    {
        /// <summary>
        /// Object lock
        /// </summary>
        private T _object;
        /// <summary>
        /// Locker
        /// </summary>
        private readonly object synhObj = new object();
        public LockObject()
        {

        }
        public T Object
        {
            set
            {
                lock (synhObj)
                {
                    this._object = value;
                }
            }
            get
            {
                lock (synhObj)
                {
                    return this._object;
                }
            }
        }
    }
}
