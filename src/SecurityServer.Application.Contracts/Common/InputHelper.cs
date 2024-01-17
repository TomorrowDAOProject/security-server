using System;
using System.IO;
using Volo.Abp;

namespace SecurityServer.Common;

public static class InputHelper
{
    
    public static string ReadText(string label = CommonConstant.EmptyString)
    {
        Console.Write(label);
        return Console.ReadLine() ?? CommonConstant.EmptyString;
    }
    
    public static string ReadPassword(string label = CommonConstant.EmptyString)
    {
        Console.Write(label);
        var pwd = CommonConstant.EmptyString;
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
            {
                pwd = pwd[..^1];
            }
            else if (key.Key != ConsoleKey.Backspace)
            {
                pwd += key.KeyChar;
            }
        }
        
        return pwd;
    }
    
    public static string ReadFile(string path)
    {
        AssertHelper.IsTrue(File.Exists(path), "File not exits " + path);
        using var textReader = File.OpenText(path);
        return textReader.ReadToEnd();
    }

    
}