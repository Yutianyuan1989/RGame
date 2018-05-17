using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存取写入玩家注册表的相关数据
public class RegisterDataManager  {

    public const string AppPrefix = "TianYuan" + "_";
    /// <summary>
    /// 生成一个Key名
    /// </summary>
    public static string GetKey(string key)
    {
        return AppPrefix  + "_" + key;
    }

    /// <summary>
    /// 取得整型
    /// </summary>
    public static int GetInt(string key, int defaultValue = 0)
    {
        string name = GetKey(key);
        return PlayerPrefs.GetInt(name, defaultValue);
    }

    /// <summary>
    /// 取得浮点型
    /// </summary>
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        string name = GetKey(key);
        return PlayerPrefs.GetFloat(name, defaultValue);
    }

    /// <summary>
    /// 有没有值
    /// </summary>
    public static bool HasKey(string key)
    {
        string name = GetKey(key);
        return PlayerPrefs.HasKey(name);
    }

    /// <summary>
    /// 保存整型
    /// </summary>
    public static void SetInt(string key, int value)
    {
        string name = GetKey(key);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetInt(name, value);
    }

    /// <summary>
    /// 保存浮点型
    /// </summary>
    public static void SetFloat(string key, float value)
    {
        string name = GetKey(key);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetFloat(name, value);
    }

    /// <summary>
    /// 取得数据
    /// </summary>
    public static string GetString(string key)
    {
        string name = GetKey(key);
        return PlayerPrefs.GetString(name);
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public static void SetString(string key, string value)
    {
        string name = GetKey(key);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetString(name, value);
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    public static void RemoveData(string key)
    {
        string name = GetKey(key);
        PlayerPrefs.DeleteKey(name);
    }
}
