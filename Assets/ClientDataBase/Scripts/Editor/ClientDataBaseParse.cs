/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : ClientDataBaseParse.cs
**********************************************************/
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Globalization;
using LumenWorks.Framework.IO.Csv;

namespace ClientDataBase
{
    using System.Linq;

    using Edu.CSV;

    public class ClientDataBaseParse : Singleton<ClientDataBaseParse>
    {

        static private string[] Names = new string[]
    {
        "StringDialogue",
        "StringUI",
        "StringPlayerName",
        "StringHero",
        "StringSkill",
        "StringEvent",
        "StringTips",
        "StringMap",
        "StringAccount",
        "StringTalent",
        "StringError",
        "StringAIName",
        "StringItem",
        "StringBattleItem",
        "StringQuest",
        "StringRankMedal",
        "StringSkin",
        "StringShare",
        "StringBattlechat",
        "StringBroadcast",
        "StringAchieve",
        "StringFirstName",
        "StringHyperlink",
    };

        /// <summary>
        /// 忽略的类型
        /// </summary>
        public const string IgnoreType = "None";

        public class TableData
        {
            public TableData(string summary, string name, string type, bool isArray)
            {
                this.summary = summary;
                this.name = name;
                this.type = type;
                this.isArray = isArray;
            }

            public string summary;
            public string name;
            public string type;
	        public bool isArray; 
            public bool isEnd;
			public bool isKey;		// 是否是主键

	        public bool IsArray()
	        {
		        return isArray;
	        }
        }

        string _tableName;
        ClientDataBaseConfig _config;


