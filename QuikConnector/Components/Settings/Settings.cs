using QuikConnector.libs;
using QuikConnector.libs.ini;

namespace QuikConnector.Components.Settings
{
	/// <summary>
	/// Класс работы с файлом настроек (конфиг)
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Элемент настройки
		/// </summary>
		public class Element
		{
			/// <summary> Название секции </summary>
			public string Section = "";
			/// <summary> Ключ/название параметра</summary>
			public string Param = "";
			/// <summary> Значение параметра</summary>
			public string Value = "";
		}
		/// <summary> Файл настроек </summary>
		public string FileSettings = "conf.ini";
		/// <summary> Хедер открытого файла настроек </summary>
		private IniFile hIniFile = null;
		public Settings()
		{
			this.hIniFile = new IniFile("conf.ini");
		}

		/// <summary>
		/// Получить данные параметра
		/// </summary>
		/// <param name="Section">Название секции</param>
		/// <param name="NameParam"></param>
		/// <returns></returns>
		public Settings.Element GetParam(string section, string nameParam)
		{
			if (this.hIniFile.NotIsNull())
			{
				return new Settings.Element() {Section = section, Param = nameParam, Value = this.hIniFile.Read(nameParam, section) };
			}
			return null;
		}
	}
}
