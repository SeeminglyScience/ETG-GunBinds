using System;
using System.IO;

namespace GunBinds;
public static class Log
{
    private static readonly string s_defaultLog = Path.Combine(ETGMod.ResourcesDirectory, "gunBinds.txt");

    public static void Init()
    {
        if (!File.Exists(s_defaultLog))
        {
            return;
        }

        File.Delete(s_defaultLog);
    }

    public static void Exception(Exception e)
    {
        Info(e.Message);
        Info(e.StackTrace);
    }

    public static void Info(string message)
    {
        using StreamWriter writer = new(s_defaultLog, append: true);
        writer.WriteLine(message);
    }
}
