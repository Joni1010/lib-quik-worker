using System;


namespace QuikConnector.MarketObjects
{
    /// <summary>
	/// Дата для правильной конвертации
	/// </summary>
	[Serializable]
    public class DateMarket
    {
        private string day = "00";
        private string month = "00";
        private string year = "0000";
        private string hour = "00";
        private string min = "00";
        private string sec = "00";
        private string ms = "000";
        //public string mcs = "000000";
        //public IEnumerator t;

        public DateMarket()
        {

        }
        public DateMarket(DateTime datetime)
        {
            this.SetDateTime(datetime);
        }

        public DateMarket(string datetime)
        {
            this.SetDateTime(this.GetDateTime(datetime));
        }

        public DateMarket SetDateTimeByStruct(int firstValue, string[] data)
        {
            SetYear(data[firstValue]).SetMonth(data[firstValue + 1]).SetDay(data[firstValue + 2]);
            SetHour(data[firstValue + 3]).SetMinute(data[firstValue + 4]).SetSecond(data[firstValue + 5]).SetMiliSecond(data[firstValue + 6]);
            return this;
        }
        public DateMarket SetYear(string year)
        {
            this.year = year;
            return this;
        }
        public DateMarket SetYear(int year)
        {
            this.year = year.ToString();
            return this;
        }
        public DateMarket SetMonth(string month)
        {
            this.month = month;
            return this;
        }
        public DateMarket SetMonth(int month)
        {
            this.month = month.ToString();
            return this;
        }
        public DateMarket SetDay(string day)
        {
            this.day = day;
            return this;
        }
        public DateMarket SetDay(int day)
        {
            this.day = day.ToString();
            return this;
        }

        public DateMarket SetHour(string hour)
        {
            this.hour = hour;
            return this;
        }
        public DateMarket SetHour(int hour)
        {
            this.hour = hour.ToString();
            return this;
        }

        public DateMarket SetMinute(string min)
        {
            this.min = min;
            return this;
        }
        public DateMarket SetMinute(int min)
        {
            this.min = min.ToString();
            return this;
        }
        public DateMarket SetSecond(string sec)
        {
            this.sec = sec;
            return this;
        }
        public DateMarket SetSecond(int sec)
        {
            this.sec = sec.ToString();
            return this;
        }
        public DateMarket SetMiliSecond(string ms)
        {
            this.ms = ms;
            return this;
        }
        public DateMarket SetMiliSecond(int ms)
        {
            this.ms = ms.ToString();
            return this;
        }

        /// <summary> Установить дату и время </summary>
        /// <param name="dateTime"></param>
        public DateMarket SetDateTime(DateTime dateTime)
        {
            this.year = dateTime.Year.ToString();
            this.month = dateTime.Month.ToString();
            this.day = dateTime.Day.ToString();

            this.hour = dateTime.Hour.ToString();
            this.min = dateTime.Minute.ToString();
            this.sec = dateTime.Second.ToString();

            this.ms = dateTime.Millisecond.ToString();
            return this;
        }

        
        /// <summary>
        /// Конвертирует DateTime в DateMarket  
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateMarket ExtractDateTime(DateTime datetime)
        {
            return (new DateMarket()).SetDateTime(datetime);
        }
        /// <summary> Конвертировать в строку формата YYYYMMDD </summary>
        /// <returns></returns>
        public string To_YYYYMMDD()
        {
            return this.year +
                (this.month.Length == 1 ? '0' + this.month : this.month) +
                (this.day.Length == 1 ? '0' + this.day : this.day);
        }
        /// <summary>
        /// Получить дату и время в структуре DateTime
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return GetDateTime();
            }
        }
        /// <summary> Получить дату и время </summary>
        /// <param name="dateTime"></param>
        public DateTime GetDateTime()
        {
            if(year.ToInt32() == 0 && month.ToInt32() == 0 && day.ToInt32() == 0)
            {
                return System.DateTime.MinValue;
            }
            return Convert.ToDateTime(this.DateTimeMarket);
        }
        public DateTime GetDateTime(string datetime)
        {
            return Convert.ToDateTime(datetime);
        }
        public override string ToString()
        {
            return this.DateTime.ToString();
        }
        public string DateTimeMarket
        {
            get
            {
                return day + "/" + month + "/" + year + " " +
                    this.Time + "." + ms;
            }
        }
        /// <summary> Дата </summary>
        public string Date
        {
            get
            {
                return day + "." + month + "." + year;
            }
        }
        /// <summary>  Время </summary>
        public string Time
        {
            get
            {
                return hour + ":" + min + ":" + sec;
            }
        }
    }
}