        /// <summary>
        /// 讀取GameTable(.txt)
        /// </summary>
        public bool LoadGameTable(Object obj, bool exportCSharp, bool exportServer, bool exportLua)
        {
            _config = ClientDataBaseManager.Instance.m_config;
            _tableName = obj.name;

            //-string strTemp;
            TextAsset data = (TextAsset)obj;
            TextReader reader = null;

            string[] _Summary = null;
            string[] _Variable = null;
            string[] _ErlangVariable = null;
            string[] _LuaVariable = null;
            string[] _Type = null;
            int index = 0;


            if (data == null)
            {
                Debug.LogError("GameTable is null.");
                return false;
            }

            if (data.text == string.Empty)
            {
                Debug.LogError("GameTable is empty.");
                return false;
            }

            reader = new StringReader(data.text);
            if (reader != null)
            {
				int keyCol = -1;
                //while ((strTemp = reader.ReadLine()) != null)
                CsvReader csvReader = new CsvReader(reader, false);
                foreach (string[] splitStr in csvReader)
                {
                    if (index == 0)
                    {
                        _Type = splitStr.Select(a => a.Trim()).ToArray();
                        _Summary = _Type.ToArray();

                        // convert to real type
                        for (var i = 0; i < _Type.Length; i++)
                        {
							//检查多个主键
							if( CsvConfigReader.IsKeyType(_Type[i]) )
							{
								if(keyCol>=0)
								{
									Debug.LogError(string.Format("Duplicate key type conflict in table:[{0}]  col:[{1}] with col[{2}]", _tableName, keyCol, index));
									return false;
								}
								keyCol = i;
							}
							
                            _Type[i] = ConvertCsvTypeToCsType(_Type[i]);
                        }
                        
                        index++;
                        continue;
                    }

                    if (index == 1)
                    {
                        _Variable = splitStr.Select(a => a.Trim()).ToArray();

                        _ErlangVariable = splitStr.ToArray();

                        // 解决字段有空格的问题
                        for (int i = 0; i < _Variable.Length; i++)
                        {
                            _ErlangVariable[i] = Utility.TypeRelate.CorrectFieldName(_ErlangVariable[i],"_").ToLower();
                            _Variable[i] = Utility.TypeRelate.CorrectFieldName(_Variable[i], "");
                        }
                        _LuaVariable = _Variable.Select(FirstCharToUpper).ToArray();


                        //1.判斷是否是 GameTable(txt)，檔案的開始字串是否包含 識別字
                        if (!string.IsNullOrEmpty(_config.m_gameTableCheck) && _Summary[0].IndexOf(_config.m_gameTableCheck) < 0)
                        {
                            Debug.LogError("GameTable is not a table. Please Check txt file start string is [" + _config.m_gameTableCheck + "]");
                            return false;
                        }

                        //2.判斷欄位數量是否一致
                        int count = _Summary.Length;
                        if (count != _Variable.Length || count != _Type.Length)
                        {
                            Debug.LogError("GameTable column not same.");
                            return false;
                        }

                        if (!exportCSharp)
                        {
                            break;
                        }

                        Dictionary<string, TableData> datamap = CreateTableScript(_Summary, _Variable, _Type);
                        if (datamap == null)
                        {
                            return false;
                        }

                        if (CreateScriptableScript(_Variable, datamap, keyCol) == false)
                        {
                            return false;
                        }

                        if (CreateScriptableScriptEditor() == false)
                        {
                            return false;
                        }

                        break;
                    }
                }
                reader.Close();
            }
            
            if (exportServer)
            {
                ExportServerConfig(_Summary, _ErlangVariable, _Type, data.text);
            }

            if (exportLua)
            {
                ExportLuaConfig(_Summary, _LuaVariable, _Type, data.text);
            }

            return true;
        }
        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("存在空列");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        /// <summary>
        /// 建立 Table Class
        /// </summary>
        /// <returns>返回该表的拥有的类型</returns>
        Dictionary<string, TableData> CreateTableScript(string[] summary, string[] variable, string[] type)
        {
            string templateDataClass = GetTemplate("TableClass");
	        if (string.IsNullOrEmpty(templateDataClass))
	        {
				return null;
			}

			Dictionary<string, TableData> dataMap = new Dictionary<string, TableData>();

			templateDataClass = templateDataClass.Replace("$ClassName", _config.GetTableClassScriptName(_tableName));

			//// 第一行内容
			//string[] firstConents = lineStr.Split(","[0]);

			StringBuilder field = new StringBuilder();

            for (int i = 0; i < variable.Length; i++)
            {
                if (type[i] == IgnoreType)
                {
					Debug.Log("Ignore variable name:" + variable[i]);
                    continue;
                }
                //透過字元 '[' ']' 判斷是否是Array 
                bool isArray = type[i].EndsWith("[]");

	            // 字段名
				string fieldName = variable[i];

				if (dataMap.ContainsKey(fieldName))
				{
					Debug.LogError(string.Format("Duplicate variable name and is not Array, table:[{0}], variable name:[{1}].", _tableName, fieldName));
					return null;
				}
				else
	            {
					dataMap.Add(fieldName, new TableData(summary[i], fieldName, type[i], isArray));
				}
            }

            foreach (KeyValuePair<string, TableData> item in dataMap)
            {
                field.Append(GetProperty(item.Value.summary, item.Value.name, item.Value.type, item.Value.IsArray(), item.Value.isEnd));
            }

            templateDataClass = templateDataClass.Replace("$MemberFields", field.ToString());

            // 如果是字符串表的话，只单单生成String表，别的都重复的
            if (!IsSequenceTable(_tableName))
            {
                UtilityEditor.CreateFolder(_config.GetTableClassPath());
                using (var writer = new StreamWriter(_config.GetTableClassPath() + _config.GetTableClassScriptName(_tableName, true)))
                {
                    writer.Write(templateDataClass);
                    writer.Close();
                }

                AssetDatabase.Refresh();
                Debug.Log(string.Format("{0} created, path : {1}", _config.GetTableClassScriptName(_tableName, true), _config.GetTableClassPath()));
            }
            

            return dataMap;
        }

