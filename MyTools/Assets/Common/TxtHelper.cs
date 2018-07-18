using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Common
{
    public class TxtHelper
    {
        private DirectoryInfo directoryInfo;
        private FileInfo fileInfo;
        private string txt = string.Empty;

        public TxtHelper(string path, string fileName)
        {
            directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            fileInfo = new FileInfo(path + "//" + fileName);
        }

        public void Write(string str)
        {
            using (StreamWriter writer = fileInfo.AppendText())
            {
                writer.WriteLine(str);
            }
        }

        public void DeleteTxt()
        {
            fileInfo.Delete();
        }

        public string Read()
        {
            Stream stream = fileInfo.OpenRead();
            using (StreamReader reader = new StreamReader(stream))
            {
                //while (reader.Peek() > 0)
                //{
                //    txt += reader.ReadLine() + "\n";
                //}
                txt = reader.ReadToEnd();
            }
            return txt;
        }
    }
}