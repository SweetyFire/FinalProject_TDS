using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    private const char SEPARATOR = '=';

    private static string SavePath
    {
        get
        {
#pragma warning disable IDE0066
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsEditor:
                    return Path.Combine(Application.dataPath, "Editor", "Saves");

                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.WindowsPlayer:
                    return Path.Combine(Application.dataPath, "Saves");

                default:
                    return Path.Combine(Application.persistentDataPath, "Saves");
            }
#pragma warning restore IDE0066
        }
    }

    private static string MainFilePath => Path.Combine(SavePath, "main.sav");

    private static readonly string _savePath;
    private static readonly string _mainFilePath;
    private static readonly StringBuilder _stringBuilder = new();

    private static readonly Dictionary<string, string> _mainValues = new();

    private static bool _dataLoaded = false;

    static SaveSystem()
    {
        _savePath = SavePath;
        _mainFilePath = MainFilePath;
    }

    public static void SetValue(string key, object value)
    {
        LoadFirstTime();
        if (_mainValues.ContainsKey(key))
        {
            _mainValues[key] = value.ToString();
        }
        else
        {
            _mainValues.Add(key, value.ToString());
        }
    }

    public static void SetValue(string key, ValueType value)
    {
        LoadFirstTime();
        if (_mainValues.ContainsKey(key))
        {
            _mainValues[key] = value.ToString();
        }
        else
        {
            _mainValues.Add(key, value.ToString());
        }
    }


    public static bool TryGetInt(string key, out int value)
    {
        value = default;
        if (!File.Exists(_mainFilePath)) return false;

        LoadFirstTime();
        if (_mainValues.ContainsKey(key))
        {
            string stringValue = _mainValues[key];

            if (!int.TryParse(stringValue, out value))
            {
                Debug.LogWarning($"Key \"{key}\" isn't integer");
                return false;
            }

            return true;
        }
        else
        {
            Debug.LogWarning($"Saved int value with key \"{key}\" doesn't exists");
        }

        return false;
    }

    public static int GetInt(string key)
    {
        if (TryGetInt(key, out int value))
            return value;

        return default;
    }

    public static bool TryGetFloat(string key, out float value)
    {
        value = default;
        if (!File.Exists(_mainFilePath)) return false;

        LoadFirstTime();
        if (_mainValues.ContainsKey(key))
        {
            string stringValue = _mainValues[key];

            if (!float.TryParse(stringValue, out value))
            {
                Debug.LogWarning($"Key \"{key}\" isn't float");
                return false;
            }

            return true;
        }
        else
        {
            Debug.LogWarning($"Saved float value with key \"{key}\" doesn't exists");
        }

        return false;
    }

    public static float GetFloat(string key)
    {
        if (TryGetFloat(key, out float value))
            return value;

        return default;
    }

    public static bool TryGetChar(string key, out char value)
    {
        value = default;
        if (!File.Exists(_mainFilePath)) return false;

        LoadFirstTime();
        if (_mainValues.ContainsKey(key))
        {
            string stringValue = _mainValues[key];

            if (!char.TryParse(stringValue, out value))
            {
                Debug.LogWarning($"Key \"{key}\" isn't char");
                return false;
            }

            return true;
        }
        else
        {
            Debug.LogWarning($"Saved char with key \"{key}\" doesn't exists");
        }

        return false;
    }

    public static char GetChar(string key)
    {
        if (TryGetChar(key, out char value))
            return value;

        return default;
    }

    public static bool TryGetString(string key, out string value)
    {
        value = default;
        if (!File.Exists(_mainFilePath)) return false;

        LoadFirstTime();
        if (_mainValues.ContainsKey(key))
        {
            value = _mainValues[key];
            return true;
        }
        else
        {
            Debug.LogWarning($"Saved string with key \"{key}\" doesn't exists");
        }

        return false;
    }

    public static string GetString(string key)
    {
        if (TryGetString(key, out string value))
            return value;

        return default;
    }


    public static void Save()
    {
        LoadFirstTime();

        File.WriteAllText(_mainFilePath, string.Empty);
        using (StreamWriter writer = new(_mainFilePath))
        {
            foreach (var value in _mainValues)
            {
                writer.WriteLine(GetKeyValueString(value.Key, value.Value));
            }
        }

        _stringBuilder.Clear();
    }

    public static void Load()
    {
        string directory = Path.GetDirectoryName(_mainFilePath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (!File.Exists(_mainFilePath))
        {
            FileStream file = File.Create(_mainFilePath);
            file.Close();
        }
        else
        {
            using (StreamReader reader = new(_mainFilePath))
            {
                string line;
                int lineNumber = 1;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == string.Empty) continue;

                    string[] split = line.Split(SEPARATOR);
                    if (split.Length < 2)
                        throw new Exception($"Key \"{split[0]}\" is incorrect on line {lineNumber} in path {_mainFilePath}");

                    _stringBuilder.Clear();
                    for (int i = 1; i < split.Length; i++)
                    {
                        _stringBuilder.Append(split[i]);
                    }

                    if (_mainValues.ContainsKey(split[0]))
                    {
                        _mainValues[split[0]] = _stringBuilder.ToString();
                    }
                    else
                    {
                        _mainValues.Add(split[0], _stringBuilder.ToString());
                    }

                    lineNumber++;
                }
            }
        }

        _stringBuilder.Clear();
    }


    private static void LoadFirstTime()
    {
        if (_dataLoaded) return;
        Load();
        _dataLoaded = true;
    }

    private static string GetKeyValueString(string key, string value)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(key);
        _stringBuilder.Append(SEPARATOR);
        _stringBuilder.Append(value);
        return _stringBuilder.ToString();
    }
}