        /// <summary>
        /// 建立 Scriptable Script
        /// </summary>
        /// <returns>是否成功建立</returns>
        bool CreateScriptableScript(string[] variable, Dictionary<string, TableData> dataMap, int keyCol)
        {
            //TODO @jiake 是否考虑做个映射表？
            string templateName = _tableName;
            string tableClassName = _config.GetScriptableScriptName(_tableName);
            string dataClassName = _config.GetTableClassScriptName(_tableName);
            string tableBaseClassName = string.Empty;
            bool existKey = (keyCol >= 0);

            if (!TryParseSequenceTable(_tableName, ref templateName, ref tableBaseClassName, ref dataClassName))
            {
                if (Names.Contains(_tableName))
                {
                    templateName = "StringScriptable";
                }
                else if (!ExistTemplate(templateName))
                {
                    templateName = (existKey ? "ScriptableWithKey" : "Scriptable");
                }
            }

            string template = GetTemplate(templateName);
            if (string.IsNullOrEmpty(template))
            {
                return false;
            }

            template = template.Replace("$ScriptableName", tableClassName);
            template = template.Replace("$ScriptableBaseName", tableBaseClassName);
            template = template.Replace("$GameTableName", _tableName);
            template = template.Replace("$ClassName", dataClassName);
            template = template.Replace(
                "$GameTablePath",
                "Config.GameTablePath + GameTableName + Config.FILE_EXTENSION_TXT");
            if (existKey)
            {
                string fieldName = variable[keyCol];
                TableData tableData = dataMap[fieldName];
                template = template.Replace("$KeyTypeName", tableData.type);
                template = template.Replace("$KeyFieldName", variable[keyCol]);
            }

            Dictionary<string, string> variableMap = new Dictionary<string, string>();

            for (int i = 0; i < variable.Length; i++)
            {
                string fieldName = variable[i];

                // 不处理忽略字段
                if (!dataMap.ContainsKey(fieldName))
                {
                    continue;
                }
                TableData tableData = dataMap[fieldName];
                if (tableData.type == IgnoreType)
                {
                    continue;
                }

                var resultStr = GetDataClassDetial(i, fieldName, tableData);
                variableMap.Add(fieldName, resultStr);
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> item in variableMap)
            {
                sb.Append(item.Value);
            }
            template = template.Replace("$DataLoad", sb.ToString());


            // 字段验证
            StringBuilder checkFieldSb = new StringBuilder();
            int checkFieldIndex = 1;
            foreach (KeyValuePair<string, string> item in variableMap)
            {
                checkFieldSb.Append(string.Format("\"{0}\"", item.Key));
                if (checkFieldIndex != variableMap.Count)
                {
                    checkFieldSb.Append(",");
                }

                checkFieldIndex++;
            }
            template = template.Replace("$FieldCheck", checkFieldSb.ToString());

            UtilityEditor.CreateFolder(_config.GetScriptableScriptsPath());
            using (var writer = new StreamWriter(_config.GetScriptableScriptsPath() + _config.GetScriptableScriptName(_tableName, true)))
            {
                writer.Write(template);
                writer.Close();
            }

            AssetDatabase.Refresh();
            Debug.Log(string.Format("[Scriptable Script] is Create.\nFile:[{0}] Path:[{1}]", _config.GetScriptableScriptName(_tableName, true), _config.GetScriptableScriptsPath()));

            return true;
        }

        /// <summary>
        /// 序列表头
        /// </summary>
        public static readonly string[] SequenceTables = { "IOExp", "String", "CharacterUltCharge" };

