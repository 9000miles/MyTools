using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Common
{
    public class TxtHelper
    {
        private readonly string filePath = @"E:\360Downloads\Hjofowe";
        private DirectoryInfo directoryInfo;
        private FileInfo fileInfo;
        private string txt = string.Empty;
        private string pathAllName;

        public TxtHelper(string path, string fileName)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            pathAllName = path + @"\" + fileName;
            if (!File.Exists(pathAllName))
            {
                File.Create(pathAllName).Dispose();
            }
            fileInfo = new FileInfo(pathAllName);
        }

        public void Write(string str)
        {
            using (StreamWriter writer = fileInfo.AppendText())
            {
                writer.WriteLine(str);
            }
        }

        public void ClearTxt()
        {
            File.Open(pathAllName, FileMode.Create).Dispose();
        }

        public string Read()
        {
            Stream stream = fileInfo.OpenRead();
            using (StreamReader reader = new StreamReader(stream))
            {
                txt = reader.ReadToEnd();
            }
            return txt;
        }
    }
}