using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.libs.zlib
{
    class Zlib
    {
        static public string Unzip(byte[] bytes, int size, string encoding)
        {
            MemoryStream mso = new MemoryStream(size);
            using (var msi = new MemoryStream(bytes))
            {
                msi.Seek(2, SeekOrigin.Begin);
                using (DeflateStream z = new DeflateStream(msi, CompressionMode.Decompress))
                {
                    z.CopyTo(mso);
                }
            }
            string str = Encoding.GetEncoding(encoding).GetString(mso.ToArray());
            return str.Substring(0, str.Length - 1);
            //return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
}
