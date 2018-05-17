/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : ClientDataBaseManager.cs
**********************************************************/
using System.Collections.Generic;
using System;

using ClientDataBase;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClientDataBaseManager : ClientDataBase.Singleton<ClientDataBaseManager>
{
    const string c_pathConfig = "Client DataBase Config";
    const string c_pathScriptableAssetRoot = "Generate/ClientDataBase/";
#if UNITY_EDITOR
    static int m_SimulateTableAssetInEditor = -1;
    const string kSimulateTableAsset = "SimulateTableAsset";
#endif

#if UNITY_EDITOR
    /// <summary>
    /// 这个标志用来指示是否模拟加载table asset资源，如果是就直接加载csv
    /// </summary>
    public static bool SimulateTableAssetInEditor
    {
        get
        {
            if (m_SimulateTableAssetInEditor == -1)
                m_SimulateTableAssetInEditor = EditorPrefs.GetBool(kSimulateTableAsset, true) ? 1 : 0;

            return m_SimulateTableAssetInEditor != 0;
        }
        set
        {
            int newValue = value ? 1 : 0;
            if (newValue != m_SimulateTableAssetInEditor)
            {
                m_SimulateTableAssetInEditor = newValue;
                EditorPrefs.SetBool(kSimulateTableAsset, value);
            }
        }
    }

    
#endif

    ClientDataBaseConfig _config;
    public ClientDataBaseConfig m_config
    {
        get
        {
            if (_config == null)
                _config = Utility.AssetRelate.ResourcesLoadCheckNull<ClientDataBaseConfig>(c_pathConfig);

            return _config;
        }
    }


    public Dictionary<Type, ScriptableObjectBase> m_tableList = new Dictionary<Type, ScriptableObjectBase>();

    public ClientDataBaseManager()
    {
    }

    static public ClientDataBaseManager Get()
    {
        return Instance;
    }


    /// <summary>
    /// 创建对象实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fullName">命名空间.类型名</param>
    /// <param name="assemblyName">程序集</param>
    /// <returns></returns>
    public ScriptableObjectBase CreateScriptableInstance(string fileName)
    {
        string path = m_config.GetScriptableScriptName(fileName);
        Type o = Type.GetType(path);//加载类型
        return (ScriptableObjectBase)ScriptableObject.CreateInstance(o);
        //object obj = Activator.CreateInstance(o);//根据类型创建实例
        //return (ScriptableObjectBase)obj;//类型转换并返回
    }

    public ScriptableObjectBase LoadTable(string fileName)
    {

        ScriptableObjectBase scriptable = null;
#if UNITY_EDITOR
        // 导表时就走下面流程
        if (!Application.isPlaying || ClientDataBaseManager.SimulateTableAssetInEditor)
        {
            scriptable = CreateScriptableInstance(fileName);
            if (scriptable == null)
            {
                Debug.LogError(string.Format("#TableCheck# 表[{0}]没有对应的脚本，请重新导表!!", fileName));
                return null;
            }
            scriptable.LoadGameTable(false);
            return scriptable;
        }
#endif

#if NO_UPDATE
        scriptable = Utility.AssetRelate.ResourcesLoadCheckNull<ScriptableObjectBase>(c_pathScriptableAssetRoot + m_config.GetScriptableAssetName(fileName));
#else
		//scriptable = (ScriptableObjectBase)RM.Instance.GetResource(c_pathScriptableAssetRoot + m_config.GetScriptableAssetName(fileName,true));
#endif

        return scriptable;
    }

    public void Register(Type type, ScriptableObjectBase dataBase)
    {
	    if (!m_tableList.ContainsKey(type))
	    {
		    m_tableList.Add(type, dataBase);
	    }
	    else
	    {
			// 由于在editor下打开另一个场景，会导致ScriptableObject会被destroy,所以做一下保护
			m_tableList[type] = dataBase;
	    }
    }

    /// <summary>
    /// 获得加载的table,没有加载的不会自动加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetLoadedTable<T>() where T : ScriptableObjectBase, new()
    {
        if (m_tableList.ContainsKey(typeof(T)))
        {
            return (T)m_tableList[typeof(T)];
        }

        return default(T);
    }

    public ScriptableObjectBase GetLoadedTable(string tableName)
    {
        foreach (var oneTable in m_tableList)
        {
            if (oneTable.Value.GetTableName() == tableName)
            {
                return oneTable.Value;
            }
        }
        return null;
    }

    public bool IsTableLoad(string tableName)
    {
        return GetLoadedTable(tableName) != null;
    }

    public ScriptableObjectBase GetOrCreateTable(string tableName)
    {
        var loadTable = GetLoadedTable(tableName);
        if (loadTable != null)
        {
            return loadTable;
        }

        var t = LoadTable(tableName); 
        if (t == null)
        {
            return null;
        }

        Register(t.GetType(), t);
        return t;
    }

    public ScriptableObjectBase CreateFromAssetName(string fileName)
    {
        return Utility.AssetRelate.ResourcesLoadCheckNull<ScriptableObjectBase>(c_pathScriptableAssetRoot + m_config.GetScriptableAssetName(fileName));
    }

    /// <summary>
    /// 注消时要clear, 有可能会有热更表格
    /// </summary>
    public void Clear()
    {
        m_tableList.Clear();
    }
}

