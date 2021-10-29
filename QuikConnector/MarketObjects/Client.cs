
namespace MarketObjects
{
    /// <summary> Класс клиента</summary>
    public class Client
    {
        public string Code;
		/// <summary> Строковое представление клиента </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Code;
		}
    }
}
