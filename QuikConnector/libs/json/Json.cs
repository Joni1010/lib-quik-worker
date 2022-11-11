using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.libs.json
{
    internal class Json<T>
    {
        public static T Decode(string jsonString, Encoding encoding)
        {
            if (encoding  != null)
            {
                var encode = encoding.GetBytes(jsonString);
                var ms = new MemoryStream(encode);
                var json = new DataContractJsonSerializer(typeof(T));
                T sfutLimit = (T)json.ReadObject(ms);
                ms.Close();
                return sfutLimit;
            }
            return default;
        }
    }
}
