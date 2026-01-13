using System.IO;
using UnityEngine;

public static class GamePaths
{
    // Keep all names in one place so menu/pause/save load the same scenes/files.
    public const string MenuScene = "MenuScene";
    public const string MainGameScene = "MainGameScene";

    public const string SaveFileName = "save.json";

    public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool HasSave() => File.Exists(SavePath);
}
