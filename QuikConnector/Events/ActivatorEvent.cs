using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
	/// <summary> Класс для отложенного запуска событий из очереди. </summary>
	public class ActivatorEvent
	{
		/// <summary> Событие нового элемента. </summary>
		public Action NewEvent = null;
		/// <summary> Событие изменения элемента. </summary>
		public Action ChangeEvent = null;
		/// <summary> Строковый тип объекта </summary>
		public string TypeObject = null;
		/// <summary> Активировать весь список событий. </summary>
		public void ExecEvent()
		{
			if (this.NewEvent.NotIsNull()) NewEvent();
			if (this.ChangeEvent.NotIsNull()) ChangeEvent();
		}
	}
}
