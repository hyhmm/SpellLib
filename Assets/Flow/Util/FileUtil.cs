using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileUtil : MonoBehaviour {

    public static string ReadTextFile(string filePath)
    {
        if (!File.Exists(filePath))
            return "";

        StreamReader r = File.OpenText(filePath);
        string info = r.ReadToEnd();
        r.Close();
        return info;
    }
}