        /// <summary>
        /// 检查并返回序列表的模版
        /// </summary>
        /// <returns></returns>
        bool TryParseSequenceTable(string tableName, ref string templateName, ref string tableBaseClassName, ref string dataClassName)
        {
            foreach(var def in SequenceTables)
            {
                if (tableName.StartsWith(def))
                {
                    templateName = string.Format("Sequence{0}", (tableName.Length == def.Length ? "" : "$"));
                    tableBaseClassName = _config.GetScriptableScriptName(def);
                    dataClassName = _config.GetTableClassScriptName(def);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否为序列表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool IsSequenceTable(string tableName)
        {
            return SequenceTables.Any(delegate (string s) { return tableName.StartsWith(s) && tableName.Length != s.Length ; });
        }

        /// <summary>
        /// 建立 Scriptable Script Editor
        /// </summary>
        /// <returns>是否成功建立</returns>
        bool CreateScriptableScriptEditor()
        {
            string templateScriptable = GetTemplate("ScriptableEditor");
            if (string.IsNullOrEmpty(templateScriptable))
                return false;

            templateScriptable = templateScriptable.Replace("$ScriptableEditorName", _config.GetScriptableScriptEditorName(_tableName));
            templateScriptable = templateScriptable.Replace("$ScriptableName", _config.GetScriptableScriptName(_tableName));


            UtilityEditor.CreateFolder(_config.GetScriptableEditorPath());
            using (var writer = new StreamWriter(_config.GetScriptableEditorPath() + _config.GetScriptableScriptEditorName(_tableName, true)))
            {
                writer.Write(templateScriptable);
                writer.Close();
            }

            AssetDatabase.Refresh();
            Debug.Log(string.Format("[Scriptable Script Editor] is Create.\nFile:[{0}] Path:[{1}]", _config.GetScriptableScriptEditorName(_tableName, true), _config.GetScriptableEditorPath()));

            return true;
        }

        /// <summary>
        /// 建立 Scriptable Asset
        /// </summary>
        /// <returns>是否成功建立</returns>
        public bool CreateScriptableAssets(string scriptableScriptName, string scriptableAssetName)
        {
            _config = ClientDataBaseManager.Instance.m_config;
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(_config.GetScriptableScriptsPath() + scriptableScriptName);

            if (script == null || script.GetClass() == null)
            {
                Debug.LogError(string.Format("Scriptable Script is Null. [Path:{0}]", _config.GetScriptableScriptsPath() + scriptableScriptName));
                return false;
            }

            string path = _config.GetScriptableAssetPath() + scriptableAssetName;
            UtilityEditor.CreateFolder(_config.GetScriptableAssetPath());

            Object _Object = ScriptableObject.CreateInstance(script.GetClass());
            AssetDatabase.CreateAsset(_Object, path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            Debug.Log(string.Format("[Scriptable Asset] is Create.\nFile:[{0}] Path:[{1}]", scriptableAssetName, _config.GetScriptableAssetPath()));

            //資料讀取
            ScriptableObjectBase scriptableObjectBase = AssetDatabase.LoadAssetAtPath<ScriptableObjectBase>(path);
            
            return scriptableObjectBase.LoadGameTable(true);
        }

		bool ExistTemplate(string name)
		{
			string path = _config.GetTemplatePathName(name);
            if (path.StartsWith("Assets/"))
                path = path.Replace("Assets/", "");
			
			path = Application.dataPath + "/" + path;

			return System.IO.File.Exists(path);
		}
        /// <summary>
        ///取得 Script Templates
        /// </summary>
        string GetTemplate(string name)
        {
            string path = _config.GetTemplatePathName(name);
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            if (textAsset == null)
            {
                Debug.LogError(string.Format("Can't found Script Templates txt file in [Path:{0}]", path));
                return null;
            }

            return textAsset.ToString();
        }

        /// <summary>
        ///設定各屬性
        /// </summary>
        string GetProperty(string summary, string name, string type, bool isArray, bool isEnd = false)
        {
            string templateProperty = GetTemplate("TableClassProperty");
            templateProperty = templateProperty.Replace("$Summary", summary);
            templateProperty = templateProperty.Replace("$Modifier", "public");
            string typeInCs = type;
            templateProperty = templateProperty.Replace("$Type", typeInCs);
            templateProperty = templateProperty.Replace("$Name", name);
            templateProperty += isEnd ? string.Empty : "\n\n";
            return templateProperty;
        }

        /// <summary>
        /// 實作 TableClass 細部
        /// </summary>
        string GetDataClassDetial(int index, string name, TableData tableData)
        {
	        bool isArray = tableData.IsArray();

			//如果是Array，去除中括號
			string type = tableData.IsArray() ? tableData.type.Replace("[]", string.Empty) : tableData.type;
            if (isArray)
            {
                //string format = "table.{0} = new {1}[{3}];for (int i = 0; i < {3}; i++){table.{0}[i] = {2};}\n";
                string format = @"					{{
                        fieldName = ""{0}"";                        
                        typename = ""{4}"";  
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, {3});   
						string[] splitArray = splitStr[fieldIndex].Split(';', ',');
						int len = splitArray.Length;		

                        table.{0} = new {1}[len];
                      
						for (int i = 0; i < len; i++)
						{{
                            trimedStr = splitArray[i].Trim();
							table.{0}[i] = {2};
						}}
					}}
";

                //return string.Format(format, name, type, GetTypeDataClass(index, type), isArray);
                return string.Format(format, name, type, GetTypeDataClass("trimedStr", type), index, tableData.type);
            }
            else
            {
                string format = @"					{{
						fieldName = ""{0}"";                        
                        typename = ""{3}"";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, {2});    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.{0} = {1};
					}}
";
                //-string str = string.Format("splitStr[{0}]", index);
                return string.Format(format, name, GetTypeDataClass("trimedStr", type), index, tableData.type);
            }
                
        }


		/// <summary>
		///取得轉型
		/// </summary>
		string GetTypeDataClass(int index, string type)
		{
		    string str = string.Format("splitStr[{0}", index);
		    return GetTypeDataClass(str, type);
		}
		
		string GetTypeDataClass(string str, string type)
		{
			switch (type)
			{
				case "Vector2":
				case "Vector3":
					return string.Format("string.IsNullOrEmpty({1}) ? default({0}) : Utility.TypeRelate.StringToVector{0}({1})", (type == "Vector2" ? "2" : "3"), str);

				case "bool":
					return string.Format("!string.IsNullOrEmpty({0}) && Utility.TypeRelate.StringToBool({0})", str);

				case "VFixedPoint":
					return string.Format("string.IsNullOrEmpty({0}) ? VFixedPoint.Zero : Utility.TypeRelate.StringToFixedPoint({0})", str);
                case "string":
                    return string.Format("string.IsNullOrEmpty({0}) ? string.Empty : {0}", str);
                default:
					return string.Format("string.IsNullOrEmpty({1}) ? default({0}) : ({0})Convert.ChangeType({1}, typeof({0}))", type, str);
			}
		}

		string ConvertCsvTypeToCsType(string csvType)
        {
            CsvValue.CsvValueType type = CsvConfigReader.GetCsvValueType(csvType);

            string ret = string.Empty;
            switch (type)
            {
                case CsvValue.CsvValueType.None:
                    ret = "None";
                    break;
                case CsvValue.CsvValueType.Bool:
                    ret = "bool";
                    break;
                case CsvValue.CsvValueType.Int:
                    ret = "int";
                    break;
                case CsvValue.CsvValueType.Long:
                    ret = "long";
                    break;
                case CsvValue.CsvValueType.Float:
                    ret = "float";
                    break;
                case CsvValue.CsvValueType.Double:
                    ret = "double";
                    break;
                case CsvValue.CsvValueType.String:
                    ret = "string";
                    break;
                case CsvValue.CsvValueType.VFixedPoint:
                    ret = "VFixedPoint";
                    break;
                case CsvValue.CsvValueType.ArrayInt:
                    ret = "int[]";
                    break;
                case CsvValue.CsvValueType.ArrayLong:
                    ret = "long[]";
                    break;
                case CsvValue.CsvValueType.ArrayFloat:
                    ret = "float[]";
                    break;
                case CsvValue.CsvValueType.ArrayString:
                    ret = "string[]";
                    break;
                case CsvValue.CsvValueType.ArrayVFixedPoint:
                    ret = "VFixedPoint[]";
                    break;
                case CsvValue.CsvValueType.Tuple:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (string.IsNullOrEmpty(ret))
            {
                Debug.LogError(string.Format("not support type convert: source[{0}]==> target[{1}] ", csvType, ret));
                throw new ArgumentOutOfRangeException(); 
            }

            return ret;
        }


        // -----------------------------------------  导出Erlang  --------------------------------------------------
        
        /// <summary>
        /// 导出服务器配置
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="variable"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        void ExportServerConfig(string[] _Summary, string[] _Variable, string[] _Type, string content)
        {
            var tableName = _tableName.ToLower();
            WriteHrlHeader(tableName, _Summary, _Variable, _Type);

            WirteErlangSrc(_Summary, _Variable, _Type, content,  tableName);
        }

        /// <summary>
        /// 写源文件
        /// </summary>
        /// <param name="_Summary"></param>
        /// <param name="_Variable"></param>
        /// <param name="_Type"></param>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <param name="tableName"></param>
        private void WirteErlangSrc(string[] _Summary, string[] _Variable, string[] _Type, string content, 
            string tableName)
        {
            string templateDataClass = GetTemplate("ErlangSrc");
            if (string.IsNullOrEmpty(templateDataClass))
            {
                Debug.LogError("could not found template file : ErlangHeader.txt");
                return;
            }

            var reader = new StringReader(content);
            var indexStats = new Dictionary<string, Dictionary<string, List<string>>>();
            var keyList = new List<string>();

            StringBuilder sb = new StringBuilder();
            //while ((strTemp = reader.ReadLine()) != null)
            CsvReader csvReader = new CsvReader(reader, false);
            int index = 0;
            foreach (string[] splitStr in csvReader)
            {
                if (index < 2)
                {
                    index++;
                    continue;
                }
                sb.Append(BuildErlangKeySearch(tableName, _Summary, _Variable, _Type, splitStr, ref indexStats, ref keyList));

                index++;
            }

            var getIndexLists = buildErlangIndexListString(tableName, indexStats);
            var allKeyLists = string.Join(", ", keyList.ToArray());


            templateDataClass = templateDataClass.Replace("$TableName", _tableName);
            templateDataClass = templateDataClass.Replace("$LowerCaseTableName", tableName.ToLower());
            templateDataClass = templateDataClass.Replace("$UpperCaseTableName", tableName.ToUpper());
            templateDataClass = templateDataClass.Replace("$KeySearchMethods", sb.ToString());
            templateDataClass = templateDataClass.Replace("$GetIndexLists", getIndexLists.ToString());
            templateDataClass = templateDataClass.Replace("$AllKeyLists", allKeyLists.ToString());

            tableName = "cfg_" + tableName.ToLower();
            string headPath = Application.dataPath + "../../../Server/apps/config/src/" + tableName + ".erl";
            using (var writer = new StreamWriter(headPath))
            {
                writer.Write(templateDataClass);
                writer.Close();

                Debug.Log(string.Format("[TableErlangSource] is Create.\nFile:[{0}] Path:[{1}]", tableName, headPath));
            }
            reader.Close();
        }

        string BuildErlangKeySearch(string tableName, string[] _Summary, 
            string[] variable, string[] type, string[] values, 
            ref Dictionary<string, Dictionary<string, List<string>>> indexStats,
            ref List<string> keyList)
        {
            string keyVal;
            string keyType;
            var sb = BuildSearchMethods(_Summary, variable, type, values, indexStats, keyList, false, out keyVal, out keyType);

            string method = string.Format("get({0}) ->\n", GetScriptValue(keyType, keyVal, false));
            method += string.Format("	#cfg_{0}{{\n", tableName);
            method += sb.ToString();
            method += "\n		};\n\n";

            return method;
        }

        private StringBuilder BuildSearchMethods(string[] _Summary, string[] variable, string[] type, string[] values,
            Dictionary<string, Dictionary<string, List<string>>> indexStats, List<string> keyList, bool isLua, out string keyVal, out string keyType)
        {
            StringBuilder sb = new StringBuilder();

            string fmt = "		{0} = {1},\n";
            string lastlineFmt = "		{0} = {1}";
            //string[] values = line.Split(","[0]).Select(a => a.Trim()).ToArray();
            keyVal = "";
            keyType = "None";
            int validCount = 0;
            for (int i = 0; i < variable.Length; i++)
            {
                if (type[i] == IgnoreType)
                {
                    continue;
                }
                validCount++;
            }

            for (int i = 0, validIndex = 0; i < variable.Length; i++)
            {
                if (type[i] == IgnoreType)
                {
                    //Debug.Log("Ignore variable name:" + variable[i]);
                    continue;
                }
                string typeVal = type[i];
                // 字段名
                string fieldName = variable[i];
                string value = values[i];

                if (CsvConfigReader.IsKeyType(_Summary[i].ToLower()))
                {
                    keyVal = value;
                    keyType = typeVal;
                    keyList.Add(GetScriptValue(keyType, keyVal, isLua));
                }
                else if (CsvConfigReader.IsIndexType(_Summary[i].ToLower()))
                {
                    Dictionary<string, List<string>> fieldIndexDict = null;
                    if (!indexStats.TryGetValue(fieldName, out fieldIndexDict))
                    {
                        fieldIndexDict = new Dictionary<string, List<string>>();
                        indexStats.Add(fieldName, fieldIndexDict);
                    }

                    List<string> keys;
                    if (!fieldIndexDict.TryGetValue(value, out keys))
                    {
                        keys = new List<string>();
                        fieldIndexDict.Add(value, keys);
                    }

                    keys.Add(string.IsNullOrEmpty(keyVal) ? GetKeyValue(variable, type, values, _Summary) : keyVal);
                }

                if (isLua)
                {
                    sb.AppendFormat(fmt, fieldName, GetScriptValue(typeVal, value, true));
                }
                else
                {
                    sb.AppendFormat(validIndex == validCount - 1 ? lastlineFmt : fmt, fieldName.ToLower(),
                        GetScriptValue(typeVal, value, false));
                }
                
                validIndex++;
            }
            return sb;
        }

        /// <summary>
        /// 输出查询索引
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        string buildErlangIndexListString(string tableName, Dictionary<string, Dictionary<string, List<string>>> stats)
        {
            StringBuilder sb = new StringBuilder();

            string fmt = @"get_index_list(#cfg_{0}.{1}, {2}) ->
	[{3}];";

            foreach (var oneStat in stats)
            {
                foreach (var keys in oneStat.Value)
                {
                    sb.AppendFormat(fmt, tableName, oneStat.Key, keys.Key, string.Join(", ", keys.Value.ToArray()));
                    sb.Append("\n\n");
                }
            }

            return sb.ToString();
        }
        string GetKeyValue(string[] variable, string[] type, string[] values, string[] _Summary)
        {
            for (int i = 0; i < variable.Length; i++)
            {
                if (type[i] == IgnoreType)
                {
                    Debug.Log("Ignore variable name:" + variable[i]);
                    continue;
                }

                if (CsvConfigReader.IsKeyType(_Summary[i]))
                {
                    return values[i];
                }
            }

            return "xxxx";
        }

        /// <summary>
        /// erlang 头文件生成
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="_Summary"></param>
        /// <param name="variable"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        void WriteHrlHeader(string tableName, string[] _Summary, string[] variable, string[] type)
        {
            string templateDataClass = GetTemplate("ErlangHeader");
            if (string.IsNullOrEmpty(templateDataClass))
            {
                Debug.LogError("could not found template file : ErlangHeader.txt");
                return;
            }

            StringBuilder sb = new StringBuilder();

            string fmt = "	{0} = {1},\n";
            string lastlineFmt = "	{0} = {1}";
            int validCount = 0;
            for (int i = 0; i < variable.Length; i++)
            {
                if (type[i] == IgnoreType)
                {
                    continue;
                }
                validCount++;
            }
            for (int i = 0, validIndex=0; i < variable.Length; i++)
            {
                if (type[i] == IgnoreType)
                {
                    continue;
                }

                // 字段名
                string fieldName = variable[i];

                sb.AppendFormat(validIndex == validCount - 1 ? lastlineFmt : fmt, fieldName.ToLower(), GetTypeDefaultValue(type[i]));
                validIndex++;
            }

            templateDataClass = templateDataClass.Replace("$TableName", _tableName);
            templateDataClass = templateDataClass.Replace("$LowerCaseTableName", tableName.ToLower());
            templateDataClass = templateDataClass.Replace("$UpperCaseTableName", tableName.ToUpper());
            templateDataClass = templateDataClass.Replace("$MemberFields", sb.ToString());

            tableName = "cfg_" + tableName.ToLower();
            string headPath = Application.dataPath + "../../../Server/apps/config/include/" + tableName + ".hrl";
            using (var writer = new StreamWriter(headPath))
            {
                writer.Write(templateDataClass);
                writer.Close();

                Debug.Log(string.Format("[TableErlangHeader] is Create.\nFile:[{0}] Path:[{1}]", tableName, headPath));
            }
        }

        /// <summary>
        /// 获得类型缺省值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetTypeDefaultValue(string type)
        {
            //透過字元 '[' ']' 判斷是否是Array 
            bool isArray = type.EndsWith("[]");
            if (isArray)
            {
                return "[]";
            }

            if (string.CompareOrdinal(type, "float") == 0 ||
                string.CompareOrdinal(type, "double") == 0)
            {
                return "0.0";
            }

            if (string.CompareOrdinal(type, "string") == 0)
            {
                return "\"\"";
            }

            if (string.CompareOrdinal(type, "bool") == 0)
            {
                return "false";
            }

            return "0";
        }

        /// <summary>
        /// 导出erlang值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="isLua"></param>
        /// <returns></returns>
        string GetScriptValue(string type, string value, bool isLua)
        {
            string leftBrackets = isLua ? "{" : "[";
            string rightBrackets = isLua ? "}" : "]";
            string arraySeparator = isLua ? ";" : ", ";
            //透過字元 '[' ']' 判斷是否是Array 
            bool isArray = type.EndsWith("[]");
            if (isArray)
            {
                return string.Format("{0}{1}{2}" , leftBrackets, string.Join(arraySeparator, value.Split(';')), rightBrackets);
            }

            if (string.CompareOrdinal(type, "float") == 0 ||
                string.CompareOrdinal(type, "double") == 0)
            {
                return !string.IsNullOrEmpty(value) ? value : "0.0";
            }

            if (string.CompareOrdinal(type, "string") == 0)
            {
                return "\"" + value.Replace("\"","\\\"") + "\"";
            }

            if (string.CompareOrdinal(type, "bool") == 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return "false";
                }
                return Utility.TypeRelate.StringToBool(value) ? "true" : "false";
            }

            return string.IsNullOrEmpty(value) ? "0" : value;
        }


        /// <summary>
        /// 导出lua配置
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="variable"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        void ExportLuaConfig(string[] _Summary, string[] _Variable, string[] _Type, string content)
        {
            var tableName = _tableName.ToLower();

            string templateDataClass = GetTemplate("Lua");
            if (string.IsNullOrEmpty(templateDataClass))
            {
                Debug.LogError("could not found template file : Lua.txt");
                return;
            }

            var reader = new StringReader(content);

            var indexStats = new Dictionary<string, Dictionary<string, List<string>>>();
            var keyList = new List<string>();

            StringBuilder sb = new StringBuilder();
            //while ((strTemp = reader.ReadLine()) != null)
            CsvReader csvReader = new CsvReader(reader, false);
            int index = 0;
            foreach (string[] splitStr in csvReader)
            {
                if (index < 2)
                {
                    index++;
                    continue;
                }
                sb.Append(BuildLuaKeySearch(tableName, _Summary, _Variable, _Type, splitStr, ref indexStats, ref keyList));

                index++;
            }

            var getIndexLists = buildLuaIndexListString(tableName, indexStats);

            templateDataClass = templateDataClass.Replace("$TableName", _tableName);
            templateDataClass = templateDataClass.Replace("$LowerCaseTableName", tableName.ToLower());
            templateDataClass = templateDataClass.Replace("$UpperCaseTableName", tableName.ToUpper());
            templateDataClass = templateDataClass.Replace("$KeySearchMethods", sb.ToString());
            templateDataClass = templateDataClass.Replace("$GetIndexLists", getIndexLists.ToString());

            tableName = tableName.ToLower();
            string headPath = Application.dataPath + "/LuaFramework/Lua/Table/" + tableName + ".lua";
            using (var writer = new StreamWriter(headPath))
            {
                writer.Write(templateDataClass);
                writer.Close();

                Debug.Log(string.Format("[TableLuaSource] is Create.\nFile:[{0}] Path:[{1}]", tableName, headPath));
            }
            reader.Close();
        }

        string BuildLuaKeySearch(string tableName, string[] _Summary,
            string[] variable, string[] type, string[] values,
            ref Dictionary<string, Dictionary<string, List<string>>> indexStats,
            ref List<string> keyList)
        {
            string keyVal;
            string keyType;
            var sb = BuildSearchMethods(_Summary, variable, type, values, indexStats, keyList, true, out keyVal, out keyType);

            string method = string.Format("	[{0}] = {{\n", GetScriptValue(keyType, keyVal,true));
            method += sb.ToString();
            method += "	},\n\n";

            return method;
        }

        string buildLuaIndexListString(string tableName, Dictionary<string, Dictionary<string, List<string>>> stats)
        {
            StringBuilder sb = new StringBuilder();

            if (stats.Count > 0)
            {
                sb.Append("IndexTable = {\n");
                foreach (var oneStat in stats)
                {
                    sb.Append(string.Format("	[\"{0}\"] = {{\n", oneStat.Key));
                    foreach (var keys in oneStat.Value)
                    {
                        sb.AppendFormat("		[{0}] = {{{1}}},\n", keys.Key, string.Join(", ", keys.Value.ToArray()));
                    }
                    sb.Append("	},\n");
                }
                sb.Append("}\n");
            }
            else
            {
                sb.Append("IndexTable = {}");
            }
           

            return sb.ToString();
        }
    }
}
