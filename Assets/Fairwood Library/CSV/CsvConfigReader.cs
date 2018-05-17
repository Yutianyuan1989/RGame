using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumenWorks.Framework.IO.Csv;
using UnityEngine;

namespace Edu.CSV
{
    /// <summary>
    /// csv config reader
    /// <example>
    /// read a file:
    /// <para>string path = Environment.CurrentDirectory + "/config.csv";   </para>
    /// <para>if (!File.Exists(path))                                       </para>
    /// <para>{                                                             </para>
    /// <para>    throw new Exception("file not found");                    </para>
    /// <para>}                                                             </para>
    /// <para><see cref="CsvConfigReader"/> reader = new <see cref="CsvConfigReader"/>();               </para>
    /// <para>reader.<see cref="ReadFile"/>(path);                                            </para>
    /// <para>you can get main key type: reader.<see cref="KeyType"/>       </para>
    /// <para>you can get your value :   reader.[index]["name"].Value;</para>
    /// </example>
    /// </summary>
    public class CsvConfigReader : IEnumerable<CsvConfig>
    {
        private static Dictionary<string, CsvValue.CsvValueType> _typeDic = new Dictionary<string, CsvValue.CsvValueType>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"b", CsvValue.CsvValueType.Bool},
            {"i", CsvValue.CsvValueType.Int},
            {"ni", CsvValue.CsvValueType.Int},
            {"n", CsvValue.CsvValueType.Int},   // 统一为整数
            {"l", CsvValue.CsvValueType.Long},
            {"f", CsvValue.CsvValueType.Float},
            {"d", CsvValue.CsvValueType.Double},
            {"s", CsvValue.CsvValueType.String},
            {"x", CsvValue.CsvValueType.VFixedPoint},
            {"ai",CsvValue.CsvValueType.ArrayInt},         //-- add by lxh
		    {"ia",CsvValue.CsvValueType.ArrayInt},         //-- add by lxh
            {"an",CsvValue.CsvValueType.ArrayInt},
            {"af",CsvValue.CsvValueType.ArrayFloat},
            {"as",CsvValue.CsvValueType.ArrayString},
            {"ax",CsvValue.CsvValueType.ArrayVFixedPoint}, //--
            {"o", CsvValue.CsvValueType.None},
        };


        private static Dictionary<string,CsvValue.CsvValueType> _keyTypeDic = new Dictionary<string, CsvValue.CsvValueType>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"ki",CsvValue.CsvValueType.Int},
            {"kn",CsvValue.CsvValueType.Int},//为了和lua版统一
            {"kl",CsvValue.CsvValueType.Long},
            {"ks",CsvValue.CsvValueType.String},
            {"kx",CsvValue.CsvValueType.VFixedPoint},
            {"kai",CsvValue.CsvValueType.ArrayInt}, // below add by lxh
            {"kan",CsvValue.CsvValueType.ArrayInt},
            {"kal",CsvValue.CsvValueType.ArrayLong},
            {"kas",CsvValue.CsvValueType.ArrayString},
            {"kax",CsvValue.CsvValueType.ArrayVFixedPoint},
        };

        private readonly Dictionary<CsvValue,CsvConfig> _configDic = new Dictionary<CsvValue, CsvConfig>();
        private readonly Dictionary<string, Dictionary<CsvValue, List<CsvConfig>>> _indexDic = new Dictionary<string, Dictionary<CsvValue, List<CsvConfig>>>();

        private CsvValue.CsvValueType _keyType = CsvValue.CsvValueType.None;
        /// <summary>
        /// main key type.
        /// can be int ,long ,string
        /// </summary>
        public CsvValue.CsvValueType KeyType
        {
            get { return _keyType; }
        }
        /// <summary>
        /// main key type
        /// you can use like this:
        /// int intKey = Keys[index];
        /// int longKey = Keys[index];
        /// string strKey = Keys[index];
        /// </summary>
        public List<CsvValue> Keys
        {
            get
            {
                List<CsvValue> list = new List<CsvValue>();
                foreach (CsvValue key in _configDic.Keys)
                {
                    list.Add(key);
                }
                return list;
            }
        }

        private readonly List<string> _fieldNames = new List<string>(); 
        /// <summary>
        /// it contains main keys you defined.
        /// </summary>
        public List<string> FieldNames
        {
            get { return _fieldNames; }
        } 

        /// <summary>
        /// if <see cref="KeyType"/>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CsvConfig this[CsvValue key]
        {
            get
            {
                foreach (KeyValuePair<CsvValue, CsvConfig> pair in _configDic)
                {
                    if (pair.Key.Equals(key))
                    {
                        return pair.Value;
                    }
                }
                return null;
            }
        }
        public CsvConfigReader()
        {
           Reset();
        }

        private void Reset()
        {
            _configDic.Clear();
            _indexDic.Clear();
            _keyType = CsvValue.CsvValueType.None;
            _fieldNames.Clear();
        }

        /// <summary>
        /// 根据配表获得相应的类型
        /// </summary>
        /// <param name="key">配表的键</param>
        /// <returns></returns>
        public static CsvValue.CsvValueType GetCsvValueType(string key)
        {
            CsvValue.CsvValueType type = CsvValue.CsvValueType.None;

            do
            {
                // 普通类型的查找
                if (_typeDic.TryGetValue(key, out type))
                {
                    break;
                }

                // 主键的查找
                if (_keyTypeDic.TryGetValue(key, out type))
                {
                    break;
                }
				Debug.LogError("该导表类型不存在:" + key);

            }
            while (false);

            return type;
        }


        /// <summary>
        /// 是否是主键
        /// </summary>
        /// <param name="key">配表的键</param>
        /// <returns></returns>
        public static bool IsKeyType(string key)
        {
            return _keyTypeDic.ContainsKey(key);
        }

        /// <summary>
        /// 是否是索引类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsIndexType(string key)
        {
            if (string.IsNullOrEmpty(key) || key[key.Length - 1] != 'i')
            {
                return false;
            }
            key = key.TrimEnd('i');

            return _typeDic.ContainsKey(key);

        }
        public void ReadFile(string path)
        {
            using ( FileStream fs = new FileStream(path,FileMode.Open))
            {
                using ( StreamReader sr = new StreamReader(fs))
                {
                    Read(sr);
                    sr.Close();
                }
                fs.Close();
            }
        }

        public void Read(TextReader reader)
        {
            Reset();
            try
            {
                CsvReader csvReader = new CsvReader(reader, false);
                int fieldCount = csvReader.FieldCount;

                bool hasKey = false;
                int keyIndex = -1;
                Dictionary<int, CsvValue.CsvValueType> typeDic = new Dictionary<int, CsvValue.CsvValueType>();

                bool hasName = false;
                Dictionary<int, string> nameDic = new Dictionary<int, string>();
                Dictionary<int, bool> buildIndexDic = new Dictionary<int, bool>();
                foreach (string[] strings in csvReader)
                {
                    //check fieldCount
                    if (strings.Length != fieldCount)
                    {
                        StringBuilder sb = new StringBuilder();
                        bool first = true;
                        foreach (string s in strings)
                        {
                            if (first)
                            {
                                sb.Append(s);
                                first = false;
                            }
                            else
                            {
                                sb.Append(",").Append(s);
                            }
                        }
                        throw new CsvException(string.Format("fields length error,{0}", sb.ToString()));
                    }

                    //set key
                    if (!hasKey)
                    {
                        for (int i = 0; i < fieldCount; i++)
                        {
                            string low = strings[i].ToLower();
                            if (!_typeDic.ContainsKey(low) && !_keyTypeDic.ContainsKey(low) && low.EndsWith("i"))
                            {
                                
                                buildIndexDic.Add(i, true);
                                low = low.TrimEnd('i');
                            }
                            else
                                buildIndexDic.Add(i, false);
                            
                            if (_typeDic.ContainsKey(low))
                            {
                                typeDic.Add(i, _typeDic[low]);
                            }
                            else if (_keyTypeDic.ContainsKey(low))
                            {
                                if (hasKey)
                                {
                                    throw new CsvException(string.Format("already has key{0}", low));
                                }
                                hasKey = true;
                                keyIndex = i;
                                typeDic.Add(i, _keyTypeDic[low]);
                                _keyType = _keyTypeDic[low];
                            }
                            else
                            {
                                throw new CsvException(string.Format("error header:{0}", low));
                            }

                        }
                        hasKey = true;
                        continue;
                    }

                    //set names
                    if (!hasName)
                    {
                        for (int i = 0; i < fieldCount; i++)
                        {
                            string name = strings[i];
                            if (_fieldNames.Contains(name))
                            {
                                throw new CsvException(string.Format("same field name:{0}", name));
                            }
                            if(buildIndexDic[i])
                            {
                                _indexDic[name] = new Dictionary<CsvValue, List<CsvConfig>>(); 
                            }
                            nameDic.Add(i, name);
                            _fieldNames.Add(name);
                        }
                        hasName = true;
                        continue;
                    }

                    CsvConfig config = new CsvConfig();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        string value = strings[i];
                        CsvValue csvValue = new CsvValue(typeDic[i], value);
                        CsvConfigField field = new CsvConfigField(i, i == keyIndex, buildIndexDic[i], nameDic[i],
                           csvValue);
                        config.AddField(field);
                        if (buildIndexDic[i])
                        {
                            bool alreadyContanValue = false;
                            CsvValue indexValue = null;
                            foreach (KeyValuePair<CsvValue, List<CsvConfig>> pair in _indexDic[nameDic[i]])
                            {
                                if (pair.Key.Equals(csvValue))
                                {
                                    alreadyContanValue = true;
                                    indexValue = pair.Key;
                                }
                            }
                            if(!alreadyContanValue)
                            {
                                _indexDic[nameDic[i]][csvValue] = new List<CsvConfig>();
                                _indexDic[nameDic[i]][csvValue].Add(config);
                            }
                            else
                            {
                                _indexDic[nameDic[i]][indexValue].Add(config);
                            }
                                
                        }
                    }

                    CsvValue keyValue = new CsvValue(typeDic[keyIndex], strings[keyIndex]);
                    foreach (KeyValuePair<CsvValue, CsvConfig> pair in _configDic)
                    {
                        if (pair.Key.Equals(keyValue))
                        {
                            throw new CsvException(string.Format("already contains key :{0}", keyValue.Value));
                        }
                    }
                    _configDic.Add(keyValue, config);
                    
                }
                if (!hasKey)
                {
                    throw new CsvException("miss key in header.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw new CsvException(e.Message);
            }
        }

        public List<CsvConfig> GetConfigsByIndex(string fieldName, CsvValue value)
        {
            if (_indexDic.ContainsKey(fieldName) == false)
                return null;
            Dictionary<CsvValue, List < CsvConfig >> dic = _indexDic[fieldName];
            foreach (var iter in dic)
            {
                if (iter.Key.Equals(value))
                    return dic[iter.Key];
            }
            return null;
        }

        public IEnumerator<CsvConfig> GetEnumerator()
        {
            return _configDic.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _configDic.Values.GetEnumerator();
        }
    }
}
