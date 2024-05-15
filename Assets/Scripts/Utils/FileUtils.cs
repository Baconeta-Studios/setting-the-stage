using System.IO;
using UnityEngine;

namespace Utils
{
    public static class FileUtils
    {
        public static void WriteToFile(string fileName, string json)
        {
            string path = GetFilePath(fileName);
            FileStream stream = new FileStream(path, FileMode.Create);

            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(json);
        }

        public static string ReadFromFile(string fileName)
        {
            string result = null;
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                using StreamReader reader = new StreamReader(path);
                result = reader.ReadToEnd();
            }
            else
            {
                StSDebug.LogWarning($"{fileName} not a valid path.");
            }
        
            return result;
        }

        public static string GetFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}
