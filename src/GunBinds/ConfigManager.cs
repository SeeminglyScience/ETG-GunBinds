using System;
using System.IO;
using UnityEngine;

namespace GunBinds;

public static class ConfigManager
{
    public static ConfigManager<T> Create<T>(string modName, string configName, Func<T> defaultFactory)
    {
        return ConfigManager<T>.Create(modName, configName, defaultFactory);
    }
}

public sealed class ConfigManager<T>
{
    private readonly string _configName;

    private readonly string _configDirectory;

    private readonly string _configPath;

    public T Value;

    private ConfigManager(string modName, string configName)
    {
        if (modName is null or { Length: 0 })
        {
            throw new ArgumentNullException(nameof(modName));
        }

        if (configName is null or { Length: 0 })
        {
            throw new ArgumentNullException(nameof(configName));
        }

        _configName = configName;

        _configDirectory = Path.Combine(ETGMod.ResourcesDirectory, modName);
        _configPath = Path.Combine(_configDirectory, configName + ".json");
    }

    internal static ConfigManager<T> Create(string modName, string configName, Func<T> defaultFactory)
    {
        ConfigManager<T> config = new(modName, configName);
        config.Init(defaultFactory);
        return config;
    }

    public void Save()
    {
        string contents = JsonUtility.ToJson(Value);
        File.WriteAllText(_configPath, contents);
    }

    private void Init(Func<T> defaultFactory)
    {
        Directory.CreateDirectory(_configDirectory);
        if (File.Exists(_configPath))
        {
            string contents = File.ReadAllText(_configPath);
            if (contents is null or { Length: 0 })
            {
                BackupInvalid();
                InitDefault(defaultFactory);
                return;
            }

            T config = default;
            try
            {
                config = JsonUtility.FromJson<T>(contents);
            }
            catch (Exception e)
            {
                Log.Info("Failed to load configuration, reverting to default.");
                Log.Exception(e);
                BackupInvalid();
                InitDefault(defaultFactory);
                return;
            }

            Value = config;
            return;
        }

        InitDefault(defaultFactory);
    }

    private void InitDefault(Func<T> defaultFactory)
    {
        T value = defaultFactory();
        string contents = JsonUtility.ToJson(value);
        File.WriteAllText(_configPath, contents);
    }

    private void BackupInvalid()
    {
        File.Move(_configPath, _configName + ".bad.json");
    }
}
