using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Logger
{
    private static string logDirName = "Logs";
    private static int fileLimit = 30;

    public static void LogToFile(string message)
    {
        string fileName = DateTime.Now.ToShortDateString() + ".txt";
        fileName = fileName.Replace("/", "_");

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logDirName + "/" + fileName, true))
        {
            file.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " +  message);
        }
    }

    public static void Init()
    {
        if(!Directory.Exists(logDirName))
        {
            Directory.CreateDirectory(logDirName);
        }

        DirectoryInfo info = new DirectoryInfo(logDirName);
        FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
        if(files.Length > fileLimit)
        {
            int diff = files.Length - fileLimit;
            for (int i = 0; i < diff; i++)
                File.Delete(files[i].FullName);
        }
    }
}
