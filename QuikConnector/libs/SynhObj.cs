namespace QuikConnector.libs
{
    /// <summary> Объект защищенный мутексом, на единичный доступ</summary>
	/// <typeparam name="T"></typeparam>
	public class SynhObj<T>
    {
        /// <summary>
        /// Object lock
        /// </summary>
        private T _object;
        /// <summary>
        /// Locker
        /// </summary>
        private readonly object synh = new object();
        public SynhObj()
        {

        }
        public T Object
        {
            set
            {
                lock (synh)
                {
                    _object = value;
                }
            }
            get
            {
                lock (synh)
                {
                    return _object;
                }
            }
        }
    }
}
