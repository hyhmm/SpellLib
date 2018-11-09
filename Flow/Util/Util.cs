using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace XFlow
{
    public static class Util
    {

        public static string ReadTextFile(string filePath)
        {
            if (!File.Exists(filePath))
                return "";

            StreamReader r = File.OpenText(filePath);
            string info = r.ReadToEnd();
            r.Close();
            return info;
        }

        public static void MakeDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); 
            }
        }

        public static void WriteTextFile(string filePath, string data)
        {
            MakeDir(Path.GetDirectoryName(filePath));
            TextWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.Write(data);
            writer.Close();
        }

        public static List<T> ConvertListItemsFromString<T>(string str, char separator = ',')
        {
            List<T> ret = new List<T>();
            if (string.IsNullOrEmpty(str))
                return ret;

            var items = str.Split(separator);

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == "")
                    continue;

                ret.Add((T)System.Convert.ChangeType(items[i], typeof(T)));
            }
            return ret;
        }
    }
}