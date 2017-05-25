using System;
using System.IO;
using System.Xml.Serialization;

namespace NPCommonHelp.Xml
{
    /// <summary>
    ///     序列化文件辅助类
    /// </summary>
    public class Serialize
    {
        /// <summary>
        ///     序列化文件
        /// </summary>
        /// <typeparam name="T">序列化对象类型</typeparam>
        /// <param name="obj">序列化对象</param>
        /// <param name="file">序列化文件</param>
        public static void SerializeNow<T>(T obj, string file) where T : class
        {
            try
            {
                var path = Path.GetDirectoryName(file);
                if (path == null)
                    return;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var xs = new XmlSerializer(typeof(T));
                Stream stream = new FileStream(file,
                    FileMode.Create, FileAccess.Write, FileShare.Read);
                xs.Serialize(stream, obj);
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="file">反序列化文件</param>
        /// <param name="obj">反序列化对象</param>
        public static void DeSerializeNow<T>(string file, out T obj) where T : class
        {
            try
            {
                if (File.Exists(file))
                {
                    var xs = new XmlSerializer(typeof(T));
                    Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    obj = xs.Deserialize(stream) as T;
                    stream.Close();
                }
                else
                {
                    obj = null;
                    Console.WriteLine("反序列化文件  " + file + "  不存在");
                }
            }
            catch (Exception e)
            {
                obj = null;
                Console.WriteLine(e);
                throw;
            }
        }
    }
}