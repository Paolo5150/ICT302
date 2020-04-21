using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    public static void LogToFile(string message, string fileName = "log.txt")
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName, true))
        {
            file.WriteLine(message);
        }
    }
}
