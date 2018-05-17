/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : ScriptableObjectBase.cs
**********************************************************/
namespace ClientDataBase
{
    using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;

    public abstract class ScriptableDictionaryObject<TKey, TValue> : ScriptableObjectBase
        where TValue : TableClassBase
    {
        //[SerializeField]
        //protected SerializableDictionary<TKey, TValue> m_tableDict = new SerializableDictionary<TKey, TValue>();

        // 使用上面的会导致无法序列化，
        protected SerializableDictionary<TKey, TValue> m_tableDict = null;

        protected ScriptableDictionaryObject()
        {
            m_tableDict = GetAllData();
        }

#if UNITY_EDITOR

        public override bool LoadGameTable(bool bSaveToAsset)
        {
            ClientDataBaseConfig  clientDataBaseConfig = ClientDataBaseManager.Instance.m_config;
            string path = clientDataBaseConfig.GetGameTablePathName(GetTableName());

            TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            if (data == null)
            {
                Debug.LogError(string.Format("Can't found GameTable txt file in [Path:{0}]", path));
                return false;
            }

            if (!LoadGameTable(data.text))
            {
                return false;
            }

            if (bSaveToAsset)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                Debug.Log(string.Format("[{0}] GameTable Asset is Update. Source:[{1}]", this.name, path));
            }



            return true;
        }

#else
    public override bool LoadGameTable(bool bSaveToAsset) { return false; }
#endif
        
#if MGAME_DEBUG
        readonly Dictionary<string, bool> fieldWarningDict = new Dictionary<string, bool>();
#endif

        protected int GetFieldIndex(List<string> fieldNames, string fieldName, int exportIndex)
        {
#if MGAME_DEBUG
            // 这样就算在csv里改变顺序，或者新增列也没有关系
            int ii = fieldNames.FindIndex(a => a == fieldName);
            if (ii == -1)
            {
                Debug.LogError(string.Format("#TableCheck# 字段名[{0}]在表格[{1}]中找不到，请确证csv与脚本一一对应", fieldName, GetTableName()));
            }
            else
            {
                if (exportIndex != ii && !fieldWarningDict.ContainsKey(fieldName))
                {
                    fieldWarningDict.Add(fieldName, true);
                    Debug.LogError(string.Format("#TableCheck# 表格[{1}]的字段名[{0}]列号在表格与脚本里不一致,表格列({2})<==>脚本列({3}),如果是新增了非忽略列的话,请重新导出脚本", fieldName, GetTableName(), ii, exportIndex));
                }
                exportIndex = ii;
            }
#endif
            return exportIndex;
        }

        public TValue GetData(TKey id)
        {
            if (m_tableDict.ContainsKey(id))
            {
                return m_tableDict[id];
            }
            else
            {
                //-Debug.LogWarning(string.Format("[{0}] GameTable Asset Dictionary Can't found key [{1}]", GetTableName(), id));
                return null;
            }
        }

        public bool Containes(TKey id)
        {
            return m_tableDict.ContainsKey(id);
        }

        public virtual SerializableDictionary<TKey, TValue> GetAllData()
        {
            return null;
        }

        public override int Count
        {
            get
            {
                return m_tableDict.Count;
            }
        }

        public override void Validate()
        {
            ClientDataChecker.Instance.Check(this, m_tableDict.Values);
        }

        public override bool CheckField(string fieldName, string checkVal)
        {
            foreach (var oneValue in m_tableDict.Values)
            {
                object obj = oneValue.GetPropertyValue(fieldName);
                if (obj != null && obj.ToString() == checkVal)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
